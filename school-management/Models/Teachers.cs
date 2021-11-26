using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Teachers
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public string teachername { get; set; }
        public int idUser { get; set; }
    }

    [NotMapped]
    public class TeachersDetail : Teachers
    {

        public string mail { get; set; }
        public string status { get; set; }
        public string code { get; set; }
    }
}