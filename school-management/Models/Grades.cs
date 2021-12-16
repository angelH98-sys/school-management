using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

    [NotMapped]
    public class GradesDetail : Grades
    {
        public string assignmentname { get; set; }
        public int coursevalue { get; set; }
    }
}