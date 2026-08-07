namespace SistemaVentasETL.Load.Utilities;

public sealed class ServiceResult<T>
{
    public bool IsSuccess { get; }

    public T? Data { get; }

    public string Message { get; }

    private ServiceResult(
        bool isSuccess,
        T? data,
        string message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }

    public static ServiceResult<T> Success(
        T data,
        string message = "")
    {
        return new ServiceResult<T>(
            true,
            data,
            message);
    }

    public static ServiceResult<T> Failure(
        string message)
    {
        return new ServiceResult<T>(
            false,
            default,
            message);
    }
}