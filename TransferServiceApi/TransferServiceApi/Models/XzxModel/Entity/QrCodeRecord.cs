using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.XzxModel.Entity
{
    /// <summary>
    /// 消费码中间表
    /// </summary>
    [SugarTable("QRCODERECORD")]
    public class QrCodeRecord
    {
        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(ColumnName = "ACCOUNT")]
        public string Account { get; set; }
        /// <summary>
        /// 二维码信息
        /// </summary>
        [SugarColumn(ColumnName = "QR_CODE")]
        public string QrCode { get; set; }
        /// <summary>
        /// 申请二维码时间
        /// </summary>
        [SugarColumn(ColumnName = "DATETIME")]
        public string DateTime { get; set; }
    }
}
