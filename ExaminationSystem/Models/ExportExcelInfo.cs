using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExaminationSystem.Models
{
    public class ExportExcelInfo
    {
        public string UserName { get; set; }
        public int LogId { get; set; }
        public bool IsSubmit { get; set; }
        public double Score { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Part { get; set; }
    }
}
