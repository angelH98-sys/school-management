using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace school_management.Models
{
    public class SchoolManagementDBContext : DbContext
    {
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Inscriptions> Inscriptions { get; set; }
        public DbSet<Semesters> Semesters { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<TeachersEnrolleds> TeachersEnrolleds { get; set; }
        public DbSet<Teachers> Teachers { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}