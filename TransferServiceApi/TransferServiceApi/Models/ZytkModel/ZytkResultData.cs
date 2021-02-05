using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.ZytkModel
{
    public class ZytkResultData<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 出错信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }
    }

    public class AccessTokenMap
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public string expires_in { get; set; }
    }

    public class AccfixidData
    {
        /// <summary>
        /// 卡户信息当前版本号
        /// </summary>
        public string accFixId { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
    }

    public class CommonQRcodeData
    {
        /// <summary>
        /// 二维码字符串
        /// </summary>
        public string qrCode { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
    }
}
