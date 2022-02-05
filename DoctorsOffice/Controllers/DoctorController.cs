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
                var usrId = SUser.Instance.GetInstance().Id;
                var foundDoc = db.Doctors.FirstOrDefault(d => d.userId == usrId);
                if (foundDoc == null) return RedirectToAction("DoctorHomePage");
                foundDoc.Speciality = doc.Speciality;
                db.Doctors.AddOrUpdate(foundDoc);
                db.SaveChanges();

                SUser.Instance.UpdateInstance(new Users
                {
                    Id = SUser.Instance.GetInstance().Id,
                    Username = SUser.Instance.GetInstance().Username,
                    LastName = SUser.Instance.GetInstance().LastName,
                    type = SUser.Instance.GetInstance().type,
                    AMKA = SUser.Instance.GetInstance().AMKA,
                    docProfession = foundDoc.Speciality
                });
            }
            return RedirectToAction("DoctorHomePage");
        }

        public ActionResult GetAppointment(int? selectDate = null)
        {
            if (TempData["message"] != null)
                    ViewBag.message = TempData["message"];
            try
            {
                var docsAMKA = Convert.ToInt64(SUser.Instance.GetInstance().AMKA);
                if (SAppointments.Instance.GetInstance().Count == 0)
                    SAppointments.Instance.UpdateList(docsAMKA);
                var appointments = new List<Appointment>();
                
                switch (selectDate)
                {
                    case 1:
                        appointments = SAppointments.Instance.GetInstance().Where(
                            ap => 
                                ap.doctorsAMKA == docsAMKA &&
                                ap.date == DateTime.Now.Date
                        ).ToList();
                        break;
                    case 2:
                        appointments = SAppointments.Instance.GetInstance().Where(
                                ap =>
                                    ap.doctorsAMKA == docsAMKA &&
                                    ap.date >= DateTime.Now.Date &&
                                    ap.date <= DateTime.Now.Date.AddDays(7))
                                .ToList();
                        break;
                    default:
                        appointments = SAppointments.Instance.GetInstance();
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
                    TempData["message"] = "Please enter the date the appointment will be available!";
                    return RedirectToAction("GetAppointment", ViewBag);
                }
                else if (patientsAMKA != "" && patientsAMKA.Length > 10)
                {
                    TempData["message"] = "Please enter a valid AMKA for your patient";
                    return RedirectToAction("GetAppointment");
                }

                using (var db = new DoctorsOfficeEntities())
                {
                    var docsAMKA = Convert.ToInt64(SUser.Instance.GetInstance().AMKA);
                    var app = new Appointment
                    {
                        date = date,
                        startTime = date,
                        endTime = date.AddHours(1),
                        patientAMKA = patientsAMKA != "" ? Convert.ToInt64(patientsAMKA) : 0,
                        doctorsAMKA = docsAMKA,
                        isAvailable = (patientsAMKA == "") // depends if patients amka has value
                    };
                    db.Appointments.Add(app);
                    db.SaveChanges();
                    SAppointments.Instance.UpdateList(docsAMKA);
                }
                TempData["message"] = $"New appointment at {date} has been added successfully";
                return RedirectToAction("GetAppointment");
            }
            catch (Exception e)
            {
                TempData["message"] = $"Something went wrong!\n{e}";
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
                    var docsAMKA = Convert.ToInt64(SUser.Instance.GetInstance().AMKA);
                    SAppointments.Instance.UpdateList(docsAMKA);
                }
                return RedirectToAction("GetAppointment");
            }
            catch (Exception e)
            {
                TempData["message"] = $"Something went wrong!\n{e}";
                return RedirectToAction("GetAppointment", ViewBag);
            }
        }
    }
}