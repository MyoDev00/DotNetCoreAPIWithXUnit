using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.API.Business;
using WorldBank.API.Controllers;
using WorldBank.API.Helper;
using WorldBank.API.Test.Fixture;
using WorldBank.Entities;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;

namespace WorldBank.API.Test.ControllerTest
{
    public class DataController_Test : IClassFixture<TestDatabaseFixture>
    {
        public DataController controller;
        public WorldBankDBContext Context { get; }
        MockData mockData;
        IMapper mapper;
        public DataController_Test(TestDatabaseFixture dBFixture)
        {
            Context = dBFixture.CreateContext();

            var unitOfWork = new UnitOfWork<WorldBankDBContext>(Context);
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            mapper = mapperConfig.CreateMapper();

            var masterDataBL = new MasterDataBL(unitOfWork,mapper);

            controller = new DataController(masterDataBL);
        }
        

        [Fact]
        public async Task Test_GetMasterData()
        {
            #region Arrange

            #endregion

            #region Act
            var response = await controller.GetMasterData();
            var okResponse = response as OkObjectResult;
            var data = okResponse.Value as BaseResponse<GetMasterDataResponse>;

            #endregion

            #region Assert
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.NotNull(data.Responsedata);
            Assert.Null(data.Error);
            Assert.NotNull(data.Responsedata.Currencies);
            Assert.NotNull(data.Responsedata.BankAccountTypes);
            Assert.NotNull(data.Responsedata.TransactionTypes);
            Assert.NotNull(data.Responsedata.AuditTypes);
            #endregion
        }
    }
}
