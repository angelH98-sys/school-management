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
    public class TeachersEnrolledsController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();

        // GET: TeachersEnrolleds
        public ActionResult Index()
        {
            return View(db.TeachersEnrolleds.ToList());
        }

        public ActionResult IndexByTeacher(int? id) 
        {
            if (id != null)
            {
                List<TeachersEnrolleds> teacherEnrolledList = db.TeachersEnrolleds.Where(t => t.idTeacher.Equals((int) id)).ToList();
                List<TeachersEnrolledsDetail> teacherEnrolledDetailList = new List<TeachersEnrolledsDetail>();
                TeachersEnrolledsDetail te;


                foreach (TeachersEnrolleds item in teacherEnrolledList)
                {
                    te = new TeachersEnrolledsDetail();
                    te.id = item.id;
                    te.idTeacher = item.idTeacher;
                    te.idCourse = item.idCourse;
                    te.enrolledstatus = item.enrolledstatus;

                    te.coursename = db.Teachers.Find(te.idTeacher).teachername;
                    te.coursename = db.Courses.Find(te.idCourse).coursename;

                    teacherEnrolledDetailList.Add(te);
                }

                ViewBag.teacherEnrolledList = teacherEnrolledDetailList;
                return PartialView();
            }
            else 
            {
                return RedirectToAction("Index", "Teachers");
            }
        }

        // GET: TeachersEnrolleds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersEnrolleds teachersEnrolleds = db.TeachersEnrolleds.Find(id);
            if (teachersEnrolleds == null)
            {
                return HttpNotFound();
            }
            return View(teachersEnrolleds);
        }

        // GET: TeachersEnrolleds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TeachersEnrolleds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,enrolledstatus,idTeacher,idCourse")] TeachersEnrolleds teachersEnrolleds)
        {
            if (ModelState.IsValid)
            {
                db.TeachersEnrolleds.Add(teachersEnrolleds);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teachersEnrolleds);
        }

        // GET: TeachersEnrolleds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersEnrolleds teachersEnrolleds = db.TeachersEnrolleds.Find(id);
            if (teachersEnrolleds == null)
            {
                return HttpNotFound();
            }
            return View(teachersEnrolleds);
        }

        // POST: TeachersEnrolleds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,enrolledstatus,idTeacher,idCourse")] TeachersEnrolleds teachersEnrolleds)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teachersEnrolleds).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teachersEnrolleds);
        }

        // GET: TeachersEnrolleds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeachersEnrolleds teachersEnrolleds = db.TeachersEnrolleds.Find(id);
            if (teachersEnrolleds == null)
            {
                return HttpNotFound();
            }
            return View(teachersEnrolleds);
        }

        // POST: TeachersEnrolleds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeachersEnrolleds teachersEnrolleds = db.TeachersEnrolleds.Find(id);
            db.TeachersEnrolleds.Remove(teachersEnrolleds);
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
    }
}
