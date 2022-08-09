using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldBank.API.Business;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;

namespace WorldBank.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController:ControllerBase
    {
        private readonly IMasterDataBL masterDataBL;

        public DataController(IMasterDataBL masterDataBL)
        {
            this.masterDataBL = masterDataBL;
        }


        [HttpGet]
        [Route("MasterData")]
        [Authorize]
        public async Task<IActionResult> GetMasterData()
        {
            BaseResponse<GetMasterDataResponse> response = await masterDataBL.GetMasterData();

            return Ok(response);
        }
    }
}
