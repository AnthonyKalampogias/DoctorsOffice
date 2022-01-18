using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorsOffice.Models
{
    // class to "Join" Appointments and Doctor results 
    public class AppointmentDoctorView
    {
        public int Id { get; set; }
        public DateTime? date { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public long? userAMKA { get; set; }
        public bool? isAvailable { get; set; }

        public string Speciality { get; set; }
        public string Lastname { get; set; }
    }
}