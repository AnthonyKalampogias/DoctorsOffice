using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
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
                var appointments = new List<Appointment>();
                var docsAMKA = Convert.ToInt32(Session["amka"]);
                using (var db = new DoctorsOfficeEntities())
                {
                    switch (selectDate)
                    {
                        case 1:
                            appointments = db.Appointments.Where(
                                ap => ap.doctorsAMKA == docsAMKA &&
                                    ap.date >= DateTime.Now.AddHours(-23) &&
                                    ap.date <= DateTime.Now.AddHours(7)
                                ).ToList();
                            break;
                        case 2:
                            appointments = db.Appointments.Where(ap => 
                                ap.doctorsAMKA == docsAMKA
                                && ap.date <= DateTime.Now.AddDays(7)).ToList();
                            break;
                        default:
                            appointments = db.Appointments.Where(ap => ap.doctorsAMKA == docsAMKA).ToList();
                            break;
                    }
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
                else if (patientsAMKA != "" && patientsAMKA.Length < 10)
                {
                    Session["message"] = "Please enter a valid AMKA for your patient";
                    return RedirectToAction("GetAppointment");
                }

                using (var db = new DoctorsOfficeEntities())
                {
                    var app = new Appointment
                    {
                        date = date,
                        startTime = date,
                        endTime = date.AddHours(1),
                        patientAMKA = patientsAMKA != "" ? Convert.ToInt32(patientsAMKA) : 0,
                        doctorsAMKA = Convert.ToInt32(Session["amka"]),
                        isAvailable = (patientsAMKA == "") // depends if patients amka has value
                    };
                    db.Appointments.Add(app);
                    db.SaveChanges();
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