using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldBank.API.Business;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using static WorldBank.Shared.Constant.Constant;

namespace WorldBank.API.Controllers
{
    [Route("CustomerManagement")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerBL customerBL;

        public CustomerController(ICustomerBL customerBL)
        {
            this.customerBL = customerBL;
        }

        [HttpPost]
        [Route("Customer")]
        [Authorize]
        public async Task<IActionResult> PostCustomer(PostCustomerRequest request)
        {
            BaseResponse<PostCustomerResponse> response = await customerBL.PostCustomer(request);

            if (response.Error == null)
                return Ok(response);
            else if(response.Error.Any(x=>x.ErrorCode==ErrorCode.IBANNumberGenerationFailed))
                return StatusCode(StatusCodes.Status503ServiceUnavailable,response);
            else
                return StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPut]
        [Route("Customer")]
        [Authorize]
        public async Task<IActionResult> PutCustomer(PutCustomerRequest request)
        {
            BaseResponse<PutCustomerResponse> response = await customerBL.PutCustomer(request);

            if (response.Error == null)
                return Ok(response);
            else
                return StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet]
        [Route("Customer")]
        [Authorize]
        public async Task<IActionResult> GetCustomer([FromQuery] GetCustomersRequest request)
        {
            BaseResponse<GetCustomersResponse> response = await customerBL.GetCustomers(request);

            if (response.Error == null)
                return Ok(response);
            else
                return NoContent();
        }

        [HttpGet]
        [Route("Customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetCustomerDetail(Guid customerId)
        {
            BaseResponse<GetCustomerDetailResponse> response = await customerBL.GetCustomerDetail(customerId);

            if (response.Error == null)
                return Ok(response);
            else
                return NoContent();
        }
    }
}
