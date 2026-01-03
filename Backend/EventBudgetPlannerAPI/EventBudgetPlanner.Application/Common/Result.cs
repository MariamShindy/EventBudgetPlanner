using Microsoft.AspNetCore.Http;

namespace EventBudgetPlanner.Application.Common
{
    //Result wrapper for operation outcomes with success/failure handling
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? Error { get; }
        public int StatusCode { get; }

        private Result(bool isSuccess, T? data, string? error, int statusCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
            StatusCode = statusCode;
        }

        public static Result<T> Success(T data, int statusCode = 200) => new(true, data, null, statusCode);
        public static Result<T> Failure(string error, int statusCode = 400) => new(false, default, error, statusCode);
        public static Result<T> NotFound(string error = "Resource not found") => new(false, default, error, StatusCodes.Status404NotFound);
        public static Result<T> BadRequest(string error = "Error happen") => new(false, default, error, StatusCodes.Status400BadRequest);

    }

    /// <summary>Result wrapper for operations without return data</summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public int StatusCode { get; }

        private Result(bool isSuccess, string? error, int statusCode)
        {
            IsSuccess = isSuccess;
            Error = error;
            StatusCode = statusCode;
        }

        public static Result Success(int statusCode = 200) => new(true, null, statusCode);
        public static Result Failure(string error, int statusCode = 400) => new(false, error, statusCode);
        public static Result NotFound(string error = "Resource not found") => new(false, error, 404);
    }
}



