using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Assignments
    {
        public int id { get; set; }
        public string assignmentname { get; set; }
        public decimal coursevalue { get; set; }
        public string assignmentstatus { get; set; }
        public int idCourse { get; set; }
    }
}