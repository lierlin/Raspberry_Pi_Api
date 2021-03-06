using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using IServices;
using Model;
using Model.MySql;

namespace Pi_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LedController : ControllerBase
    {
        private readonly IPiTestServices _piTestServices;
        private readonly tb_pi_test _tbPiTest;

        public LedController(IPiTestServices piTestServices, tb_pi_test tbPiTest)
        {
            _piTestServices = piTestServices;
            _tbPiTest = tbPiTest;
        }

        [HttpPost]
        public async Task<ApiMessageEntity> LedOption(int Gpio, bool Is_High)
        {
            try
            {
                var res = await _piTestServices.Led(Gpio, Is_High);
                if (res)
                {
                    return ApiMessageEntity.Ok();
                }
                return ApiMessageEntity.Error("_piTestServices 方法返回false");
            }
            catch (Exception e)
            {
                return ApiMessageEntity.Error(e);
            }
        }
    }
}