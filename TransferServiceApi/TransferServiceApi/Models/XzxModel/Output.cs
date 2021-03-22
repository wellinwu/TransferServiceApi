using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.XzxModel
{
    public class QrCodeCacheMap
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string FromJnNumber { get; set; }
        /// <summary>
        /// 二维码ID
        /// </summary>
        public string QrCode { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    public class CardPaymentResultMap
    {
        /// <summary>
        /// 是否有优惠活动
        /// </summary>
        public string IsDisCount { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public string TotalAmount { get; set; }
        /// <summary>
        /// 优惠金额
        /// </summary>
        public string DisCountAmount { get; set; }
        /// <summary>
        /// 优惠后金额
        /// </summary>
        public string AmounTafterDisCount { get; set; }
    }
}
