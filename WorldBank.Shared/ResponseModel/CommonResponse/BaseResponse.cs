namespace WorldBank.Shared.ResponseModel.CommonResponse
{
    public class BaseResponse<T>
    {
        public Error Error { get; set; }
        /// <summary>
        /// Response data
        /// </summary>
        public T? Responsedata { get; set; }
    }
    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
