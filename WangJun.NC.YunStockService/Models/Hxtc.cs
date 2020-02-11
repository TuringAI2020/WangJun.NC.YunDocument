using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class Hxtc
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
