using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.Constant
{
    public  class Constant
    {
        public enum CommonStatus
        {
            Deleted = -1,
            Inactive = 0,
            Active = 1,
        }
        public struct ErrorCode
        {
            public const string InternalServerError = "IntSevEr";
            public const string LoginFailed = "loginFail";
            public const string UnAuthorize = "UnAuthorize";

            public const string EmailAlreadyUsed = "EmailUsed";
            public const string MobileAlreadyUsed = "MobileUsed";
            public const string IdentityAlreadyUsed = "IdentityUsed";
            public const string IBANNumberGenerationFailed = "IBANFailed";
            public const string CustomerNotFound = "CustomerNF";

            public const string InvalidAmount = "InvalidAmount";
            public const string AcoountIsInactive = "AccountInactive";
            public const string NotEnoughBalance = "NotEnoughBalance";
        }

        public struct ErrorMessage
        {
            public const string InternalServerError = "Internal server error!";
            public const string LoginFailed = "User name or password is incorrect!";
            public const string UnAuthorize = "Invalid Token or Token Expire!";

            public const string EmailAlreadyUsed = "Email already used";
            public const string MobileAlreadyUsed = "Mobil already used";
            public const string IdentityAlreadyUsed = "Identity number already used";
            public const string IBANNumberGenerationFailed = "Failed to generate IBAN Number";
            public const string CustomerNotFound = "Customer not found";

            public const string InvalidAmount = "Inavlid Amount!";
            public const string AcoountIsInactive = "Customer or bank account is inactive!";
            public const string NotEnoughBalance = "Not Enough Balance.";
        }

        public struct ErrorFieldName
        {
            public const string Email = "Email";
            public const string Mobile = "Mobile";
            public const string IdentityCardNo = "IdentityCardNo";
            public const string InvalidAmount = "Amount";
            public const string CustomerNotFound = "CustomerId";
            public const string NotEnoughBalance = "Amount";
        }
        public struct CustomClaims
        {
            public const string Name = "Name";
            public const string AccountId = "AccountId";
        }

        public struct TransactionType
        {
            public const string BankDeposit = "BankDeposit";
            public const string FundTransfer = "FundTransfer";
        }
        public struct TransactionChargeTypes
        {
            public const string BankDeposit="BankDeposit";
            public const string FundTransfer = "FundTransfer";
        }

        public struct AuditType
        {
            public const string BankDeposit = "BankDeposit";
            public const string FundTransfer = "FundTransfer";
        }
    }
}
