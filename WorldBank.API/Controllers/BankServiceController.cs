using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldBank.API.Business;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BankServiceController : ControllerBase
    {
        private readonly IBankServiceBL bankServiceBL;

        public BankServiceController(IBankServiceBL bankServiceBL)
        {
            this.bankServiceBL = bankServiceBL;
        }

        [HttpPost]
        [Route("Deposit")]
        [Authorize]
        public async Task<IActionResult> PostDeposit(PostDepositRequest request)
        {
            var staffId = Guid.Parse(HttpContext.User.FindFirst(x => x.Type == CustomClaims.AccountId).Value);
            BaseResponse<PostDepositResponse> response = await bankServiceBL.PostDeposit(request,staffId);

            return Ok(response);
        }

        [HttpPost]
        [Route("FundTransfer")]
        [Authorize]
        public async Task<IActionResult> FundTransfer(PostFundTransferRequest request)
        {
            BaseResponse<PostFundTransferResponse> response = await bankServiceBL.PostFundTransfer(request);

            return Ok(response);
        }
    }
}
