using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.XzxModel
{
    public class AccessTokenParam
    {
        /// <summary>
        /// 令牌类型：1-消费码访问令牌 2-无卡扣款、退款访问令牌
        /// </summary>
        public int TokenType { get; set; }
    }

    public class OnecardBarcodeParam
    {
        /// <summary>
        /// 一卡通账户
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 支付方式：
        /// 1、 校园卡账户支付
        /// 2、 绑定银行卡支付
        /// 3、 自定义银行卡支付
        /// </summary>
        public string paytype { get; set; }
        /// <summary>
        /// 支付账号：
        /// paytype 为 1 时此字段，值可为：
        /// ###：为卡账户；其他为电子账户类型
        /// paytype 为 2 时此字段，值可为：空
        /// paytype 为 3 时此字段，值可为：银行卡号
        /// </summary>
        public string payacc { get; set; }
    }
}
