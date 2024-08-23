namespace Transferencia.Application.Interfaces
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public ApiError Error { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T> { Success = true, Data = data };
        }

        public static ApiResponse<T> ErrorResponse(string errorCode, string message, object details = null, IEnumerable<object> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = new ApiError { Code = errorCode, Message = message, Details = details, Errors = errors }
            };
        }
    }
}