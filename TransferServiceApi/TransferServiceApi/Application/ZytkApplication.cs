using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TransferServiceApi.Common;
using TransferServiceApi.Help;
using TransferServiceApi.Models.ZytkModel;

namespace TransferServiceApi.Application
{
    /// <summary>
    /// 正元一卡通统一Api接口
    /// </summary>
    public class ZytkApplication
    {
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public static AccessTokenMap AccessToken()
        {
            var res = MemoryCacheHelper.Get<AccessTokenMap>("ZyApi_access_token");
            if (res == null || string.IsNullOrWhiteSpace(res.access_token))
            {
                res = GetAccessToken();
                if (res != null && !string.IsNullOrWhiteSpace(res.expires_in))
                {
                    int seconds = int.Parse(res.expires_in);
                    MemoryCacheHelper.Set("ZyApi_access_token", res, new TimeSpan(0, 0, seconds));
                }
            }
            else
            {
                if (!CheckAccessToken(res.access_token))
                {
                    res = GetAccessToken();
                    if (res != null && !string.IsNullOrWhiteSpace(res.expires_in))
                    {
                        int seconds = int.Parse(res.expires_in);
                        MemoryCacheHelper.Set("ZyApi_access_token", res, new TimeSpan(0, 0, seconds));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 验证access_token
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        private static bool CheckAccessToken(string access_token)
        {
            string url = ZytkConfig.ApiUrl + "/api/common/systemdocking/getaccfixid?access_token=" + access_token;
            Dictionary<string, string> paramsMap = new Dictionary<string, string>
            {
                { "epId", "" }
            };
            var postData = JsonConvert.SerializeObject(paramsMap);
            var res = HttpClientHelper.HttpPost(url, postData, "application/json");
            var result = JsonConvert.DeserializeObject<ZytkResultData<AccfixidData>>(res);
            if (result.code != "0" || result.code == "40004")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 强制获取access_token
        /// </summary>
        /// <returns></returns>
        public static AccessTokenMap GetAccessToken()
        {
            string url = ZytkConfig.ApiUrl + "/api/token?appid=" + ZytkConfig.AppId + "&appsecret=" + ZytkConfig.AppSecret;
            var res = HttpClientHelper.HttpGet(url, "application/json");
            var result = JsonConvert.DeserializeObject<AccessTokenMap>(res);
            return result;
        }

        /// <summary>
        /// 获取通用二维码
        /// </summary>
        /// <param name="accNum">一卡通帐号</param>
        /// <returns></returns>
        public static ZytkResultData<CommonQRcodeData> CommonQRcode(string accNum)
        {
            string access_token = AccessToken().access_token;
            string url = ZytkConfig.ApiUrl + "/api/common/payservice/getcommonqrcode?access_token=" + access_token;
            Dictionary<string, string> paramsMap = new Dictionary<string, string>
            {
                { "accNum", accNum }
            };
            var data = SignHelper.ASCIISort(paramsMap) + "&key=" + ZytkConfig.AppId;
            var sign = MD5Helper.MD5Encrypt(data).ToUpper();
            paramsMap.Add("sign", sign);
            var postData = JsonConvert.SerializeObject(paramsMap);
            var res = HttpClientHelper.HttpPost(url, postData, "application/json");
            var result = JsonConvert.DeserializeObject<ZytkResultData<CommonQRcodeData>>(res);
            return result;
        }
    }
}
