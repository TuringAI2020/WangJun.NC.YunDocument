using System;
using System.Collections.Generic;
using System.Linq;

namespace WangJun.NC.YunDocument.Models
{
    public partial class Document
    {
        public bool IsValid
        {
            get
            {
                var properties = this.GetType().GetProperties().ToList();
                return true;
            }
        }
    }
}
