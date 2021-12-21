using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using school_management.Models;

namespace school_management.Controllers
{
    public class AssignmentsController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();

        // GET: Assignments
        public ActionResult Index(int? id)
        {
            ViewBag.id = id;
            int availablepercentage = 100;
            if (id != null)
            {

                List<Assignments> assignmentByCourse = db.Assignments.Where(a => a.idCourse == id).ToList();

                foreach (Assignments a in assignmentByCourse) 
                {
                    availablepercentage -= a.coursevalue;
                }

                ViewBag.availablepercentage = availablepercentage;
                return PartialView(assignmentByCourse);
            }
            else
            {

                ViewBag.availablepercentage = availablepercentage;
                return PartialView(new List<Assignments>());
            }
        }

        // GET: Assignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignments assignments = db.Assignments.Find(id);
            if (assignments == null)
            {
                return HttpNotFound();
            }
            return View(assignments);
        }

        // GET: Assignments/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Courses");

            ViewBag.coursename = db.Courses.Find(id).coursename;

            Assignments assignment = new Assignments();
            assignment.idCourse = (int) id;

            return View(assignment);
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,assignmentname,coursevalue,assignmentstatus,idCourse")] Assignments assignments)
        {
            if (ModelState.IsValid)
            {
                assignments.assignmentname = assignments.assignmentname.ToUpper();
                assignments.assignmentstatus = "Active";
                db.Assignments.Add(assignments);
                    db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = assignments.idCourse});
            }

            return View(assignments);
        }

        // GET: Assignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Courses");
            }
            Assignments assignments = db.Assignments.Find(id);
            if (assignments == null)
            {
                return RedirectToAction("Index", "Courses");
            }
            return View(assignments);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,assignmentname,coursevalue,assignmentstatus,idCourse")] Assignments assignments, string status)
        {
            if (ModelState.IsValid)
            {
                assignments.assignmentname = assignments.assignmentname.ToUpper();
                assignments.assignmentstatus = status;
                db.Entry(assignments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = assignments.idCourse});
            }
            return View(assignments);
        }

        // GET: Assignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignments assignments = db.Assignments.Find(id);
            if (assignments == null)
            {
                return HttpNotFound();
            }
            return View(assignments);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignments assignments = db.Assignments.Find(id);
            db.Assignments.Remove(assignments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [AcceptVerbs("GET", "POST")]
        public JsonResult assignmentNameIsAvailable(string coursename, int idCourse)
        {
            Boolean nameIsAvailable;

            nameIsAvailable = !db.Assignments.Any(a => a.idCourse == idCourse && a.assignmentname.Equals(coursename));
            
            return Json(nameIsAvailable, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs("GET", "POST")]
        public JsonResult enoughCourseValue(int coursevalue, int idCourse, int? id)
        {

            List<Assignments> assignmentByCourse = db.Assignments.Where(a => a.idCourse.Equals(idCourse) && a.assignmentstatus.Equals("Active")).ToList();

            decimal currentValue = 0;

            foreach (Assignments a in assignmentByCourse) {
                if(id == null | a.id != id)
                    currentValue += a.coursevalue;
            }

            bool isEnoughCourseValue = (100 - currentValue) >= coursevalue;

            return Json(isEnoughCourseValue, JsonRequestBehavior.AllowGet);
        }
    }
}
