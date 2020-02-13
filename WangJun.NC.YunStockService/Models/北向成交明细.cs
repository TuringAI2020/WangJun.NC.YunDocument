using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class 北向成交明细
    {
        public string Code { get; set; }
        public int 日期tag { get; set; }
        public int 当日排名 { get; set; }
        public decimal 收盘价 { get; set; }
        public decimal 涨跌幅 { get; set; }
        public decimal 沪深股通净买额 { get; set; }
        public decimal 沪深股通买入金额 { get; set; }
        public decimal 沪深股通卖出金额 { get; set; }
        public decimal 沪深股通成交金额 { get; set; }
    }
}
