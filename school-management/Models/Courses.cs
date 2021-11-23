using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Courses
    {
        public int id { get; set; }
        public string coursename { get; set; }
        public string coursestatus { get; set; }
        public int courseyear { get; set; }
    }
}