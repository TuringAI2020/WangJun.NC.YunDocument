using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class 资金流向
    {
        public string Code { get; set; }
        public int 日期tag { get; set; }
        public string 日期 { get; set; }
        public decimal 收盘价 { get; set; }
        public decimal 涨跌幅 { get; set; }
        public decimal 主力净流入净额 { get; set; }
        public decimal 主力净流入净占比 { get; set; }
        public decimal 超大单净流入净额 { get; set; }
        public decimal 超大单净流入净占比 { get; set; }
        public decimal 大单净流入净额 { get; set; }
        public decimal 大单净流入净占比 { get; set; }
        public decimal 中单净流入净额 { get; set; }
        public decimal 中单净流入净占比 { get; set; }
        public decimal 小单净流入净额 { get; set; }
        public decimal 小单净流入净占比 { get; set; }
    }
}
