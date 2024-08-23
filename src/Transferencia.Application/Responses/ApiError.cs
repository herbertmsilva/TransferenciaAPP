namespace Transferencia.Application.Interfaces
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public object Details { get; set; }
        public IEnumerable<object> Errors { get; set; }

    }
}