using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferServiceApi.Application;
using TransferServiceApi.Common;
using TransferServiceApi.Models.XzxModel;
using static TransferServiceApi.Help.LogHelper;

namespace TransferServiceApi.Controllers
{
    /// <summary>
    /// 新中新方法
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    public class XzxServiceController : ControllerBase
    {
        private readonly DataResult dataResult = new DataResult();

        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetAuthorizeAccessToken([FromQuery] AccessTokenParam model)
        {
            try
            {
                if (model.TokenType == 0)
                {
                    dataResult.BS = "-1";
                    dataResult.Msg = "令牌类型不能为空！";
                    return DataSerialize.StringOfObject(dataResult, 1);
                }
                var data = XzxApplication.AuthorizeAccessToken(model.TokenType);
                if (data != null && !string.IsNullOrWhiteSpace(data.access_token))
                {
                    dataResult.BS = "1";
                    dataResult.Msg = "获取成功！";
                    dataResult.Rows = data;
                    dataResult.Total = 1;
                }
                else
                {
                    dataResult.BS = "0";
                    dataResult.Msg = "获取失败！";
                    dataResult.Rows = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取访问令牌系统错误", ex);
                dataResult.BS = "-99";
                dataResult.Msg = "系统错误！";
            }
            return DataSerialize.StringOfObject(dataResult, 1);
        }

        /// <summary>
        /// 获取账户二维码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetOnecardBarcode([FromQuery] OnecardBarcodeParam model)
        {
            try
            {
                var data = XzxApplication.OnecardBarcode(model.account, model.paytype, model.payacc);
                if (data != null && !string.IsNullOrWhiteSpace(data.barcode))
                {
                    dataResult.BS = "1";
                    dataResult.Msg = "获取成功！";
                    dataResult.Rows = data;
                    dataResult.Total = 1;
                }
                else
                {
                    dataResult.BS = "0";
                    dataResult.Msg = "获取失败！";
                    dataResult.Rows = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取账户二维码系统错误", ex);
                dataResult.BS = "-99";
                dataResult.Msg = "系统错误！";
            }
            return DataSerialize.StringOfObject(dataResult, 1);
        }
    }
}
