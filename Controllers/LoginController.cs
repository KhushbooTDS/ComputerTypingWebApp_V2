using ComputerTypingWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerTypingWebApp.Controllers
{
    public class LoginController : Controller
    {
        private dbContext myDbContext;

        public LoginController(dbContext context)
        {
            myDbContext = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginUser()
        {
            string UserName = Request.Form["username"];
            string Password = Request.Form["password"];

            var user = myDbContext.Users.Where(u => u.Username == UserName && u.Password == Password && u.IsActive == true).FirstOrDefault();

            if (user != null)
            {
                storeUserIdentity(user);
                CaptureUserLogin(user.Id.Value);

                if (user.RoleId == 1)
                {
                    // Student Login -- Redirect to Student Dashboard
                    HttpContext.Session.SetString("DashId", "2");
                    return RedirectToAction("Index", "Student");
                }
                else if (user.RoleId == 2)
                {
                    // Admin Login -- Redirect to Admin Dashboard
                    HttpContext.Session.SetString("DashId", "3");
                    return RedirectToAction("Index", "Admin");
                }

                else if (user.RoleId == 3)
                {
                    // Super Admin Login -- Redirect to SuperAdmin Dashboard
                    HttpContext.Session.SetString("DashId", "1");
                    return RedirectToAction("Index", "SuperAdmin");
                }
                else if (user.RoleId == 4)
                {
                    // Instructor Login -- Redirect to Instructor Dashboard
                    HttpContext.Session.SetString("DashId", "4");
                    return RedirectToAction("Index", "Instructor");
                }
                else
                {
                    ViewBag.Message = "Invalid Login!";
                    return View("Index");
                }
            }
            else
            {
                ViewBag.Message = "Invalid Username or Password!";
                return View("Index");
            }

        }

        public void storeUserIdentity(Users user)
        {
            //var serializedRecords = JsonConvert.SerializeObject(user);
            //HttpContext.Session.SetString("UserIdentity", serializedRecords);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("InstituteID", user.InstituteId.ToString());

            //Get Institute Name
            string InstituteName = myDbContext.Institute.Where(x => x.Id == user.InstituteId).Select(x => x.InstituteName).FirstOrDefault();
            if (InstituteName != null)
            {
                HttpContext.Session.SetString("InstituteName", InstituteName);
            }

            // Get Current Institute Session
            var allSessions = myDbContext.InstituteSessions.Where(x => x.InstituteId == user.InstituteId).ToList();
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            var currentSession = allSessions.Where(x => (Convert.ToInt32(x.startSessionYY) == currentYear && Convert.ToInt32(x.endSessionYY) == currentYear) && (currentMonth >= x.startMonth && currentMonth <= x.EndMonth)).FirstOrDefault();
            int CurrentSessionId = 0;
            if (currentSession != null)
            {
                CurrentSessionId = currentSession.Id;
            }
            HttpContext.Session.SetString("CurrentSessionId", CurrentSessionId.ToString());
        }

        public IActionResult SignOut()
        {
            string strUserId = HttpContext.Session.GetString("UserId");
            int UserId = Convert.ToInt32(strUserId);
            CaptureUserLogout(UserId);

            HttpContext.Session.Remove("UserName");
            return View("Index");

        }

        public void CaptureUserLogin(int userid)
        {
            UserLogins userLogin = new UserLogins();
            userLogin.UserId = userid;
            userLogin.Login = DateTime.Now;

            myDbContext.UserLogins.Add(userLogin);
            myDbContext.SaveChanges();
        }

        public void CaptureUserLogout(int userid)
        {
            UserLogins userLogin = new UserLogins();
            userLogin.UserId = userid;
            userLogin.LogOut = DateTime.Now;

            myDbContext.UserLogins.Add(userLogin);
            myDbContext.SaveChanges();
        }
    }
}
