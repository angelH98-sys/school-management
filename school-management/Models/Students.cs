using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Students
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string studentname { get; set; }
        public int courseyear { get; set; }
        public int idUser { get; set; }
    }

    [NotMapped]
    public class StudentsDetail : Students
    { 
        public string username { get; set; }
        public string userstatus { get; set; }
    }
}