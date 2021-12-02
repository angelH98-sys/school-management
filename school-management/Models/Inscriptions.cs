using System;
using System.Collections.Generic;
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
        public int idSemester { get; set; }
        public int idCourse { get; set; }
        public int idTeacher { get; set; }
    }
}