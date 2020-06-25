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
    
    public partial class ES_ExamPaper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ES_ExamPaper()
        {
            this.ES_ExamPaper_Exercise = new HashSet<ES_ExamPaper_Exercise>();
            this.ES_Paper_Tag = new HashSet<ES_Paper_Tag>();
            this.ES_User_ExamPaper = new HashSet<ES_User_ExamPaper>();
            this.ES_ExamLog = new HashSet<ES_ExamLog>();
        }
    
        public int EmPaperId { get; set; }
        public string EmPaperName { get; set; }
        public int EmPaperSelectNum { get; set; }
        public int EmPaperFillNum { get; set; }
        public int EmPaperJudgeNum { get; set; }
        public int EmPaperMultipleNum { get; set; }
        public int EmPaperScore { get; set; }
        public Nullable<double> EmPaperTrueScore { get; set; }
        public bool IsDel { get; set; }
        public int EmPaperSelectScore { get; set; }
        public int EmPaperFillScore { get; set; }
        public int EmPaperJudgeScore { get; set; }
        public int EmPaperMultipleScore { get; set; }
        public Nullable<int> EmPtId { get; set; }
        public string EmTagPercent { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ES_ExamPaper_Exercise> ES_ExamPaper_Exercise { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ES_Paper_Tag> ES_Paper_Tag { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ES_User_ExamPaper> ES_User_ExamPaper { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ES_ExamLog> ES_ExamLog { get; set; }
        public virtual ES_ExamPart ES_ExamPart { get; set; }
    }
}
