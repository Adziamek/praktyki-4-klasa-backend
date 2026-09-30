namespace Warehouse.Application.Common;

public record OperationResult(
    string? Error = null,
    int? StatusCode = null)
{
    public bool Success => Error == null;

    public static OperationResult Ok() =>
        new();

    public static OperationResult Fail(string error, int statusCode) =>
        new(error, statusCode);
}

public record OperationResult<T>(
    T? Data,
    string? Error = null,
    int? StatusCode = null)
{
    public bool Success => Error == null;

    public static OperationResult<T> Ok(T data) =>
        new(data);

    public static OperationResult<T> Ok() =>
        new(default);

    public static OperationResult<T> Fail(string error, int statusCode) =>
        new(default, error, statusCode);
}