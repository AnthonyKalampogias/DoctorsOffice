using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorsOffice.Models
{
    public class SUser
    {
        //Variable
        private static SUser instance;
        private Users user;

        //Constructor
        private SUser()
        {
            user = new Users();
        }

        //Property
        public static SUser Instance => instance ?? (instance = new SUser());

        public void UpdateInstance(Users newInstance)
        {
            if (newInstance != null)
                user = newInstance;
        }

        public Users GetInstance()
        {
            return user;
        }

        public void DropInstance()
        {
            user = new Users();
        }
    }
}