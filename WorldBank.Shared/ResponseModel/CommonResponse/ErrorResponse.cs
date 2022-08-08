using System.Text.Json;

namespace WorldBank.Shared.ResponseModel.CommonResponse
{
    public class ErrorResponse : BaseResponse<string>
    {
        public ErrorResponse(string errorCode, string errorMessage)
        {
            Error = new List<Error>();
            Error.Add(new Error { ErrorCode = errorCode, ErrorMessage = errorMessage ,FieldName=""});
        }

        public ErrorResponse(Error errors)
        {
            Error = new List<Error>();
            Error.Add(errors);
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
