using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishTime { get; set; }
        public long DateTag { get; set; }
        public int Status { get; set; }
        public string DataSource { get; set; }
        public string Href { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string Tags { get; set; }
    }
}
