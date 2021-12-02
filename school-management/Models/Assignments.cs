using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace school_management.Models
{
    public class Assignments
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Remote(action: "assignmentNameIsAvailable", controller: "Assignments", AdditionalFields = "idCourse", ErrorMessage = "Nombre de asignación no disponible")]
        public string assignmentname { get; set; }

        [Remote(action: "enoughCourseValue", controller: "Assignments", AdditionalFields = "idCourse,id", ErrorMessage = "Valor no disponible")]
        public int coursevalue { get; set; }
        public string assignmentstatus { get; set; }
        public int idCourse { get; set; }
    }
}