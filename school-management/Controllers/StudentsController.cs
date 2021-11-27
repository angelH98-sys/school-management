using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using school_management.Models;

namespace school_management.Controllers
{
    public class StudentsController : Controller
    {
        private SchoolManagementDBContext db = new SchoolManagementDBContext();

        // GET: Students
        public ActionResult Index()
        {
            List<Students> studentList = db.Students.ToList();
            List<StudentsDetail> studentDetailedList = new List<StudentsDetail>();

            StudentsDetail sd;
            foreach (Students s in studentList) {

                sd = new StudentsDetail();

                sd.courseyear = s.courseyear;
                sd.id = s.id;
                sd.idUser = s.idUser;
                sd.studentname = s.studentname;

                Users user = db.Users.Where(u => u.id.Equals(s.idUser)).First();
                sd.username = user.username;
                if (user.userstatus.Equals("Active")) {

                    sd.userstatus = "Activo";
                }
                else {

                    sd.userstatus = "Inactivo";
                }

                studentDetailedList.Add(sd);
            }

            ViewBag.studentDetailedList = studentDetailedList;
            return View();
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Students students = db.Students.Find(id);
            Users user = db.Users.Find(students.idUser);
            if (students == null | user == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.user = user;
            return View(students);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,studentname,courseyear,idUser")] Students students, string mail)
        {
            try
            {
                //First, should create students user and then, student
                MailAddress m = new MailAddress(mail);
                if (db.Users.Where(u => u.mail.Equals(mail)).Where(u => u.userstatus.Equals("Active")).Count() == 0)
                {

                    string username = "";
                    do
                    {
                        username = studentCodeMaker(students.studentname);
                        //Check if username is available
                    } while (db.Users.Where(u => u.username.Equals(username)).Count() > 0);

                    Users user = new Users();
                    user.username = username;
                    user.userstatus = "Active";
                    user.usertype = "s";
                    user.psswd = "";
                    user.mail = mail;

                    db.Users.Add(user);
                    db.SaveChanges();

                    students.idUser = user.id;
                    students.studentname = students.studentname.ToUpper();
                    db.Students.Add(students);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                ViewBag.mailValidation = "Correo electrónico no disponible";
                return View(students);

            }
            catch (FormatException e)
            {
                ViewBag.mailValidation = "Formato de correo erróneo";
                return View(students);
            }
            catch (Exception e) {
                return View(students);
            }

        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students students = db.Students.Find(id);
            Users user = db.Users.Find(students.idUser);
            if (students == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.mail = user.mail;
            ViewBag.status = user.userstatus;
             
            return View(students);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,studentname,courseyear,idUser")] Students students, string mail, string status)
        {
            ViewBag.mail = mail;
            ViewBag.status = status;
            try
            {

                MailAddress m = new MailAddress(mail);

                Users user = db.Users.Find(students.idUser);
                bool isUniqueMail = db.Users.Where(u => u.mail.Equals(mail))
                                            .Where(u => u.userstatus.Equals("Active"))
                                            .Where(u => u.id != students.idUser)
                                            .Count() == 0;

                if (isUniqueMail)
                {
                    user.mail = mail;
                    user.userstatus = status;
                    db.Entry(user).State = EntityState.Modified;
                    students.studentname = students.studentname.ToUpper();
                    db.Entry(students).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", "Students", new { id = students.id });
                }

                ViewBag.mailValidation = "Correo eletrónico no disponible";
                return View(students);
            }
            catch (FormatException e) {

                ViewBag.mailValidation = "Formato de correo erróneo";
                return View(students);
            }
            catch(Exception e){
                
                return View(students);
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Students students = db.Students.Find(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Students students = db.Students.Find(id);
            db.Students.Remove(students);
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

        public string studentCodeMaker(string name) {
            //This method return an student id, made with last name initials
            //Respect this format: IIYYRRRRR
            //Where:    I: Capital last name initials
            //          Y: Year of creation, 21 for example
            //          R: Random number between 1000 and 9999

            string code = "";
            string[] initials = name.Split(Char.Parse(" "));

            for (int i = 0; i <= initials.Length - 1; i++) {
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
