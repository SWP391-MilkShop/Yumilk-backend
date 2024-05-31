namespace NET1814_MilkShop.Repositories.Models
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public object? Data { get; set; }

        public ResponseModel() { }
        private ResponseModel(int statusCode, string status, string message, object? data)
        {
            StatusCode = statusCode;
            Status = status;
            Message = message;
            Data = data;
        }
        /// <summary>
        /// Return a success response with a message and data (Status Code: 200)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseModel Success(string message, object? data)
        {
            return new ResponseModel(200, "Success", message, data);
        }
        /// <summary>
        /// Return an error response with a message (Status Code: 500)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseModel Error(string message)
        {
            return new ResponseModel(500, "Error", message, null);
        }
        /// <summary>
        /// Return a not found response with a message (Status Code: 404)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseModel NotFound(string message)
        {
            return new ResponseModel(404, "Not Found", message, null);
        }
        /// <summary>
        /// Return a bad request response with a message (Status Code: 400)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseModel BadRequest(string message)
        {
            return new ResponseModel(400, "Bad Request", message, null);
        }
    }

}
