using System.Text.Json;

namespace WorldBank.Shared.ResponseModel.CommonResponse
{
    public class ErrorResponse : BaseResponse<string>
    {
        public ErrorResponse(string errorCode, string errorMessage)
        {
            Error = new Error { ErrorCode = errorCode, ErrorMessage = errorMessage };
        }

        public ErrorResponse(Error errors)
        {
            Error = errors;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
