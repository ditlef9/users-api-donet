public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; } // Optional: can hold any additional data you want to return.

    public ApiResponse(bool success, string message, object data = null)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}
