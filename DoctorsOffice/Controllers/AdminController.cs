using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoctorsOffice.Models;

namespace DoctorsOffice.Controllers
{
    public class AdminController : Controller
    {
        private UserAccessController userAccessController = new UserAccessController();

        // GET: Admin
        public ActionResult AdminPanel()
        {
            using (var db = new DoctorsOfficeEntities())
            {
                var modelData = db.Users.Where(u => u.userType != "Admin").ToList();
                if (TempData["message"] != null)
                    ViewBag.message = TempData["message"];
                return View(modelData); 
            }
        }

        public ActionResult CreatePatient()
        {
            return View();
        }

        public ActionResult RegisterPatient(Users user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("AdminPanel");
                
                user.type = "Patient";
                var dbUser = userAccessController.CreateUser(user);
                if (dbUser != null)
                    userAccessController.CreatePatient(dbUser, SUser.Instance.GetInstance().adminID);
                
                TempData["message"] = $"Successfully added new patient {dbUser.FirstName} {dbUser.LastName} to the database!";
            }
            catch (Exception e)
            {
                TempData["message"] = $"Something went wrong... Please try again<br>{e}";
            }
            return RedirectToAction("AdminPanel");
        }
        public ActionResult CreateDoctor()
        {
            return View();
        }

        public ActionResult RegisterDoctor(Users doc)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("AdminPanel");

                doc.type = "Doctor";
                var dbUser = userAccessController.CreateUser(doc);
                if (dbUser != null)
                    userAccessController.CreateDoctor(dbUser, SUser.Instance.GetInstance().adminID);

                TempData["message"] = $"Successfully added new doctor {dbUser.FirstName} {dbUser.LastName} to the database!";
            }
            catch (Exception e)
            {
                TempData["message"] = $"Something went wrong... Please try again<br>{e}";
            }
            return RedirectToAction("AdminPanel");
        }

        public ActionResult DeleteUser(int id, string usrType)
        {
            if (id == 0 || usrType == "") 
                return RedirectToAction("AdminPanel");

            using (var db = new DoctorsOfficeEntities())
            {
                switch (usrType)
                {
                    case "Patient":
                        db.Patients.Remove(db.Patients.FirstOrDefault(ap => ap.userId == id));
                        break;
                    case "Doctor":
                        db.Doctors.Remove(db.Doctors.FirstOrDefault(d => d.userId == id));
                        break;
                }
                db.Users.Remove(db.Users.FirstOrDefault(u => u.Id == id));
                db.SaveChanges();
            }
            TempData["message"] = $"Successfully deleted {usrType} with Id {id} from the database!";
            return RedirectToAction("AdminPanel");
        }
    }
}