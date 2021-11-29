using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace school_management.Models
{
    public class Courses
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Remote(action: "courseNameIsAvailable", controller: "Courses", AdditionalFields = "id", ErrorMessage = "Nombre de curso no disponible")]
        public string coursename { get; set; }
        public string coursestatus { get; set; }
        public int courseyear { get; set; }
    }
}