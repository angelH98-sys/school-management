using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Semesters
    {
        public int id { get; set; }
        public int semesterstatus { get; set; }
        public decimal avarage { get; set; }
        public decimal progress { get; set; }
        public DateTime enrolleddate { get; set; }
        public int idStudent { get; set; }
    }
}