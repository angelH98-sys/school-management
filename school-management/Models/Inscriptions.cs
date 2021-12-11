using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Inscriptions
    {
        public int id { get; set; }
        public decimal generalgrade { get; set; }
        public string inscriptionstatus { get; set; }
        public int progress { get; set; }
        public decimal avarage { get; set; }
        public int idStudent { get; set; }
        public int idCourse { get; set; }
        public int idTeacher { get; set; }
    }

    [NotMapped]
    public class InscriptionDetail : Inscriptions
    { 
        public string studentname { get; set; }
        public string studentcode { get; set; }
        public string coursename { get; set; }
        public string teachername { get; set; }
        public string teachercode { get; set; }
    }
}