using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class TeacherEnrolled
    {
        public int id { get; set; }
        public string enrolledstatus { get; set; }
        public int idTeacher { get; set; }
        public int idCourse { get; set; }
    }
}