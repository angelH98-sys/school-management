using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class TeachersEnrolleds
    {
        public int id { get; set; }
        public string enrolledstatus { get; set; }
        public int idTeacher { get; set; }
        public int idCourse { get; set; }
    }

    [NotMapped]
    public class TeachersEnrolledsDetail : TeachersEnrolleds
    {
        public string teachername { get; set; }
        public string coursename { get; set; }
    }
}