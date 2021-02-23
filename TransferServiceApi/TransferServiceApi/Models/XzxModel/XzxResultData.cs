using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.XzxModel
{
    public class XzxResultData
    {
        /// <summary>
        /// 0 表示成功
        /// -1 系统繁忙
        /// -2 系统内部错误
        /// 40001 不合法的数据报文
        /// 40002 不支持的 appkey
        /// 40003 不支持的签名算法
        /// 40004 签名校验失败
        /// 40005 系统授权过期
        /// 40006 API 授权过期
        /// 40007 接入系统授权过期
        /// 40008 系统未授权
        /// 40009 API 未授权
        /// 40010 接入系统未授权
        /// 40011 接入系统未启用
        /// 40012 不支持的 API
        /// 40013 API 未启用
        /// 40014 调用次数超限
        /// 40015 数据格式错误
        /// 40016 协议版本不支持
        /// 40017 access_token 无效
        /// 40018 时间戳无效
        /// </summary>
        public string errcode { get; set; }
        /// <summary>
        /// 对 API 调用参数（除 sign 外）进行签名获得
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string request { get; set; }
    }

    public class AuthorizeAccessTokenMap
    {
        /// <summary>
        /// 业务处理错误代码（0：成功 其他：错误）
        /// </summary>
        public string retcode { get; set; }
        /// <summary>
        /// 详细错误信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 表示访问令牌，获取访问令牌交易中返回，其他交易此字段为空
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 表示访问令牌的过期时间，单位为秒，必须在过期时间内重新获取访问令牌。获取访问令牌交易中不为空，其他交易此字段为空
        /// </summary>
        public int expires_in { get; set; }
    }

    public class OnecardBarcodeMap
    {
        /// <summary>
        /// 业务处理错误代码：0：成功 其他：错误
        /// </summary>
        public string retcode { get; set; }
        /// <summary>
        /// 详细错误信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 一卡通账户
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string paytype { get; set; }
        /// <summary>
        /// 支付账号
        /// </summary>
        public string payacc { get; set; }
        /// <summary>
        /// 条码数据：22 字节的数字串
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// 二维码超时时间（60秒）
        /// </summary>
        public string expires { get; set; }
    }
}
