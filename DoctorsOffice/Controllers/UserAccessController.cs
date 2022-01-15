using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoctorsOffice.Models;
using Microsoft.Ajax.Utilities;

namespace DoctorsOffice.Controllers
{
    public class UserAccessController : Controller
    {

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LoginUser(Users user)
        {
            if (user == null)
                return RedirectToAction("Login");

            using (var db = new DoctorsOfficeEntities())
            {
                var foundUser =
                    db.Users.FirstOrDefault(usr =>
                        usr.Username == user.Username &&
                        usr.Password == user.Password
                    );

                if (foundUser == null)
                    return RedirectToAction("Login");
                
                switch (foundUser.userType)
                {
                    case "Doctor":
                    {
                        var docProf = db.Doctors.Where(d => d.userId == foundUser.Id).Select(d => d.Speciality).FirstOrDefault();
                        if (docProf != null)
                            SUser.Instance.UpdateInstance(new Users
                            {
                                Id = foundUser.Id,
                                Username = foundUser.Username,
                                LastName = foundUser.LastName,
                                type = foundUser.userType,
                                AMKA = foundUser.AMKA.ToString(),
                                docProfession = docProf
                            });
                        break;
                    }
                    case "Admin":
                    {
                        var AdminId = db.Admins.Where(a => a.userId == foundUser.Id).Select(a => a.Id).FirstOrDefault();
                            if (AdminId != 0)
                            {
                                SUser.Instance.UpdateInstance(new Users
                                {
                                    Id = foundUser.Id,
                                    Username = foundUser.Username,
                                    LastName = foundUser.LastName,
                                    type = foundUser.userType,
                                    AMKA = foundUser.AMKA.ToString(),
                                    adminID = AdminId
                                }); 
                            }
                        break;
                    }
                }

                if(SUser.Instance.GetInstance().Id == 0) // if no instance was created on the above switch
                    SUser.Instance.UpdateInstance(new Users
                    {
                        Id = foundUser.Id,
                        Username = foundUser.Username,
                        LastName = foundUser.LastName,
                        type = foundUser.userType,
                        AMKA = foundUser.AMKA.ToString(),
                    });

                switch (foundUser.userType)
                {
                    case "Patient":
                        return RedirectToAction("PatientHomePage", "Patient");
                    case "Doctor":
                        return RedirectToAction("DoctorHomePage","Doctor");
                    case "Admin":
                        return RedirectToAction("AdminPanel", "Admin");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        #region Register
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult RegisterUser(Users user)
        {
            try
            {
                if (user.Username == null)
                    return RedirectToAction("Login");

                var dbUser = CreateUser(user);
                if (dbUser.Id == 0) return RedirectToAction("Login");

                switch (user.type)
                {
                    case "Patient":
                        CreatePatient(dbUser);
                        break;
                    case "Doctor":
                        CreateDoctor(dbUser);
                        break;
                    case "Admin":
                        CreateAdmin(dbUser);
                        break;
                        
                }
                return RedirectToAction("LoginUser", user);
            }
            catch (Exception e)
            {
                Session["message"] = $"Something went wrong!\nPlease try again..<br>{e}";
                Console.WriteLine(e);
                return RedirectToAction("Register");
            }
        } 
        #endregion

        public ActionResult Logout()
        {
            try
            {
                SUser.Instance.LogoutInstance();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Index", "Home");
            }
        }

        #region User Creation Methods
        public User CreateUser(Users user)
        {
            try
            {
                var dbUser = new User
                { //create new db user to add to db
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    AMKA = Convert.ToInt32(user.AMKA),
                    Password = user.Password,
                    userType = user.type
                };

                using (var db = new DoctorsOfficeEntities())
                {
                    db.Users.Add(dbUser);
                    db.SaveChanges();
                }

                return dbUser;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new User();
            }
        }

        public void CreatePatient(User dbUser)
        {
            try
            {
                using (var db = new DoctorsOfficeEntities())
                {
                    db.Patients.Add(new Patient
                    {
                        userId = dbUser.Id,
                        Name = dbUser.FirstName,
                        Surname = dbUser.LastName,
                        patientAMKA = dbUser.AMKA
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void CreateDoctor(User dbUser, int? adminId = null)
        {
            try
            {
                using (var db = new DoctorsOfficeEntities())
                {
                    db.Doctors.Add(new Doctor()
                    {
                        userId = dbUser.Id,
                        doctorAMKA = dbUser.AMKA,
                        ADMIN_Id = adminId
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void CreateAdmin(User dbUser)
        {
            try
            {
                using (var db = new DoctorsOfficeEntities())
                {
                    db.Admins.Add(new Admin()
                    {
                        userId = dbUser.Id,
                        Username = dbUser.Username,
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 
        #endregion
    }
}