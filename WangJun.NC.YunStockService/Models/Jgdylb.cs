using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class Jgdylb
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int 接待机构数量 { get; set; }
        public string 接待方式 { get; set; }
        public string 接待人员 { get; set; }
        public string 接待地点 { get; set; }
        public string 接待日期 { get; set; }
        public string 公告日期 { get; set; }
        public int 公告日期tag { get; set; }
        public int 接待日期tag { get; set; }
    }
}
