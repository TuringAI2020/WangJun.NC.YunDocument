using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class 北向持股统计
    {
        public int 持股日期tag { get; set; }
        public string 机构名称 { get; set; }
        public string 持股日期 { get; set; }
        public int 持股只数 { get; set; }
        public decimal 持股市值 { get; set; }
        public decimal 持股市值变化1日 { get; set; }
        public decimal 持股市值变化5日 { get; set; }
        public decimal 持股市值变化10日 { get; set; }
    }
}
