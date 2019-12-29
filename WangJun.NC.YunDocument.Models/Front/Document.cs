using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument.Front
{
    public partial class Document:Item
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Status { get; set; }
        public int Sort { get; set; }
        public Guid CreatorId { get; set; }
        public Guid EditorId { get; set; }

        [Keyword]
        public string DataSource { get; set; }

        [Keyword]
        public string DataSourceName { get; set; }

        [Keyword]
        public string Title { get; set; }

        [Keyword]
        public string Summary { get; set; }

        [Keyword]
        [NotListItem]
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
