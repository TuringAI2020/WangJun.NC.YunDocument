using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class 融资融券
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int 交易日期tag { get; set; }
        public string 交易日期 { get; set; }
        public decimal 收盘价 { get; set; }
        public decimal 涨跌幅 { get; set; }
        public decimal 融资余额 { get; set; }
        public decimal 融资余额占流通市值比 { get; set; }
        public decimal 融资买入额 { get; set; }
        public decimal 融资偿还额 { get; set; }
        public decimal 融资净买入 { get; set; }
        public decimal 融券余额 { get; set; }
        public decimal 融券余量 { get; set; }
        public decimal 融券卖出量 { get; set; }
        public decimal 融券偿还量 { get; set; }
        public decimal 融券净卖出 { get; set; }
        public decimal 融资融券余额 { get; set; }
        public decimal 融资融券余额差值 { get; set; }
    }
}
