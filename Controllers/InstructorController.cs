using Microsoft.AspNetCore.Mvc;
using ComputerTypingWebApp.Models;

namespace ComputerTypingWebApp.Controllers
{
    public class InstructorController : Controller
    {
        private dbContext myDbContext;

        public InstructorController(dbContext context)
        {
            myDbContext = context;

        }
        public IActionResult Index()
        {
           
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            string instituteID = HttpContext.Session.GetString("InstituteID").ToString();
            int registeredStudentsCount = myDbContext.Users.Where(x => x.RoleId == 1 && x.InstituteId == Convert.ToInt32(instituteID)).Count();
            ViewBag.StudentsCount = registeredStudentsCount;
            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var notice = myDbContext.Notices.Where(x => x.ToUserId == userid).OrderByDescending(X => X.CreatedAt).FirstOrDefault();
            if(notice != null)
            {
             ViewBag.Notice = notice.NoticeText;
            }
            else
            {
                ViewBag.Notice = "";
            }
            return View();
        }

        public JsonResult GetSubjectList(string StudentUserName)
        {
            int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var data = (from st in myDbContext.Students
                        join crs in myDbContext.Coursefee on st.StudentType.ToString() equals crs.StudentType
                        where st.StudentUserName == StudentUserName && crs.Instituteid == InstituteId
                        select new { sub30 = st.SelectSub30wpm, sub40 = st.SelectSub40wpm, courseid = crs.subjectid, fee = crs.Fees }
                        ).ToList();

            return Json(data);
        }
    }
}
