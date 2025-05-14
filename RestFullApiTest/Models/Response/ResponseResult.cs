namespace RestFullApiTest
{
    public class ResponseResult 
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public ResponseResult(int statusCode, string message, object? data = null)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
        public static ResponseResult Success(string message, object? data = null)
        {
            return new ResponseResult(200, message, data);
        }
        public static ResponseResult Error(string message, int statusCode = 500)
        {
            return new ResponseResult(statusCode, message);
        }
        public static ResponseResult NotFound(string message)
        {
            return new ResponseResult(404, message);
        }
        public static ResponseResult BadRequest(string message)
        {
            return new ResponseResult(400, message);
        }
        public static ResponseResult BadRequest(string message, object? data = null)
        {
            return new ResponseResult(400, message, data);
        }
        public static ResponseResult Created(string message, object? data = null)
        {
            return new ResponseResult(201, message, data);
        }
        public static ResponseResult Unauthorized(string message)
        {
            return new ResponseResult(401, message);
        }
        public static ResponseResult Forbidden(string message)
        {
            return new ResponseResult(403, message);
        }
        public static ResponseResult Conflict(string message)
        {
            return new ResponseResult(409, message);
        }
    }
}
