using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class Users
    {
        public int id { get; set; }
        public string username { get; set; }
        public string psswd { get; set; }
        public string usertype { get; set; }
        public string userstatus { get; set; }
        public string mail { get; set; }
    }
}