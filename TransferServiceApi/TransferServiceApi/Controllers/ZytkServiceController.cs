using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using TransferServiceApi.Application;
using TransferServiceApi.Common;
using TransferServiceApi.Models.ZytkModel;
using static TransferServiceApi.Help.LogHelper;

namespace TransferServiceApi.Controllers
{
    /// <summary>
    /// 正元一卡通方法
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    public class ZytkServiceController : ControllerBase
    {
        private readonly DataResult dataResult = new DataResult();

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetAccessToken()
        {
            try
            {
                var data = ZytkApplication.AccessToken();
                if (data != null && !string.IsNullOrWhiteSpace(data.access_token))
                {
                    dataResult.BS = "1";
                    dataResult.Msg = "查询成功！";
                    dataResult.Rows = data;
                    dataResult.Total = 1;
                }
                else
                {
                    dataResult.BS = "0";
                    dataResult.Msg = "查询不到数据！";
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取AccessToken系统错误", ex);
                dataResult.BS = "-99";
                dataResult.Msg = "系统错误！";
            }
            return DataSerialize.StringOfObject(dataResult, 1);
        }

        /// <summary>
        /// 获取通用二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetCommonQRcode([FromQuery] CommonQRcodeParam model)
        {
            try
            {
                var data = ZytkApplication.CommonQRcode(model.accNum);
                if (data != null && !string.IsNullOrWhiteSpace(data.code))
                {
                    dataResult.BS = "1";
                    dataResult.Msg = "查询成功！";
                    dataResult.Rows = data;
                    dataResult.Total = 1;
                }
                else
                {
                    dataResult.BS = "0";
                    dataResult.Msg = "查询不到数据！";
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取通用二维码系统错误", ex);
                dataResult.BS = "-99";
                dataResult.Msg = "系统错误！";
            }
            return DataSerialize.StringOfObject(dataResult, 1);
        }
    }
}
