using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.XzxModel.Entity
{
    /// <summary>
    /// 交易流水表
    /// </summary>
    [SugarTable("TRJN")]
    public class TransactionRecord
    {
        /// <summary>
        /// 金额
        /// </summary>
        [SugarColumn(ColumnName = "TRANAMT")]
        public long TraNamt { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [SugarColumn(ColumnName = "FROMJNNUMBER")]
        public long FromJnNumber { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [SugarColumn(ColumnName = "TRANCODE")]
        public string TranCode { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(ColumnName = "FROMACCOUNT")]
        public long FromAccount { get; set; }
        /// <summary>
        /// 到账时间
        /// </summary>
        [SugarColumn(ColumnName = "JNDATETIME")]
        public DateTime JnDateTime { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        [SugarColumn(ColumnName = "RESUME")]
        public string ReSume { get; set; }
    }
}
