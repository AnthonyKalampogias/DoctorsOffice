using System;
using DoctorsOffice;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsOffice.Models
{
    class SAppointments
    {
        //Variable
        private static SAppointments instance;
        private List<Appointment> dbAppointments;

        //Constructor
        private SAppointments() {
            dbAppointments = new List<Appointment>();
        }

        //Property
        public static SAppointments Instance => instance ?? (instance = new SAppointments());

        //Methods
        public void UpdateList(long docAMKA)
        {
            using(var db = new DoctorsOfficeEntities())
            {
                dbAppointments = db.Appointments.Where(ap => ap.doctorsAMKA == docAMKA).OrderBy(ob => ob.date).ToList();
            }
            dbAppointments = dbAppointments.Where(ap => ap.date.Value.Date >= DateTime.Now.Date).ToList();
        }

        public List<Appointment> GetInstance()
        {
            return dbAppointments;
        }

        public void DropInstance()
        {
            dbAppointments = new List<Appointment>();
        }
    }
}