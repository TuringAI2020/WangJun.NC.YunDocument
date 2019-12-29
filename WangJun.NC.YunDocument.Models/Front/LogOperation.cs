using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument.Front
{
    public partial class LogOperation: Item
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? CreateTime { get; set; }
        public Guid? UserId { get; set; }

        [Editable(true)]
        public string UserName { get; set; }
        public Guid? TargetId { get; set; }
        public int? TargetType { get; set; }
        public string TargetTypeName { get; set; }
        public int? BehaviorType { get; set; }
        public string BehaviorTypeName { get; set; }
        public int? ResultStatus { get; set; }
        public string ResultStatusName { get; set; }
        public string Exception { get; set; }

        [Editable(true)]
        public string Remark { get; set; }
        public Guid AppId { get; set; }
    }
}
