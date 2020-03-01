using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class ShortNews
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public DateTime PublishTime { get; set; }
        public string Content { get; set; }
        public string Href { get; set; }
    }
}
