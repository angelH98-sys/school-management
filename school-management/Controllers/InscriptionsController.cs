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
    public class InscriptionsController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();

        // GET: Inscriptions
        public ActionResult Index()
        {
            return View(db.Inscriptions.ToList());
        }

        public ActionResult IndexByStudent(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Students");

            ViewBag.idStudent = (int)id;

            List<Inscriptions> inscriptionByStudent = db.Inscriptions.Where(i => i.idStudent.Equals((int)id)).ToList();

            List<InscriptionDetail> inscriptionDetailList = new List<InscriptionDetail>();

            InscriptionDetail item;

            foreach (Inscriptions i in inscriptionByStudent) 
            {

                item = new InscriptionDetail();

                item.idTeacher = i.idTeacher;

                if (inscriptionDetailList.Where(idl => idl.idTeacher.Equals(i.idTeacher)).Any())
                {
                    item.teachername = inscriptionDetailList.Where(idl => idl.idTeacher.Equals(i.idTeacher)).First().teachername;
                }
                else 
                {
                    item.teachername = db.Teachers.Find(i.idTeacher).teachername;
                }

                item.idCourse = i.idCourse;

                if (inscriptionDetailList.Where(idl => idl.idCourse.Equals(i.idCourse)).Any())
                {
                    item.coursename = inscriptionDetailList.Where(idl => idl.idCourse.Equals(i.idCourse)).First().coursename;
                }
                else
                {
                    item.coursename = db.Courses.Find(i.idCourse).coursename;
                }

                item.idStudent = (int)id;
                item.avarage = i.avarage;
                item.progress = i.progress;
                item.inscriptionstatus = i.inscriptionstatus;
                item.generalgrade = i.generalgrade;
                item.id = i.id;

                inscriptionDetailList.Add(item);
            }

            ViewBag.inscriptionDetailList = inscriptionDetailList;
            return PartialView();
        }

        // GET: Inscriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Inscriptions inscriptions = db.Inscriptions.Find(id);
            if (inscriptions == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.studentname = db.Students.Find(inscriptions.idStudent).studentname;
            ViewBag.coursename = db.Courses.Find(inscriptions.idCourse).coursename;
            ViewBag.teachername = db.Teachers.Find(inscriptions.idTeacher).teachername;

            return View(inscriptions);
        }

        // GET: Inscriptions/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult CreateByStudent(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Students");

            ViewBag.selectList = getCoursesAvailable((int)id);
            ViewBag.studentname = db.Students.Find((int)id).studentname;
            ViewBag.idStudent = id;

            return View();
        }


        // POST: Inscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,generalgrade,inscriptionstatus,progress,avarage,idSemester,idCourse,idTeacher")] Inscriptions inscriptions)
        {
            if (ModelState.IsValid)
            {
                db.Inscriptions.Add(inscriptions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inscriptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateByStudent([Bind(Include = "id,generalgrade,inscriptionstatus,progress,avarage,idSemester,idCourse,idTeacher")] Inscriptions inscriptions, int idStudent)
        {

            if (ModelState.IsValid)
            {
                inscriptions.id = 0;
                inscriptions.idStudent = idStudent;
                inscriptions.inscriptionstatus = "Active";
                inscriptions.progress = 0;
                inscriptions.avarage = 0.00M;
                inscriptions.generalgrade = 0.00M;

                List<TeachersEnrolleds> teacherEnrolled = db.TeachersEnrolleds.Where(te => te.idCourse.Equals(inscriptions.idCourse) && te.enrolledstatus.Equals("Active")).ToList();

                int inscriptionMin = 0, aux = 0;

                foreach (TeachersEnrolleds te in teacherEnrolled)
                {
                    aux = db.Inscriptions.Where(i => i.idTeacher.Equals(te.idTeacher) && i.inscriptionstatus.Equals("Active")).Count();

                    if (inscriptionMin == 0 | inscriptionMin < aux)
                    {
                        inscriptionMin = aux;
                        inscriptions.idTeacher = te.idTeacher;
                    }
                }

                db.Inscriptions.Add(inscriptions);
                db.SaveChanges();

                return RedirectToAction("Details", "Students", new { id = inscriptions.idStudent });
            }

            ViewBag.selectList = getCoursesAvailable(idStudent);

            return View(inscriptions);
        }

        // GET: Inscriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscriptions inscriptions = db.Inscriptions.Find(id);
            if (inscriptions == null)
            {
                return HttpNotFound();
            }
            return View(inscriptions);
        }
        
        public ActionResult EditByStudent(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Students");
            }

            Inscriptions inscriptions = db.Inscriptions.Find(id);
            if (inscriptions == null)
            {
                return RedirectToAction("Index", "Students");
            }

            ViewBag.teacherAvailable = getTeachersAvailable(inscriptions.idCourse);
            ViewBag.studentname = db.Students.Find(inscriptions.idStudent).studentname;
            ViewBag.coursename = db.Courses.Find(inscriptions.idCourse).coursename;

            return View(inscriptions);
        }

        // POST: Inscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,generalgrade,inscriptionstatus,progress,avarage,idSemester,idCourse,idTeacher")] Inscriptions inscriptions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inscriptions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inscriptions);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditByStudent([Bind(Include = "id,generalgrade,inscriptionstatus,progress,avarage,idStudent,idCourse,idTeacher")] Inscriptions inscriptions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inscriptions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Students", new { id = inscriptions.idStudent});
            }
            return View(inscriptions);
        }

        // GET: Inscriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inscriptions inscriptions = db.Inscriptions.Find(id);
            if (inscriptions == null)
            {
                return HttpNotFound();
            }
            return View(inscriptions);
        }

        // POST: Inscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inscriptions inscriptions = db.Inscriptions.Find(id);
            db.Inscriptions.Remove(inscriptions);
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

        public List<SelectListItem> getCoursesAvailable(int idStudent) 
        {

            List<SelectListItem> coursesAvailable = new List<SelectListItem>();

            int courseyear = db.Students.Find(idStudent).courseyear;

            foreach (Courses c in db.Courses.Where(co => co.coursestatus.Equals("Active") && co.courseyear.Equals(courseyear)).ToList()) 
            {
                if (db.TeachersEnrolleds.Where(te => te.idCourse.Equals(c.id) && te.enrolledstatus.Equals("Active")).Count() > 0) 
                {
                    coursesAvailable.Add(new SelectListItem { Text = c.coursename, Value = c.id.ToString() });
                }
            }

            return coursesAvailable;
        }

        public List<SelectListItem> getTeachersAvailable(int idCourse) 
        {

            List<SelectListItem> teacherList = new List<SelectListItem>();

            Teachers teacher;

            foreach (TeachersEnrolleds te in db.TeachersEnrolleds.Where(t => t.idCourse.Equals(idCourse) && t.enrolledstatus.Equals("Active")).ToList()) 
            {
                teacher = db.Teachers.Find(te.idTeacher);

                teacherList.Add(new SelectListItem { Text = teacher.teachername, Value = teacher.id.ToString() });
            }

            return teacherList;
        }
    }
}
