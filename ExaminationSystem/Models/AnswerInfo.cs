using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExaminationSystem.Models
{
    public class AnswerInfo
    {
        public int EsId { get; set; }

        public List<string> Ans { get; set; }
    }
}