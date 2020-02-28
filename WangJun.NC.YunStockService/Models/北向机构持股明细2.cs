using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class 北向机构持股明细2
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int 持股日期tag { get; set; }
        public string JgCode { get; set; }
        public decimal 当日收盘价 { get; set; }
        public decimal 当日涨跌幅 { get; set; }
        public string 机构名称 { get; set; }
        public decimal 持股数量 { get; set; }
        public decimal 持股市值 { get; set; }
        public decimal 持股数量占a股百分比 { get; set; }
        public decimal 持股市值变化1日 { get; set; }
        public decimal 持股市值变化5日 { get; set; }
        public decimal 持股市值变化10日 { get; set; }
    }
}
