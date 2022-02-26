using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using IServices;
using Model;
using Model.pi;

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


        /// <summary>
        /// Manual Drive   
        /// </summary>
        /// <param name="direction">forward backward leftward rightward</param>
        /// <param name="duration">时长(s)</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiMessageEntity> ManualDrive(string direction, int  duration)
        {
            try
            {
                var res = _piTestServices.ManualDrive(direction, duration);
                if (res)
                {
                    return ApiMessageEntity.Ok();
                }
                return ApiMessageEntity.Error("ManualDrive 方法返回false");
            }
            catch (Exception e)
            {
                return ApiMessageEntity.Error(e);
            }
        }


        [HttpPost]
        public async Task<ApiMessageEntity> LedOption(int Gpio, bool Is_High)
        {
            try
            {
                var res = _piTestServices.Led(Gpio, Is_High);
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

        //[HttpPost]
        //public async Task<ApiMessageEntity> TestAsync()
        //{
        //    try
        //    {
        //        var s1 =  _piTestServices.Transaction_Test();

        //        return ApiMessageEntity.Error("_piTestServices 方法返回false");
        //    }
        //    catch (Exception e)
        //    {
        //        return ApiMessageEntity.Error(e);
        //    }
        //}

        [HttpPost]
        public async Task<ApiMessageEntity> MorseCode(string morseMsg)
        {
            try
            {
                var res = _piTestServices.Morse(morseMsg);
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
