namespace Inventory.API.Models
{
    public class Response<T> : ResponseBase
    {
        public T Data {  get; set; }
    }

    public class ResponseBase
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
