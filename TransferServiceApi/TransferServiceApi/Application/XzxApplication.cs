using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using TransferServiceApi.Common;
using TransferServiceApi.Help;
using TransferServiceApi.Models.XzxModel;
using static TransferServiceApi.Help.LogHelper;

namespace TransferServiceApi.Application
{
    /// <summary>
    /// 新中新统一Api接口
    /// </summary>
    public class XzxApplication
    {
        /// <summary>
        /// 获取authorize_access_token
        /// </summary>
        /// <param name="TokenType">令牌类型：1-消费码访问令牌 2-无卡扣款、退款访问令牌</param>
        /// <returns></returns>
        public static AuthorizeAccessTokenMap AuthorizeAccessToken(int TokenType)
        {
            AuthorizeAccessTokenMap data = new AuthorizeAccessTokenMap();
            switch (TokenType)
            {
                case 1://消费码
                    data = MemoryCacheHelper.Get<AuthorizeAccessTokenMap>("XzxApi_QRCode_authorize_access_token");
                    if (data == null || string.IsNullOrWhiteSpace(data.access_token))
                    {
                        data = GetAuthorizeAccessToken(TokenType);
                        if (data != null && data.expires_in > 0)
                        {
                            int seconds = data.expires_in;
                            MemoryCacheHelper.Set("XzxApi_QRCode_authorize_access_token", data, new TimeSpan(0, 0, seconds));
                        }
                    }
                    break;
                case 2://无卡扣款、退款
                    data = MemoryCacheHelper.Get<AuthorizeAccessTokenMap>("XzxApi_Consume_authorize_access_token");
                    if (data == null || string.IsNullOrWhiteSpace(data.access_token))
                    {
                        data = GetAuthorizeAccessToken(TokenType);
                        if (data != null && data.expires_in > 0)
                        {
                            int seconds = data.expires_in;
                            MemoryCacheHelper.Set("XzxApi_Consume_authorize_access_token", data, new TimeSpan(0, 0, seconds));
                        }
                    }
                    break;
            }
            return data;
        }

        /// <summary>
        /// 强制获取访问令牌
        /// </summary>
        /// <param name="TokenType">令牌类型：1-消费码访问令牌 2-无卡扣款、退款访问令牌</param>
        /// <returns></returns>
        private static AuthorizeAccessTokenMap GetAuthorizeAccessToken(int TokenType)
        {
            AuthorizeAccessTokenMap tokenMap = new AuthorizeAccessTokenMap();
            string des_key = string.Empty;//加密解密deskey
            string app_key = string.Empty;//提供的appkey
            string privateKeyPath = string.Empty;//私钥路径地址
            switch (TokenType)
            {
                case 1://消费码
                    des_key = QRCodeKey.CodeDesKey;
                    app_key = QRCodeKey.CodeAppKey;
                    privateKeyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "wwwroot\\QrCodeKey\\svr_private.key";
                    break;
                case 2://无卡扣款、退款
                    des_key = ConsumeKey.ConsumeDesKey;
                    app_key = ConsumeKey.ConsumeAppKey;
                    privateKeyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "wwwroot\\ConsumeKey\\svr_private.key";
                    break;
            }
            string access_Token = "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            string format = "json";
            string method = "synjones.authorize.access_token";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sign_method = "rsa";
            string v = "2.0";
            string request = "{\"authorize_access_token\": {}}";
            SafeUtilHelper client = new SafeUtilHelper();
            request = client.GetEncryptDES(request, des_key);
            string sign = client.Sign("access_token" + access_Token + "app_key" + app_key + "format" + format + "method" + method + "request" + request + "sign_method" + sign_method + "timestamp" + timestamp + "v" + v, privateKeyPath);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
            //Post内容
            var queryPost = new Dictionary<string, string>
                {
                    {"access_token", access_Token},
                    {"app_key", app_key},
                    {"format", format},
                    {"method",method},
                    {"request", request},
                    {"sign", sign},
                    {"sign_method", sign_method},
                    {"timestamp", timestamp},
                    {"v", v}
                };
            //日志记录
            switch (TokenType)
            {
                case 1://消费码
                    Log.Info($"获取消费码访问令牌请求参数：{JsonConvert.SerializeObject(queryPost)}");
                    break;
                case 2://无卡扣款、退款
                    Log.Info($"获取无卡扣款、退款访问令牌请求参数：{JsonConvert.SerializeObject(queryPost)}");
                    break;
            }
            var httpContent = new FormUrlEncodedContent(queryPost);
            httpContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
            string Uri = XzxConfig.TsmUrl;//TSM后台访问地址
            var response = httpClient.PostAsync(new Uri(Uri), httpContent);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string[] strresponse = response.Result.Content.ReadAsStringAsync().Result.ToString().Split('&');
                string errcode = string.Empty;
                for (int i = 0; i < strresponse.Length; i++)
                {
                    switch (strresponse[i].Split('=')[0])
                    {
                        case "errcode":
                            errcode = strresponse[i].Split('=')[1];
                            break;
                        case "request":
                            request = strresponse[i].Split('=')[1];
                            break;
                        case "sign":
                            sign = strresponse[i].Split('=')[1];
                            break;
                    }
                }
                request = HttpUtility.UrlDecode(request);
                if (errcode == "0")
                {
                    string strinfo = client.GetDecryptDES(request, des_key);
                    //返回获得的数据
                    tokenMap = JsonConvert.DeserializeObject<AuthorizeAccessTokenMap>(strinfo);
                }
                else
                    tokenMap.errmsg = $"获取访问令牌失败，错误码[errcode]：{errcode}";
            }
            else
                tokenMap.errmsg = $"获取访问令牌失败，请求状态码：{response.Result.StatusCode}";
            //日志记录
            switch (TokenType)
            {
                case 1://消费码
                    Log.Info($"获取消费码访问令牌返回结果：{JsonConvert.SerializeObject(tokenMap)}");
                    break;
                case 2://无卡扣款、退款
                    Log.Info($"获取无卡扣款、退款访问令牌返回结果：{JsonConvert.SerializeObject(tokenMap)}");
                    break;
            }
            return tokenMap;
        }

        public static OnecardBarcodeMap GetOnecardBarcode()
        {
            return null;
        }
    }
}
