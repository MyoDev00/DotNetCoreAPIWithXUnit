namespace WorldBank.Shared.ResponseModel.CommonResponse
{
    public class BaseResponse<T>
    {
        public List<Error> Error { get; set; }
        /// <summary>
        /// Response data
        /// </summary>
        public T? Responsedata { get; set; }
    }
    public class Error
    {
        public string FieldName { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
