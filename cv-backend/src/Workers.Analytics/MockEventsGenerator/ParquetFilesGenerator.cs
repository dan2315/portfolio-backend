using Microsoft.Extensions.Hosting;
using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator;

public sealed class ParquetFilesGenerator : BackgroundService
{
    private const string BucketName = "activity_sessions";
    private const int BatchSize = 50_000;
    private const long MaxFileSizeBytes = 256 * 1024 * 1024;
    private const int MaxFiles = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Generator executed v2");
        var generator = new SessionGenerator();
        var buffer = new List<ActivityEvent>(BatchSize);

        var folder = Path.Combine("/tmp", "mock_data");
        Directory.CreateDirectory(folder);

        int fileIndex = 1;
        bool rotatingFiles = true;

        Console.WriteLine($"Starting generation job for {MaxFiles} files of size {MaxFileSizeBytes}");

        try
        {
            while (rotatingFiles && fileIndex <= MaxFiles && !stoppingToken.IsCancellationRequested)
            {
                var fileId = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";
                var fileName = $"activity-{fileId}.parquet";
                var filePath = Path.Combine(folder, fileName);
                Console.WriteLine($"[INFO] Writing to {filePath}");

                await using (var stream = File.Create(filePath))
                await using (var writer = await ParquetWriterHelper.CreateWriter(stream))
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var sessionId = Guid.NewGuid();
                        var anonId = Guid.NewGuid();
                        var startTime = DateTime.Now - TimeSpan.FromDays(30);

                        foreach (var ev in generator.GenerateSession(startTime, sessionId, anonId))
                        {
                            buffer.Add(ev);

                            if (buffer.Count >= BatchSize)
                            {
                                await writer.WriteRowGroup(buffer);
                                buffer.Clear();
                            }
                        }

                        if (stream.Length > MaxFileSizeBytes)
                            break;
                    }

                    if (buffer.Count > 0)
                    {
                        await writer.WriteRowGroup(buffer);
                        buffer.Clear();
                    }
                }

                var objectName = $"analytics/activity/{Path.GetFileName(filePath)}";

                Console.WriteLine($"[INFO] Started uploading as {objectName}");
                await GcsUploader.UploadAsync(
                    BucketName,
                    filePath,
                    objectName,
                    stoppingToken
                );
                Console.WriteLine($"[INFO] Upload completed");

                fileIndex++;
            }



            Console.WriteLine("[INFO] Mock data generation finished.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception occured: {e}");
            throw;
        }
    }
}
