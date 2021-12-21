using school_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace school_management.Controllers
{
    public class HomeController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();
        public ActionResult Index()
        {

            studentsPerCourseYear();
            mostDemandedCourses();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void studentsPerCourseYear() 
        {

            string studentByCourseYear = "";

            for (int i = 1; i <= 9; i++)
            {
                studentByCourseYear += db.Students.Where(s => s.courseyear.Equals(i)).Count().ToString();
                if (i < 9)
                    studentByCourseYear += ",";
            }

            string courseYearNames = "\'1er año\',\'2do año\',\'3er año\',\'4to año\',\'5to año\',\'6to año\',\'7mo año\',\'8vo año\',\'9no año\'";

            ViewBag.studentByCourseYear = studentByCourseYear;
            ViewBag.courseYearNames = courseYearNames;
        }

        public void mostDemandedCourses() 
        {

            string coursesName = "";

            string inscriptionsByCourse = "";

            List<Courses> allCourses = db.Courses.ToList();

            Courses aux;

            for (int i = 0; i < allCourses.Count(); i++) 
            {

                aux = allCourses[i];
                coursesName += "'" + aux.coursename + "'";
                inscriptionsByCourse += db.Inscriptions.Where(ins => ins.idCourse.Equals(aux.id)).Count().ToString();

                if (i + 1 < allCourses.Count()) 
                {
                    coursesName += ",";
                    inscriptionsByCourse += ",";
                }
            }

            ViewBag.courseName = coursesName;
            ViewBag.inscriptionsByCourse = inscriptionsByCourse;
        }

    }
}