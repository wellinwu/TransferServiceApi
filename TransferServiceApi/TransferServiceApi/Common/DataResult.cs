using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Common
{
    public class DataResult
    {
        /// <summary>
        /// 标志（0-失败 1-成功 -99 系统错误）
        /// </summary>
        public string BS { get; set; }
        /// <summary>
        /// 出错信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 信息总数量
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 结果返回值
        /// </summary>
        public object Rows { get; set; }
    }
}
