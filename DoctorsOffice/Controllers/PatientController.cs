using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoctorsOffice.Models;

namespace DoctorsOffice.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult PatientHomePage()
        {
            return View();
        }

        public ActionResult GetAppointment(string queryDoc = "ofthalmiatros")
        {
            try
            {
                var availableAppointments = new List<AppointmentDoctorView>();
                using (var db = new DoctorsOfficeEntities())
                {
                    availableAppointments = db.Doctors.Join(db.Appointments,
                        d => d.doctorAMKA,
                        ap => ap.doctorsAMKA,
                        (d, ap) => new
                        {
                            Id = ap.Id,
                            date = ap.date.Value,
                            startTime = ap.startTime.Value,
                            Speciality = d.Speciality,
                            endTime = ap.endTime.Value,
                            doctorsAMKA = ap.doctorsAMKA,
                            isAvailable = ap.isAvailable
                        }
                    ).Where(adv => adv.Speciality == queryDoc && adv.isAvailable == true).Join(db.Users,
                        av => av.doctorsAMKA,
                        us => us.AMKA,
                        (ap, us) => new AppointmentDoctorView
                        {
                            Id = ap.Id,
                            date = ap.date,
                            startTime = ap.startTime,
                            endTime = ap.endTime,
                            doctorsAMKA = ap.doctorsAMKA,
                            Lastname = us.LastName
                        }
                    ).OrderBy(ob => ob.date).Distinct().ToList();
                }

                return View(availableAppointments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View(new List<AppointmentDoctorView>());
            }
        }

        public ActionResult ReserveAppointment(int id)
        {
            using (var db = new DoctorsOfficeEntities())
            {
                var appointmentToReserve = db.Appointments.FirstOrDefault(ap => ap.Id == id);
                appointmentToReserve.patientAMKA = SUser.Instance.GetInstance().AMKA;
                appointmentToReserve.isAvailable = false;
                db.Appointments.AddOrUpdate(appointmentToReserve);
                db.SaveChanges();
            }
            return RedirectToAction("MyAppointments");
        }
    }
}