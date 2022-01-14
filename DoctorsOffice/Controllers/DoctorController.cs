using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using DoctorsOffice.Models;
using System.Web.Mvc;

namespace DoctorsOffice.Controllers
{
    public class DoctorController : Controller
    {
        public ActionResult DoctorHomePage()
        {
            return View();
        }
        public ActionResult DoctorRegisterProfession()
        {
            return View();
        }

        public ActionResult DocSubmitProf(Doctor doc)
        {
            using (var db = new DoctorsOfficeEntities())
            {
                var usrId = Convert.ToInt32(Session["id"]);
                var foundDoc = db.Doctors.FirstOrDefault(d => d.userId == usrId);
                if (foundDoc != null)
                {
                    foundDoc.Speciality = doc.Speciality;
                    Session["userProfession"] = foundDoc.Speciality;
                    db.Doctors.AddOrUpdate(foundDoc);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("DoctorHomePage");
        }

        public ActionResult GetAppointment(int? selectDate = null)
        {
            try
            {
                var docsAMKA = Convert.ToInt32(Session["amka"]);
                if (Singleton.Instance.GetList().Count == 0)
                    Singleton.Instance.UpdateList(docsAMKA);
                var appointments = new List<Appointment>();
                
                switch (selectDate)
                {
                    case 1:
                        appointments = Singleton.Instance.GetList().Where(
                            ap => 
                                ap.doctorsAMKA == docsAMKA &&
                                ap.date == DateTime.Now.Date
                        ).ToList();
                        break;
                    case 2:
                        appointments = Singleton.Instance.GetList().Where(
                                ap =>
                                    ap.doctorsAMKA == docsAMKA &&
                                    ap.date >= DateTime.Now.Date &&
                                    ap.date <= DateTime.Now.Date.AddDays(7))
                                .ToList();
                        break;
                    default:
                        appointments = Singleton.Instance.GetList();
                        break;
                }
                return View(appointments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("DoctorHomePage");
            }
        }

        public ActionResult CreateNewAppointment(DateTime date, string patientsAMKA = null)
        {
            try
            {
                if (date.Equals(null))
                {
                    Session["message"] = "Please enter the date the appointment will be available!";
                    return RedirectToAction("GetAppointment", ViewBag);
                }
                else if (patientsAMKA != "" && patientsAMKA.Length > 10)
                {
                    Session["message"] = "Please enter a valid AMKA for your patient";
                    return RedirectToAction("GetAppointment");
                }

                using (var db = new DoctorsOfficeEntities())
                {
                    var docsAMKA = Convert.ToInt32(Session["amka"]);
                    var app = new Appointment
                    {
                        date = date,
                        startTime = date,
                        endTime = date.AddHours(1),
                        patientAMKA = patientsAMKA != "" ? Convert.ToInt32(patientsAMKA) : 0,
                        doctorsAMKA = docsAMKA,
                        isAvailable = (patientsAMKA == "") // depends if patients amka has value
                    };
                    db.Appointments.Add(app);
                    db.SaveChanges();
                    Singleton.Instance.UpdateList(docsAMKA);
                }
                Session["message"] = $"New appointment at {date} has been added successfully";
                return RedirectToAction("GetAppointment");
            }
            catch (Exception e)
            {
                Session["message"] = $"Something went wrong!\n{e}";
                return RedirectToAction("GetAppointment");
            }
        }

        public ActionResult DeleteAppointment(int id)
        {
            try
            {
                using (var db = new DoctorsOfficeEntities())
                {
                    db.Appointments.Remove(db.Appointments.FirstOrDefault(ap => ap.Id == id));
                    db.SaveChanges();
                }
                return RedirectToAction("GetAppointment");
            }
            catch (Exception e)
            {
                Session["message"] = $"Something went wrong!\n{e}";
                return RedirectToAction("GetAppointment", ViewBag);
            }
        }
    }
}