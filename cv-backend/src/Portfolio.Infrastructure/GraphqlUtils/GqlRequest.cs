using System.Text;
using System.Text.Json;

public static class GqlRequestExtension
{
    public static async Task<T> ExecuteGraphqlAsync<T>(
        this HttpClient httpClient,
        string query,
        object variables)
    {
        var request = new { query, variables };

        var response = await httpClient.PostAsync(
            "",
            new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json")
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        GraphqlResponse<T>? gqlResponse = JsonSerializer.Deserialize<GraphqlResponse<T>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (gqlResponse?.Errors?.Length > 0)
            throw new InvalidOperationException(gqlResponse.Errors[0].Message);

        return gqlResponse!.Data!;
    }

    public sealed class GraphqlResponse<T>
    {
        public T? Data { get; set; }
        public GraphqlError[]? Errors { get; set; }
    }

    public sealed class GraphqlError
    {
        public string Message { get; set; } = default!;
    }
}