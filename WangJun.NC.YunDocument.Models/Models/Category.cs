using System;
using System.Collections.Generic;

namespace WangJun.NC.YunDocument.Models
{
    public partial class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ParentId { get; set; }
        public Guid RootId { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int Depth { get; set; }
        public int FileCountNext { get; set; }
        public int FolderCountNext { get; set; }
        public int FileCountSubTotal { get; set; }
        public int FolderCountSubTotal { get; set; }
        public long Size { get; set; }
        public string PathIdarray { get; set; }
        public string PathNameArray { get; set; }
        public Guid? AppId { get; set; }
    }
}
