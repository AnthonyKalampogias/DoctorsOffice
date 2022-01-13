using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoctorsOffice.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult PatientHomePage()
        {
            return View();
        }
    }
}