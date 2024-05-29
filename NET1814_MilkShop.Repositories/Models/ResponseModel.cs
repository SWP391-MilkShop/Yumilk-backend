namespace NET1814_MilkShop.Repositories.Models
{
    public class ResponseModel
    {
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public object? Data { get; set; }
        public ResponseModel() { }
        private ResponseModel(string Status, string Message, object? Data)
        {
            this.Status = Status;
            this.Message = Message;
            this.Data = Data;
        }
        /// <summary>
        /// This method is used to return success message (Code 200)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseModel Success(string message, object? data)
        {
            return new ResponseModel("200 Success", message, data);
        }
        /// <summary>
        /// This method is used to return error message (Code 500)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseModel Error(string message)
        {
            return new ResponseModel("500 Error", message, null);
        }
        /// <summary>
        /// This method is used to return not found message (Code 404)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseModel NotFound(string message)
        {
            return new ResponseModel("404 Not Found", message, null);
        }
        /// <summary>
        /// This method is used to return bad request message (Code 400)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseModel BadRequest(string message)
        {
            return new ResponseModel("400 Bad Request", message, null);
        }
    }

}
