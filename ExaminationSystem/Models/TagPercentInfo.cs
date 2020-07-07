using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExaminationSystem.Models
{
    public class TagPercentInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Percent { get; set; }

        public bool IsMust { get; set; }
    }
}