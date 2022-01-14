using DoctorsOffice;
using System.Collections.Generic;
using System.Linq;

namespace DoctorsOffice.Models
{
    class Singleton
    {
        //Variable
        private static Singleton instance;
        private List<Appointment> dbAppointments;

        //Constructor
        private Singleton() {
            dbAppointments = new List<Appointment>();
        }

        //Property
        public static Singleton Instance => instance ?? (instance = new Singleton());

        //Methods
        public void UpdateList(int docAMKA)
        {
            using(var db = new DoctorsOfficeEntities())
            {
                dbAppointments = db.Appointments.Where(ap => ap.doctorsAMKA == docAMKA).OrderBy(ob => ob.date).ToList();
            }
        }

        public List<Appointment> GetList()
        {
            return dbAppointments;
        }
    }
}