using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Grades
    {
        public int id { get; set; }
        public decimal grade { get; set; }
        public decimal gradevalue { get; set; }
        public int idAssignment { get; set; }
        public int idInscription { get; set; }
    }
}