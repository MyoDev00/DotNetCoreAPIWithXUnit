using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.Constant
{
    public  class Constant
    {
        public struct ErrorCode
        {
            public const string InternalServerError = "IntSevEr";
            public const string LoginFailed = "loginFail";
        }

        public struct ErrorMessage
        {
            public const string InternalServerError = "Internal server error!";
            public const string LoginFailed = "User name or password is incorrect!";
        }

    }
}
