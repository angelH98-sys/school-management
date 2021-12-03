using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using school_management.Models;

namespace school_management.Controllers
{
    public class TeachersController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();

        // GET: Teachers
        public ActionResult Index()
        {
            List<Teachers> teacherList = db.Teachers.ToList();
            List<TeachersDetail> teacherDetailList = new List<TeachersDetail>();

            TeachersDetail td;
            Users user;

            foreach (Teachers t in teacherList) {

                td = new TeachersDetail();
                td.id = t.id;
                td.idUser = t.idUser;
                td.teachername = t.teachername;

                user = db.Users.Find(t.idUser);
                td.mail = user.mail;
                td.code = user.username;
                td.status = user.userstatus;

                teacherDetailList.Add(td);
            }

            ViewBag.teacherDetailList = teacherDetailList;
            return View();
        }

        // GET: Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Teachers teachers = db.Teachers.Find(id);
            Users user = db.Users.Find(teachers.idUser);
            if (teachers == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.user = user;
            return View(teachers);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            ViewBag.courseList = db.Courses.Where(c => c.coursestatus.Equals("Active")).ToList();
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,teachername,idUser")] Teachers teachers, string mail, int? courseId)
        {

            try
            {

                MailAddress m = new MailAddress(mail);

                if (db.Users.Where(u => u.mail.Equals(mail)).Where(u => u.userstatus.Equals("Active")).Count() == 0) {

                    Users user = new Users();
                    user.mail = mail;
                    user.psswd = "";
                    user.username = teacherCodeMaker(teachers.teachername);
                    user.usertype = "t";
                    user.userstatus = "Active";
                    db.Users.Add(user);
                    db.SaveChanges();

                    teachers.idUser = user.id;
                    teachers.teachername = teachers.teachername.ToUpper();
                    db.Teachers.Add(teachers);
                    db.SaveChanges();

                    if (courseId != null) {
                        TeachersEnrolleds te = new TeachersEnrolleds();
                        te.idCourse = (int) courseId;
                        te.idTeacher = teachers.id;
                        te.enrolledstatus = "Active";

                        db.TeachersEnrolleds.Add(te);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Details", new { id = teachers.id });
                }

                ViewBag.courseList = db.Courses.Where(c => c.coursestatus.Equals("Active")).ToList();
                ViewBag.mailValidation = "Correo electrónico no disponible";
                return View(teachers);
            }
            catch (FormatException e)
            {
                ViewBag.courseList = db.Courses.Where(c => c.coursestatus.Equals("Active")).ToList();
                ViewBag.mailValidation = "Formato de correo erróneo";
                return View(teachers);
            }
            catch (Exception e) {

                ViewBag.courseList = db.Courses.Where(c => c.coursestatus.Equals("Active")).ToList();
                return View(teachers);
            }
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");

            }
            Teachers teachers = db.Teachers.Find(id);
            Users user = db.Users.Find(teachers.idUser);
            if (teachers == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.mail = user.mail;
            ViewBag.status = user.userstatus;
            return View(teachers);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,teachername,idUser")] Teachers teachers, string mail, string status)
        {
            ViewBag.mail = mail;
            ViewBag.status = status;
            try
            {
                
                MailAddress m = new MailAddress(mail);

                Users user = db.Users.Find(teachers.idUser);
                bool isUniqueMail = db.Users.Where(u => u.mail.Equals(mail))
                                            .Where(u => u.userstatus.Equals("Active"))
                                            .Where(u => u.id != teachers.idUser)
                                            .Count() == 0;

                if (isUniqueMail) {

                    user.mail = mail;
                    user.userstatus = status;
                    db.Entry(user).State = EntityState.Modified;
                    teachers.teachername = teachers.teachername.ToUpper();
                    db.Entry(teachers).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", "Teachers", new { id = teachers.id });
                }

                ViewBag.mailValidation = "Correo electrónico no disponible";
                return View(teachers);
            }
            catch (FormatException e)
            {
                ViewBag.mailValidation = "Formato de correo erróneo";
                return View(teachers);
            }
            catch (Exception e) {
                return View(teachers);
            }
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teachers teachers = db.Teachers.Find(id);
            if (teachers == null)
            {
                return HttpNotFound();
            }
            return View(teachers);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teachers teachers = db.Teachers.Find(id);
            db.Teachers.Remove(teachers);
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

        public string teacherCodeMaker(string name)
        {
            //This method return an teacher id, made with last name initials
            //Respect this format: IIYYRRRRR
            //Where:    I: Capital last name initials
            //          Y: Year of creation, 21 for example
            //          R: Random number between 1000 and 9999

            string code = "";
            string[] initials = name.Split(Char.Parse(" "));

            for (int i = 0; i <= initials.Length - 1; i++)
            {
                initials[i] = initials[i].Substring(0, 1).ToUpper();
            }

            code += initials[initials.Length - 2];
            code += initials[initials.Length - 1];

            code += DateTime.Now.Year.ToString().Substring(2, 2);

            Random random = new Random();
            code += random.Next(1000, 9999).ToString();

            return code;
        }
    }
}
