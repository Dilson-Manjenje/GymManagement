using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using GymManagement.Api.Common;

namespace GymManagement.Api.IntegrationTests.Common;

public partial class BaseHttpService
{
    protected readonly HttpClient _client;

    protected BaseHttpService(HttpClient client)
    {
        _client = client;
    }

    protected async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<T>(url, cancellationToken);

        return response;
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T request, CancellationToken cancellationToken = default)
    {
        return await _client.PostAsJsonAsync(url, request, cancellationToken);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T request, CancellationToken cancellationToken = default)
    {
        return await _client.PutAsJsonAsync(url, request, cancellationToken);
    }


    protected ServiceResponse<T> ConvertApiExceptions<T>(ApiException ex)
    {

        if (ex.StatusCode == (int)ApiResponseStatusCode.BadRequest400 || ex.StatusCode == (int)ApiResponseStatusCode.Conflict409)
        {
            return new ServiceResponse<T>() { Message = "Invalid data on request.", ValidationErrors = ex.Response, Success = false };
        }
        else if (ex.StatusCode == (int)ApiResponseStatusCode.NotFound404)
        {
            return new ServiceResponse<T>() { Message = "Not Found.", Success = false };
        }
        else if (ex.StatusCode == (int)ApiResponseStatusCode.Unauthorized401)
        {
            return new ServiceResponse<T>() { Message = "Unauthorized to perform the operation.", Success = false };
        }
        else
        {
            return new ServiceResponse<T>() { Message = "Something went wrong, try again later.", Success = false };
        }
    }
}

public enum ApiResponseStatusCode
{
    BadRequest400 = 400,
    Unauthorized401 = 401,
    NotFound404 = 404,
    Conflict409 = 409,
    InternalServerError500 = 500
}

public class ServiceResponse<T>
{
    public string? Message { get; set; }
    public string? ValidationErrors { get; set; }
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
}

public class ApiException : Exception
{
    public int StatusCode { get; private set; }

    public string Response { get; private set; }

    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

    public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
        : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
    {
        StatusCode = statusCode;
        Response = response!;
        Headers = headers;
    }

    public override string ToString()
    {
        return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
    }
}