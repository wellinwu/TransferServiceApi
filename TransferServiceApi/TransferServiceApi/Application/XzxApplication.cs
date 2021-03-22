using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using TransferServiceApi.Common;
using TransferServiceApi.Help;
using TransferServiceApi.Models.XzxModel;
using TransferServiceApi.Models.XzxModel.Entity;
using static TransferServiceApi.Help.LogHelper;

namespace TransferServiceApi.Application
{
    /// <summary>
    /// 新中新统一Api接口
    /// </summary>
    public class XzxApplication
    {
        /// <summary>
        /// 访问令牌
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
            XzxResultData xzxResult = new XzxResultData();
            var response = httpClient.PostAsync(new Uri(Uri), httpContent);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string[] strresponse = response.Result.Content.ReadAsStringAsync().Result.ToString().Split('&');
                for (int i = 0; i < strresponse.Length; i++)
                {
                    switch (strresponse[i].Split('=')[0])
                    {
                        case "errcode":
                            xzxResult.errcode = strresponse[i].Split('=')[1];
                            break;
                        case "request":
                            xzxResult.request = strresponse[i].Split('=')[1];
                            break;
                        case "sign":
                            xzxResult.sign = strresponse[i].Split('=')[1];
                            break;
                    }
                }
                Log.Info($"XZX返回访问令牌信息：{JsonConvert.SerializeObject(xzxResult)}");
                if (xzxResult.errcode == "0")
                {
                    string DecodeResponse = HttpUtility.UrlDecode(xzxResult.request);
                    string strinfo = client.GetDecryptDES(DecodeResponse, des_key);
                    Log.Info($"XZX访问令牌请求返回request解析：{strinfo}");
                    //返回获得的数据
                    JObject jo = (JObject)JsonConvert.DeserializeObject(strinfo);
                    AuthorizeAccessTokenMap authorizeAccess = new AuthorizeAccessTokenMap
                    {
                        retcode = jo["authorize_access_token"]["retcode"].ToString(),
                        errmsg = jo["authorize_access_token"]["errmsg"].ToString(),
                        access_token = jo["authorize_access_token"]["access_token"].ToString(),
                        expires_in = int.Parse(jo["authorize_access_token"]["expires_in"].ToString()),
                    };
                    tokenMap = authorizeAccess;
                }
                else
                    tokenMap.errmsg = $"获取访问令牌失败，错误码[errcode]：{xzxResult.errcode}";
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

        /// <summary>
        /// 账户二维码
        /// </summary>
        /// <param name="account">一卡通账户</param>
        /// <param name="paytype">支付方式：
        /// 1、 校园卡账户支付
        /// 2、 绑定银行卡支付
        /// 3、 自定义银行卡支付</param>
        /// <param name="payacc">支付账号：<summary>
        /// paytype 为 1 时此字段，值可为：
        /// ###：为卡账户；其他为电子账户类型
        /// paytype 为 2 时此字段，值可为：空
        /// paytype 为 3 时此字段，值可为：银行卡号
        /// </summary></param>
        /// <returns></returns>
        public static OnecardBarcodeMap OnecardBarcode(string account, string paytype, string payacc)
        {
            OnecardBarcodeMap onecardResult = new OnecardBarcodeMap();
            var tokenMap = AuthorizeAccessToken(1);
            string access_Token = tokenMap.access_token;
            string des_key = QRCodeKey.CodeDesKey;
            string app_key = QRCodeKey.CodeAppKey;
            string format = "json";
            string method = "synjones.onecard.barcode.get";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sign_method = "rsa";
            string v = "2.0";
            string request = "{\"barcode_get\":{\"account\":\"" + account + "\",\"paytype\":\"" + paytype + "\",\"payacc\":\"" + payacc + "\"}}";
            Log.Info($"获取账户二维码请求参数：{request}");
            SafeUtilHelper client = new SafeUtilHelper();
            request = client.GetEncryptDES(request, des_key);
            string privateKeyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\svr_private.key";
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
            var httpContent = new FormUrlEncodedContent(queryPost);
            httpContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
            string Uri = XzxConfig.TsmUrl;//TSM访问地址
            XzxResultData xzxResult = new XzxResultData();
            var response = httpClient.PostAsync(new Uri(Uri), httpContent);
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string[] strresponse = response.Result.Content.ReadAsStringAsync().Result.ToString().Split('&');
                for (int i = 0; i < strresponse.Length; i++)
                {
                    switch (strresponse[i].Split('=')[0])
                    {
                        case "errcode":
                            xzxResult.errcode = strresponse[i].Split('=')[1];
                            break;
                        case "request":
                            xzxResult.request = strresponse[i].Split('=')[1];
                            break;
                        case "sign":
                            xzxResult.sign = strresponse[i].Split('=')[1];
                            break;
                    }
                }
                Log.Info($"XZX返回账户二维码信息：{JsonConvert.SerializeObject(xzxResult)}");
                if (xzxResult.errcode == "0")
                {
                    string DecodeResponse = HttpUtility.UrlDecode(xzxResult.request);
                    string strinfo = client.GetDecryptDES(DecodeResponse, des_key);//返回获得的数据
                    Log.Info($"XZX账户二维码请求返回request解析：{strinfo}");
                    JObject jo = (JObject)JsonConvert.DeserializeObject(strinfo);
                    OnecardBarcodeMap codeResult = new OnecardBarcodeMap
                    {
                        retcode = jo["barcode_get"]["retcode"].ToString(),
                        errmsg = jo["barcode_get"]["errmsg"].ToString(),
                        account = jo["barcode_get"]["account"].ToString(),
                        paytype = jo["barcode_get"]["paytype"].ToString(),
                        payacc = jo["barcode_get"]["payacc"].ToString(),
                        barcode = jo["barcode_get"]["barcode"].ToString(),
                        expires = jo["barcode_get"]["expires"].ToString()
                    };
                    onecardResult = codeResult;
                }
                else
                    onecardResult.errmsg = $"获取账户二维码失败，错误码[errcode]：{xzxResult.errcode}";
            }
            else
                onecardResult.errmsg = $"获取账户二维码失败，请求状态码：{response.Result.StatusCode}";
            Log.Info($"获取消费码访问令牌返回结果：{JsonConvert.SerializeObject(onecardResult)}");
            return onecardResult;
        }

        /// <summary>
        /// 餐卡支付结果通知
        /// </summary>
        /// <param name="Account">账号</param>
        /// <param name="QrCode">二维码字符串</param>
        /// <returns></returns>
        public static CardPaymentResultMap CardPaymentResult(string Account, string QrCode)
        {
            CardPaymentResultMap cardPaymentResult = new CardPaymentResultMap();
            switch (XzxConfig.ResultType)
            {
                case 1:
                    var QrCodeRecordInfo = SqlDBHelper.GetInstance(XzxConfig.DbConnect, XzxConfig.DbType).Queryable<QrCodeRecord>()
                        .Where(x => x.Account == Account && x.QrCode == QrCode).First();
                    if (QrCodeRecordInfo == null || string.IsNullOrWhiteSpace(QrCodeRecordInfo.QrCode))
                        return cardPaymentResult;
                    DateTime DateTimeStart = Convert.ToDateTime(QrCodeRecordInfo.DateTime);
                    DateTime DateTimeEnd = DateTimeStart.AddSeconds(60);//一分钟之内，二维码有效期为 60 秒
                    var QRCodeCacheList = MemoryCacheHelper.Get<List<QrCodeCacheMap>>($"{Account}_QRCodeCache");
                    var AccountNumber = (from m in QRCodeCacheList
                                         where m.Account == Account && m.QrCode == QrCode
                                         select m.FromJnNumber).FirstOrDefault();
                    if (AccountNumber != null) return cardPaymentResult;
                    else
                    {
                        var FromJnnuMberList = (from m in QRCodeCacheList
                                                where m.Account == Account
                                                select long.Parse(m.FromJnNumber)).ToList();
                        TransactionRecord TransactionInfo = new TransactionRecord();
                        if (FromJnnuMberList != null && FromJnnuMberList.Count > 0)
                        {
                            TransactionInfo = SqlDBHelper.GetInstance(XzxConfig.DbConnect, XzxConfig.DbType).Queryable<TransactionRecord>()
                                .Where(x => x.TranCode == "99" && x.FromAccount == long.Parse(Account) &&
                                x.JnDateTime >= DateTimeStart && x.JnDateTime <= DateTimeEnd &&
                                !FromJnnuMberList.Contains(x.FromJnNumber)).OrderBy(x => x.JnDateTime).First();
                        }
                        else
                        {
                            TransactionInfo = SqlDBHelper.GetInstance(XzxConfig.DbConnect, XzxConfig.DbType).Queryable<TransactionRecord>()
                               .Where(x => x.TranCode == "99" && x.FromAccount == long.Parse(Account) &&
                               x.JnDateTime >= DateTimeStart && x.JnDateTime <= DateTimeEnd).OrderBy(x => x.JnDateTime).First();
                        }
                        //待加入缓存的二维码信息
                        List<QrCodeCacheMap> QrCodeCacheList = new List<QrCodeCacheMap>();
                        //返回值
                        if (TransactionInfo != null && TransactionInfo.FromAccount > 0)
                        {
                            cardPaymentResult.IsDisCount = "1";
                            cardPaymentResult.TotalAmount = TransactionInfo.TraNamt.ToString();
                            cardPaymentResult.DisCountAmount = "0.00";
                            cardPaymentResult.AmounTafterDisCount = TransactionInfo.TraNamt.ToString();
                            //添加到缓存
                            QrCodeCacheMap qrCodeCache = new QrCodeCacheMap
                            {
                                Account = Account,
                                FromJnNumber = TransactionInfo.FromJnNumber.ToString(),
                                QrCode = QrCode,
                                CreateTime = DateTime.Now
                            };
                            QrCodeCacheList.Add(qrCodeCache);
                        }
                        //二维码信息缓存有效期 3 分钟
                        if (QrCodeCacheList != null && QrCodeCacheList.Count > 0)
                            MemoryCacheHelper.Set($"{Account}_QRCodeCache", QrCodeCacheList, new TimeSpan(0, 3, 0));
                    }
                    break;
                case 2:
                    string ReSume = $"二维码=[{QrCode}] {QrCode}";//刷卡结果备注
                    var TRJN_Info = SqlDBHelper.GetInstance(XzxConfig.DbConnect, XzxConfig.DbType).Queryable<TransactionRecord>()
                        .Where(x => x.FromAccount == long.Parse(Account) &&
                        (x.ReSume == ReSume || x.ReSume.Trim() == ReSume.Trim())).First();
                    if (TRJN_Info != null)
                    {
                        cardPaymentResult.IsDisCount = "1";
                        cardPaymentResult.TotalAmount = TRJN_Info.TraNamt.ToString();
                        cardPaymentResult.DisCountAmount = "0.00";
                        cardPaymentResult.AmounTafterDisCount = TRJN_Info.TraNamt.ToString();
                    }
                    break;
            }
            return cardPaymentResult;
        }
    }
}
