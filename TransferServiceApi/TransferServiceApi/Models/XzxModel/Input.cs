using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferServiceApi.Models.XzxModel
{
    public class AccessTokenMap
    {
        /// <summary>
        /// 令牌类型：1-消费码访问令牌 2-无卡扣款、退款访问令牌
        /// </summary>
        public int TokenType { get; set; }
    }
}
