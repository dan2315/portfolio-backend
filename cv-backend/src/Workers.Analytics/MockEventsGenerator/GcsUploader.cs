using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace Workers.Analytics.MockEventsGenerator;

public static class GcsUploader
{
    private static readonly StorageClient Client = StorageClient.Create();

    public static async Task UploadAsync(
        string bucket,
        string localFile,
        string objectName,
        CancellationToken ct)
    {
        
        await using var fs = File.OpenRead(localFile);

        await Client.UploadObjectAsync(
            bucket,
            objectName,
            "application/octet-stream",
            fs,
            cancellationToken: ct
        );
    }
}