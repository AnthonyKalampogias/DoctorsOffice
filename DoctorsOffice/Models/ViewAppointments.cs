using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorsOffice.Models
{
    public class ViewAppointments
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public Nullable<int> patientAMKA { get; set; }
        public Nullable<int> doctorsAMKA { get; set; }
        public Nullable<bool> isAvailable { get; set; }
    }
}