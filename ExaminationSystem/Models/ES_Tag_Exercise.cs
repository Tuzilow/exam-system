//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExaminationSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ES_Tag_Exercise
    {
        public int TgEsId { get; set; }
        public Nullable<int> TagId { get; set; }
        public Nullable<int> EsId { get; set; }
        public bool IsDel { get; set; }
    
        public virtual ES_Exercise ES_Exercise { get; set; }
        public virtual ES_Tag ES_Tag { get; set; }
    }
}
