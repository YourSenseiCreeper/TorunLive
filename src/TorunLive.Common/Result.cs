namespace TorunLive.Common
{
    public class Result<T>(bool isSuccess, T resultObject, string? errorMessage) where T : class
    {
        public bool IsSuccess { get; } = isSuccess;
        public bool IsFailure { get { return !IsSuccess; } }
        public string? ErrorMessage { get; } = errorMessage;
        public T ResultObject { get; } = resultObject;

        public static Result<T> Success(T resultObject) => new(true, resultObject, null);
        public static Result<T> Failure(string message) => new(false, null, message);
    }
}
