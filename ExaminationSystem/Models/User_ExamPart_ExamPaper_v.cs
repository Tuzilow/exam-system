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
    
    public partial class User_ExamPart_ExamPaper_v
    {
        public int 考生ID { get; set; }
        public string 账号 { get; set; }
        public string 密码 { get; set; }
        public string 姓名 { get; set; }
        public Nullable<System.DateTime> 开始时间 { get; set; }
        public Nullable<System.DateTime> 结束时间 { get; set; }
        public Nullable<int> 试卷ID { get; set; }
        public string 试卷标题 { get; set; }
        public Nullable<int> 选择题数量 { get; set; }
        public Nullable<int> 填空题数量 { get; set; }
        public Nullable<int> 判断题数量 { get; set; }
        public Nullable<int> 多选题数量 { get; set; }
        public Nullable<int> 总分值 { get; set; }
        public Nullable<int> 得分 { get; set; }
    }
}
