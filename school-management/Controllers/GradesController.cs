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
    public class GradesController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();

        // GET: Grades
        public ActionResult Index(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Home");

            Inscriptions inscription = db.Inscriptions.Find(id);

            if(inscription == null)
                return RedirectToAction("Index", "Home");

            ViewBag.inscriptionDetailed = getInscriptionDetail(inscription);


            return View(getGradesDetailed((int)id));
        }

        // GET: Grades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            return View(grades);
        }

        // GET: Grades/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Home");

            Inscriptions inscription = db.Inscriptions.Find((int)id);

            if (inscription == null)
                return RedirectToAction("Index", "Home");

            ViewBag.availableAssignments = getAvailableAssignments(inscription);
            ViewBag.inscriptionDetailed = getInscriptionDetail(inscription);

            Grades grade = new Grades();
            grade.idInscription = (int)id;

            return View(grade);
        }

        // POST: Grades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,grade,gradevalue,idAssignment,idInscription")] Grades grades)
        {
            if (ModelState.IsValid)
            {
                int courseValue = db.Assignments.Find(grades.idAssignment).coursevalue;

                grades.gradevalue = grades.grade * courseValue / 10;


                db.Grades.Add(grades);
                db.SaveChanges();

                updateInscription(grades);

                return RedirectToAction("Index", new { id = grades.idInscription});
            }

            ViewBag.availableAssignments = getAvailableAssignments(db.Inscriptions.Find(grades.idInscription));
            ViewBag.idInscription = grades.idInscription;
            ViewBag.inscriptionDetailed = getInscriptionDetail(db.Inscriptions.Find(grades.idInscription));

            return View(grades);
        }

        // GET: Grades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Inscriptions inscription = db.Inscriptions.Find(grades.idInscription);

            List<SelectListItem> selectListItems = getAvailableAssignments(inscription);
            Assignments assignment = db.Assignments.Find(grades.idAssignment);
            selectListItems.Add(new SelectListItem { Text = assignment.assignmentname + " | " + assignment.coursevalue + "%", Value = assignment.id.ToString() });

            ViewBag.availableAssignments = selectListItems;
            ViewBag.inscriptionDetailed = getInscriptionDetail(inscription);

            return View(grades);
        }

        // POST: Grades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,grade,gradevalue,idAssignment,idInscription")] Grades grades, string oldAssignmentId)
        {
            if (ModelState.IsValid)
            {
                int courseValue = db.Assignments.Find(grades.idAssignment).coursevalue;

                grades.gradevalue = grades.grade * courseValue / 10;

                db.Entry(grades).State = EntityState.Modified;
                db.SaveChanges();

                updateInscription(grades);
                return RedirectToAction("Index", new { id = grades.idInscription});
            }
            return View(grades);
        }

        // GET: Grades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grades grades = db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            return View(grades);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grades grades = db.Grades.Find(id);
            db.Grades.Remove(grades);
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

        public InscriptionDetail getInscriptionDetail(Inscriptions inscription) 
        {
            InscriptionDetail inscriptionDetailed = new InscriptionDetail();

            inscriptionDetailed.id = inscription.id;
            inscriptionDetailed.idCourse = inscription.idCourse;
            inscriptionDetailed.coursename = db.Courses.Find(inscription.idCourse).coursename;
            inscriptionDetailed.idStudent = inscription.idStudent;

            Students student = db.Students.Find(inscription.idStudent);
            inscriptionDetailed.studentname = student.studentname;
            inscriptionDetailed.studentcode = db.Users.Find(student.idUser).username;

            inscriptionDetailed.idTeacher = inscription.idTeacher;

            Teachers teacher = db.Teachers.Find(inscription.idTeacher);
            inscriptionDetailed.teachername = teacher.teachername;
            inscriptionDetailed.teachercode = db.Users.Find(teacher.idUser).username;

            inscriptionDetailed.inscriptionstatus = inscription.inscriptionstatus;
            inscriptionDetailed.progress = inscription.progress;
            inscriptionDetailed.avarage = inscription.avarage;
            inscriptionDetailed.generalgrade = inscription.generalgrade;

            return inscriptionDetailed;
        }

        public List<GradesDetail> getGradesDetailed(int idInscription) 
        {
            List<Grades> gradesByInscription = db.Grades.Where(g => g.idInscription.Equals(idInscription)).ToList();

            List<GradesDetail> gradeDetailedList = new List<GradesDetail>();

            GradesDetail item;

            foreach (Grades g in gradesByInscription) 
            {
                item = new GradesDetail();

                item.id = g.id;
                item.grade = g.grade;
                item.gradevalue = g.gradevalue;

                Assignments assignment = db.Assignments.Find(g.idAssignment);
                item.coursevalue = assignment.coursevalue;
                item.idAssignment = g.idAssignment;
                item.assignmentname = assignment.assignmentname;
                item.idInscription = g.idInscription;

                gradeDetailedList.Add(item);
            }

            return gradeDetailedList;
        }

        public List<SelectListItem> getAvailableAssignments(Inscriptions inscription) 
        {
            List<Assignments> allAssignments = db.Assignments.Where(a => a.idCourse.Equals(inscription.idCourse) && a.assignmentstatus.Equals("Active")).ToList();

            List<Grades> gradesByInscription = db.Grades.Where(g => g.idInscription.Equals(inscription.id)).ToList();

            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (Assignments a in allAssignments)
            {
                if (!gradesByInscription.Where(g => g.idAssignment.Equals(a.id)).Any())
                    selectList.Add(new SelectListItem { Text = a.assignmentname + " | " + a.coursevalue + "%" , Value = a.id.ToString() });
            }

            return selectList;
        }

        public void updateInscription(Grades grade) 
        {
            Inscriptions inscription = db.Inscriptions.Find(grade.idInscription);

            List<Grades> gradeList = db.Grades.Where(g => g.idInscription.Equals(inscription.id)).ToList();

            inscription.avarage = 0;

            inscription.generalgrade = 0;

            inscription.progress = 0;

            foreach (Grades g in gradeList) 
            {

                inscription.avarage += g.grade;
                inscription.progress += db.Assignments.Find(g.idAssignment).coursevalue;
                inscription.generalgrade += g.gradevalue;
            }

            inscription.avarage /= gradeList.Count();

            db.Entry(inscription).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
