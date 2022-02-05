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

        public ActionResult GetAppointment(string queryDoc)
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
                            ap.Id,
                            date = ap.date.Value,
                            startTime = ap.startTime.Value,
                            d.Speciality,
                            endTime = ap.endTime.Value,
                            ap.doctorsAMKA,
                            ap.isAvailable
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
                            userAMKA = ap.doctorsAMKA,
                            Lastname = us.LastName
                        }
                    ).Where(x => x.date >= DateTime.Now)
                        .OrderBy(ob => ob.date).Distinct().ToList();
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

        public ActionResult MyAppointments()
        {
            using (var db = new DoctorsOfficeEntities())
            {
                var patientAMKA = SUser.Instance.GetInstance().AMKA;
                var availableAppointments = db.Doctors.Join(db.Appointments,
                    d => d.doctorAMKA,
                    ap => ap.doctorsAMKA,
                    (d, ap) => new
                    {
                        Id = ap.Id,
                        date = ap.date.Value,
                        startTime = ap.startTime.Value,
                        endTime = ap.endTime.Value,
                        patientAMKA = ap.patientAMKA,
                        Speciality = d.Speciality,
                        isAvailable = ap.isAvailable
                    }
                ).Where(adv => adv.patientAMKA == patientAMKA).Join(db.Users,
                    av => av.patientAMKA,
                    us => us.AMKA,
                    (ap, us) => new AppointmentDoctorView
                    {
                        Id = ap.Id,
                        date = ap.date,
                        startTime = ap.startTime,
                        endTime = ap.endTime,
                        userAMKA = ap.patientAMKA,
                        Speciality = ap.Speciality,
                        Lastname = us.LastName
                    }
                ).Where(x => x.date >= DateTime.Now)
                .OrderBy(ob => ob.date).Distinct().ToList();
                
                return View(availableAppointments);
            }
        }
        public ActionResult MyPastAppointments()
        {
            using (var db = new DoctorsOfficeEntities())
            {
                var patientAMKA = SUser.Instance.GetInstance().AMKA;
                var availableAppointments = db.Doctors.Join(db.Appointments,
                    d => d.doctorAMKA,
                    ap => ap.doctorsAMKA,
                    (d, ap) => new
                    {
                        Id = ap.Id,
                        date = ap.date.Value,
                        startTime = ap.startTime.Value,
                        endTime = ap.endTime.Value,
                        patientAMKA = ap.patientAMKA,
                        Speciality = d.Speciality,
                        isAvailable = ap.isAvailable
                    }
                ).Where(adv => adv.patientAMKA == patientAMKA).Join(db.Users,
                    av => av.patientAMKA,
                    us => us.AMKA,
                    (ap, us) => new AppointmentDoctorView
                    {
                        Id = ap.Id,
                        date = ap.date,
                        startTime = ap.startTime,
                        endTime = ap.endTime,
                        userAMKA = ap.patientAMKA,
                        Speciality = ap.Speciality,
                        Lastname = us.LastName
                    }
                ).Where(x => x.date <= DateTime.Now)
                .OrderBy(ob => ob.date).Distinct().ToList();
                
                return View(availableAppointments);
            }
        }

        public ActionResult CancelReservation(int id)
        {
            using (var db = new DoctorsOfficeEntities())
            {
                var appointmentToReserve = db.Appointments.FirstOrDefault(ap => ap.Id == id);
                appointmentToReserve.patientAMKA = null;
                appointmentToReserve.isAvailable = true;
                db.Appointments.AddOrUpdate(appointmentToReserve);
                db.SaveChanges();
            }
            return RedirectToAction("MyAppointments");
        }
    }
}