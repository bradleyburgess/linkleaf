using System.Text.Json.Serialization;

namespace LinkLeaf.Api.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApiStatusCode
{
    VALIDATION_ERROR,
    TOKEN_INVALID_OR_EXPIRED,
    INVALID_CREDENTIALS,
    USER_EXISTS
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public ApiStatusCode Code { get; set; }
    public Dictionary<string, List<string>>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> FailureResponse(
        ApiStatusCode code,
        Dictionary<string, List<string>>? errors = default,
        string? message = null
    ) =>
        new() { Code = code, Success = false, Message = message, Errors = errors };
}
