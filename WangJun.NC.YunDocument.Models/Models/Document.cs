using System;
using System.Collections.Generic;

namespace WangJun.NC.YunDocument.Models
{
    public partial class Document
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Status { get; set; }
        public int Sort { get; set; }
        public Guid CreatorId { get; set; }
        public Guid EditorId { get; set; }
        public string DataSource { get; set; }
        public string DataSourceName { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
        public Guid GroupId { get; set; }
        public int ReadCount { get; set; }
        public int FavCount { get; set; }
        public int LikeCount { get; set; }
        public Guid TagGroupId { get; set; }
        public string ImageUrl { get; set; }
        public int? ShowType { get; set; }
        public Guid AppId { get; set; }
        public long Version { get; set; }
        public Guid? FirstVersionId { get; set; }
    }
}
