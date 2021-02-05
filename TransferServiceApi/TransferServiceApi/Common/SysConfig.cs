using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Common
{
    /// <summary>
    /// 配置文件信息
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// 配置文件参数
        /// </summary>
        public static AppConfigModel AppConfig { get; set; }
    }

    public class AppConfigModel
    {
        /// <summary>
        /// 是否开启Swagger，true-仅用于开发环境调式，false-正式环境保护接口,swagger页面一定要关闭
        /// </summary>
        public static bool OpenSwagger { get; set; }
        /// <summary>
        /// 正元一卡通配置
        /// </summary>
        public static ZytkConfig ZytkConfig { get; set; }
        /// <summary>
        /// 新中新配置
        /// </summary>
        public static XzxConfig XzxConfig { get; set; }
    }

    public class ZytkConfig
    {
        /// <summary>
        /// 正元一卡通4.0统一Api接口地址
        /// </summary>
        public static string ApiUrl { get; set; }
        /// <summary>
        /// 第三方用户唯一凭证
        /// </summary>
        public static string AppId { get; set; }
        /// <summary>
        /// 第三方用户唯一凭证密钥
        /// </summary>
        public static string AppSecret { get; set; }
    }

    public class XzxConfig
    {
        /// <summary>
        /// 消费码Key
        /// </summary>
        public static QRCodeKey QRCodeKey { get; set; }
        /// <summary>
        /// 无卡扣款、退款Key
        /// </summary>
        public static ConsumeKey ConsumeKey { get; set; }
        /// <summary>
        /// TSM后台访问地址
        /// </summary>
        public static string TsmUrl { get; set; }
        /// <summary>
        /// 新中新连接数据库地址
        /// </summary>
        public static string DbConnect { get; set; }
        /// <summary>
        /// 连接数据库类型（0-MySql 1-SqlServer 3-Oracle）
        /// </summary>
        public static int DbType { get; set; }
        /// <summary>
        /// 申请二维码电子钱包的类型
        /// </summary>
        public static string Ewallet { get; set; }
        /// <summary>
        /// 是否保存二维码串到数据库，false：不保存 true：保存
        /// （当二维码消费结果通知类型[ResultType]是1时，必须要保存二维码串信息，IsSave设置为true）
        /// </summary>
        public static bool IsSaveQRcode { get; set; }
        /// <summary>
        /// 二维码消费结果通知类型：
        /// 1-以二维码消费时间来取二维码消费结果（这个模式需要POS机和TSM服务的时间一致，最好以网络时间同步）；
        /// 2-以二维码字符串信息来取二维码消费结果
        /// </summary>
        public static int ResultType { get; set; }
    }

    public class QRCodeKey
    {
        /// <summary>
        /// 消费码加密解密deskey
        /// </summary>
        public static string CodeDesKey { get; set; }
        /// <summary>
        /// 消费码提供的appkey
        /// </summary>
        public static string CodeAppKey { get; set; }
    }

    public class ConsumeKey
    {
        /// <summary>
        /// 无卡扣款、退款加密解密deskey
        /// </summary>
        public static string ConsumeDesKey { get; set; }
        /// <summary>
        /// 无卡扣款、退款提供的appkey
        /// </summary>
        public static string ConsumeAppKey { get; set; }
    }
}
