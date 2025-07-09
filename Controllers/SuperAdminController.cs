using ComputerTypingWebApp.Models;
using ComputerTypingWebApp.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace ComputerTypingWebApp.Controllers
{
    public class SuperAdminController : Controller
    {
        private dbContext myDbContext;

        public SuperAdminController(dbContext context)
        {
            myDbContext = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var TotalRevenue = myDbContext.Payment.Select(x => x.Amount).Sum();
            ViewBag.Revenue = TotalRevenue;


            var InstructorCount = myDbContext.Instructor.Count();
            ViewBag.InstructorCount = InstructorCount;

            var studentCount = myDbContext.Students.Count();
            ViewBag.StudentCount = studentCount;

            int totalInstituteCount = myDbContext.Institute.Count();
            ViewBag.InstituteCount = totalInstituteCount;
            return View();
        }

        public IActionResult AddInstitute()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
        public async Task<IActionResult> Submit(InstituteVM ins, IFormCollection form)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            string NameOfInstitute = Request.Form["nameofinstitute"];
            string NameOfPrincipal = Request.Form["nameofprincipal"];
            string InstituteAddress = Request.Form["instituteaddress"];
            string InstituteCode = Request.Form["institutecode"];
            string ContactNo = Request.Form["contactno"];
            string Email = Request.Form["email"];
            int NoOfComputer = Convert.ToInt16(Request.Form["noofcomputer"]);
            string PrincipalPhototURL = Request.Form["principalphotourl"];
            string InstituteSymbolURL = Request.Form["institutesymbolurl"];
            string Status = "Active";
            //string StatusCode = Request.Form["code"];
            if (myDbContext.Institute.Any(x => x.InstituteCode == InstituteCode && x.InstituteName==NameOfInstitute && x.ContactNo==(ContactNo)))
            {
                TempData["ErrorMsg"] = "Institute already exists!";
                return RedirectToAction("AddInstitute");
            }
            else
            {

                Institute institute = new Institute();

                string uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                institute.PrincipalPhotoUrl = await UploadFile(ins.PrincipalPhotoUrl, uploadDirectory);
                institute.InstituteSymbolUrl = await UploadFile(ins.InstituteSymbolUrl, uploadDirectory);

                institute.InstituteName = NameOfInstitute;
                institute.PrincipalName = NameOfPrincipal;
                institute.InstituteAddress = InstituteAddress;
                institute.InstituteCode = InstituteCode;
                institute.ContactNo = ContactNo;
                institute.Email = Email;
                institute.NoOfComputer = NoOfComputer;
                institute.PrincipalPhotoUrl = PrincipalPhototURL;
                institute.InstituteSymbolUrl = InstituteSymbolURL;
                institute.Status = Status;
                myDbContext.Add(institute);
                await myDbContext.SaveChangesAsync();
                return RedirectToAction("index");
            }
        }

        private async Task<string> UploadFile(IFormFile file, string uploadDirectory)
        {
            try
            {
                if (file == null) return null;

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                string fileExtension = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadDirectory, fileNameWithoutExtension + fileExtension);

                int counter = 1;

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    filePath = Path.Combine(uploadDirectory, $"{fileNameWithoutExtension}_{counter}{fileExtension}");
                    counter++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return null;
            }
        }

        public IActionResult InstituteReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report = myDbContext.Institute.ToList();

            return View(report);
        }
        public IActionResult StudentReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report = (from st in myDbContext.Students
                          join us in myDbContext.Users on st.StudentUserName equals us.Username
                          //where st.Session == HttpContext.Session.GetString("CurrentSessionId")
                          select new StudentViewVM
                          {
                              FirstName = st.FirstName,
                              LastName = st.LastName,
                              MobileNo = st.MobileNumber,
                              InstituteName = myDbContext.Institute.Where(x => x.Id == us.InstituteId).Select(x => x.InstituteName).FirstOrDefault(),
                              Status = st.Status,
                          }).ToList();
            return View(report);
        }
        public IActionResult InstructorReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report1 = myDbContext.Instructor.ToList();
            return View(report1);
        }
        public IActionResult GenderReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report2 = myDbContext.Gender.ToList();

            return View(report2);
        }
        public IActionResult AddGender()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            return View("AddGender");
        }
        public IActionResult SaveGender()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            string NameOfGender = Request.Form["nameofGender"];
            Gender gender = new Gender();
            gender.Name = NameOfGender;
            myDbContext.Gender.Add(gender);
            myDbContext.SaveChanges();
            return RedirectToAction("GenderReport");
        }
        public IActionResult HandicapReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report3 = myDbContext.Handicap.ToList();
            return View(report3);
        }
        public IActionResult AddHandicap()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            return View("AddHandicap");
        }
        public IActionResult CourseReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report4 = myDbContext.Course.ToList();
            return View(report4);
        }
        public IActionResult SubjectReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report5 = (from a in myDbContext.Course
                           join b in myDbContext.Subject on a.Id equals b.CourseId
                           select new SubjectReportVM { CourseName = a.CourseName, SubjectName = b.SubjectName }).ToList();
            return View(report5);
        }
        public IActionResult IdentityDocReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report6 = myDbContext.IdentityDoc.ToList();
            return View(report6);
        }
        public IActionResult SaveHandicap()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            string Handicap = Request.Form["Handicap"];
            Handicap handicap = new Handicap();
            handicap.HadicapName = Handicap;

            myDbContext.Handicap.Add(handicap);
            myDbContext.SaveChanges();

            return RedirectToAction("HandicapReport");
        }
        public IActionResult AddCourse()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
        public IActionResult AddSubject()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var courseList = myDbContext.Course.ToList();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var Model in courseList)
            {
                list.Add(new SelectListItem { Text = Model.CourseName, Value = Model.Id.ToString() });
            }
            ViewBag.CourseList = list;


            var studentTypeList = myDbContext.studenttype.ToList();

            List<SelectListItem> list2 = new List<SelectListItem>();
            foreach (var Model in studentTypeList)
            {
                list2.Add(new SelectListItem { Text = Model.StudentTypeDesc, Value = Model.StudentType.ToString() });
            }
            ViewBag.StudentTypeList = list2;

            return View();
        }
        public IActionResult AddIdentityDoc()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
        public IActionResult SaveCourse()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            string coursename = Request.Form["coursename"];
            Course cc = new Course();
            cc.CourseName = coursename;

            myDbContext.Course.Add(cc);
            myDbContext.SaveChanges();

            return RedirectToAction("CourseReport");
        }
        public IActionResult SaveSubject()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            string CourseId = Request.Form["Course"];
            string subjectName = Request.Form["SubjectName"];

            Subject cc = new Subject();
            cc.CourseId = Convert.ToInt32(CourseId);
            cc.SubjectName = subjectName;

            myDbContext.Subject.Add(cc);
            myDbContext.SaveChanges();

            return RedirectToAction("SubjectReport");
        }
        public IActionResult SaveIdentityDoc()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            string Docname = Request.Form["docname"];
            IdentityDoc cc = new IdentityDoc();
            cc.DocName = Docname;

            myDbContext.IdentityDoc.Add(cc);
            myDbContext.SaveChanges();

            return RedirectToAction("IdentityDocReport");
        }
        public IActionResult CoursesUpload()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            if (InstituteId == 0)
            {
                var report = (from a in myDbContext.CoursesUpload
                              join b in myDbContext.Subject on a.SubjectId equals b.Id
                              join c in myDbContext.Course on a.CourseId equals c.Id
                              join d in myDbContext.coursepractices on a.PracticeId equals d.Id

                              select new CourseUploadVM { Id = a.Id, CourseName = c.CourseName, SubjectName = b.SubjectName, PracticeName = d.PracticeName, PracticeData = a.PracticeData }).ToList();
                return View(report);
            }
            else
            {
                var report = (from a in myDbContext.CoursesUpload
                              join b in myDbContext.Subject on a.SubjectId equals b.Id
                              join c in myDbContext.Course on a.CourseId equals c.Id
                              join d in myDbContext.coursepractices on a.PracticeId equals d.Id
                              where a.InstituteId == InstituteId
                              select new CourseUploadVM { Id = a.Id, CourseName = c.CourseName, SubjectName = b.SubjectName, PracticeName = d.PracticeName, PracticeData = a.PracticeData }).ToList();
                return View(report);
            }
        }

        public JsonResult GetSubjectsByCourseId(int courseId)
        {
            var subjects = myDbContext.Subject.Where(s => s.CourseId == courseId).Select(s => new
            {
                SubjectId = s.Id,
                SubjectName = s.SubjectName
            }).ToList();

            return Json(subjects);
        }

        public IActionResult AddPracticeData()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            TempData["SuccessMsg"] = "";
            ViewBag.DashId = HttpContext.Session.GetString("DashId");
            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            var courseList = myDbContext.Course.ToList();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var Model in courseList)
            {
                list.Add(new SelectListItem { Text = Model.CourseName, Value = Model.Id.ToString() });
            }
            ViewBag.CourseList = list;

            var subjectList = myDbContext.Subject.Where(x => x.SubjectName != null && x.SubjectName != "").ToList();

            List<SelectListItem> list2 = new List<SelectListItem>();
            foreach (var Model in subjectList)
            {
                list2.Add(new SelectListItem { Text = Model.SubjectName, Value = Model.Id.ToString() });
            }
            ViewBag.SubjectList = list2;


            var coursepracticesList = myDbContext.coursepractices.Where(x=>x.InstituteId == instituteId).ToList();

            List<SelectListItem> list3 = new List<SelectListItem>();
            foreach (var Model in coursepracticesList)
            {
                list3.Add(new SelectListItem { Text = Model.PracticeName, Value = Model.Id.ToString() });
            }
            ViewBag.PracticeNameList = list3;

            return View();
        }
        public IActionResult SaveCourseData()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }

                int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                int userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

                string CourseId = Request.Form["Course"];
                string subjectId = Request.Form["Subject"];
                string practiceId = Request.Form["PracticeName"];
                string PracticeUnicode = Request.Form["Unicode"];
                string PracticeData = PracticeUnicode;

                CoursesUpload cc = new CoursesUpload();
                cc.CourseId = Convert.ToInt32(CourseId);
                cc.SubjectId = Convert.ToInt32(subjectId);
                cc.PracticeId = Convert.ToInt32(practiceId);
                cc.PracticeData = PracticeData;
                cc.UniCodePracticeData = PracticeUnicode;
                cc.InstituteId = InstituteId;
                cc.UserId = userid;

                myDbContext.CoursesUpload.Add(cc);
                myDbContext.SaveChanges();
                TempData["SuccessMsg"] = "Course data uploaded successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            TempData["DashId"] = "3"; //HttpContext.Session.GetString("DashId");

            return RedirectToAction("CoursesUpload");
        }
        public IActionResult LockUnlockStudent()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var studentList = myDbContext.Students.ToList();
            List<SelectListItem> studentlist = new List<SelectListItem>();
            foreach (var Model in studentList)
            {
                studentlist.Add(new SelectListItem { Text = Model.FirstName, Value = Model.Id.ToString() });
            }
            ViewBag.StudentList = studentlist;
            return View();
        }
        public IActionResult NoticeReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var noticereport = (from not in myDbContext.Notices
                                join us in myDbContext.Users on not.ToUserId equals us.Id
                                select new NoticeReportVM { Id = not.Id, UserName = us.Username, NoticeText = not.NoticeText, CreatedAt = not.CreatedAt }).ToList();


            return View(noticereport);
        }
        public IActionResult AddNotice()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var usersList = myDbContext.Users.Where(x => x.RoleId == 1 && x.IsActive == true).ToList();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var Model in usersList)
            {
                list.Add(new SelectListItem { Text = Model.Username, Value = Model.Id.ToString() });
            }
            ViewBag.UsersList = list;

            ViewBag.DashId = HttpContext.Session.GetString("DashId");

            return View();
        }
        public IActionResult SaveNotice()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            string UserId = Request.Form["UserName"];
            string NoticeText = Request.Form["NoticeText"];

            Notices notices = new Notices();
            notices.ToUserId = Convert.ToInt32(UserId);
            notices.NoticeText = NoticeText;
            notices.CreatedAt = DateTime.Now;
            notices.CreatedBy = 1;

            myDbContext.Notices.Add(notices);
            myDbContext.SaveChanges();

            return RedirectToAction("NoticeReport");
        }
        public IActionResult LicenseActivation()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public IActionResult AddAdmin()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var instituteList = myDbContext.Institute.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var Model in instituteList)
            {
                list.Add(new SelectListItem { Text = Model.InstituteName, Value = Model.Id.ToString() });
            }
            ViewBag.InstituteList = list;

            return View();
        }
        public IActionResult SaveAdmin()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }

                string UserName = Request.Form["UserName"];
                string Password = Request.Form["Password"];
                string Email = Request.Form["Email"];
                string Institute = Request.Form["Institute"];

                if (Institute == "" | Institute == null)
                {
                    TempData["ErrorMsg"] = "Institute is required.";
                    return RedirectToAction("AddAdmin");
                }

                var userList = myDbContext.Users.Where(x => x.Username.ToLower() == UserName.ToLower()).ToList();
                if (userList.Count > 0)
                {
                    TempData["ErrorMsg"] = "Username already exists!";
                    return RedirectToAction("AddAdmin");
                }

                Users user = new Users();
                user.Username = UserName;
                user.Password = Password;
                user.Email = Email;
                user.IsActive = true;
                user.RoleId = (int?)RoleEnums.Admin;
                user.InstituteId = Convert.ToInt32(Institute);

                myDbContext.Users.Add(user);
                myDbContext.SaveChanges();
                TempData["SuccessMsg"] = "Admin/Principle added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            return RedirectToAction("AddAdmin");
        }
        public IActionResult Revenue()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var revenueReport = (from pay in myDbContext.Payment
                                 join usr in myDbContext.Users on pay.UserId equals usr.Id
                                 join inst in myDbContext.Institute on usr.InstituteId equals inst.Id
                                 select new RevenueReportVM
                                 {
                                     InstituteName = inst.InstituteName,
                                     MoneyReceived = pay.Amount,
                                     Mode = pay.PaymentMode,
                                     PaymentDate = pay.PaymentDate.ToShortDateString()
                                 }
                                 ).ToList();

            return View(revenueReport);
        }
        public IActionResult Settings()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }


            return View();
        }
        public IActionResult LockStudent()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            int studentid = Convert.ToInt32(Request.Form["StudentName"]);
            var student = myDbContext.Students.Where(x => x.Id == studentid).FirstOrDefault();
            if (student != null)
            {
                student.Status = "Locked";
                myDbContext.SaveChanges();
                ViewBag.Message = "Student is Locked Successfully";
            }


            return RedirectToAction("LockUnlockStudent");
        }
        public IActionResult UnlockLockStudent()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            int studentid = Convert.ToInt32(Request.Form["StudentName"]);
            var student = myDbContext.Students.Where(x => x.Id == studentid).FirstOrDefault();
            if (student != null)
            {
                student.Status = "Active";
                myDbContext.SaveChanges();
                ViewBag.Message = "Student is UnLocked Successfully";
            }


            return RedirectToAction("LockUnlockStudent");
        }
        public JsonResult DeleteNotices(string id)
        {
            // Delete institute timings

            string errorresult = string.Empty;
            string successresult = string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                errorresult = "Invalid Notice ID.";
            }
            int noticeId;
            if (!int.TryParse(id, out noticeId))
            {
                errorresult = "Invalid Notice ID format.";
            }
            var record = myDbContext.Notices.Where(x => x.Id == noticeId).FirstOrDefault();
            if (record == null)
            {
                errorresult = "Notice not found.";
            }

            myDbContext.Notices.Remove(record);
            myDbContext.SaveChanges();

            if (string.IsNullOrEmpty(errorresult))
            {
                successresult = "Notice deleted successfully.";
                return Json(new { success = true, message = successresult });
            }
            else
            {
                return Json(new { success = false, message = errorresult });
            }
        }
        public JsonResult DeleteCourse(string Id)
        {
            string errorresult = string.Empty;
            string successresult = string.Empty;

            if (string.IsNullOrEmpty(Id))
            {
                errorresult = "Invalid Course ID.";
            }
            int courseId;
            if (!int.TryParse(Id, out courseId))
            {
                errorresult = "Invalid Course ID format.";
            }
            var record = myDbContext.CoursesUpload.Where(x => x.Id == courseId).FirstOrDefault();
            if (record == null)
            {
                errorresult = "Course not found.";
            }

            myDbContext.CoursesUpload.Remove(record);
            myDbContext.SaveChanges();

            if (string.IsNullOrEmpty(errorresult))
            {
                successresult = "Course deleted successfully.";
                return Json(new { success = true, message = successresult });
            }
            else
            {
                return Json(new { success = false, message = errorresult });
            }
        }
        public IActionResult DeactivateInstitute(string id)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            
            var inst = myDbContext.Institute.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
            
                inst.Status = "Deactive";
                myDbContext.SaveChanges();
                ViewBag.Message = "Institute is Locked Successfully";           
            return RedirectToAction("InstituteReport");
        }
        public IActionResult ActivateInstitute(string id)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var inst = myDbContext.Institute.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();
            inst.Status = "Active";
            myDbContext.SaveChanges();
            ViewBag.Message = "Institute is UnLocked Successfully";


            return RedirectToAction("InstituteReport");
        }

        public JsonResult GetPracticeName(int courseId, int subjectId)
        {
            int instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var coursepracticesList = myDbContext.coursepractices.Where(x => x.InstituteId == instituteId && x.SubjectId == subjectId).ToList();

            List<SelectListItem> list3 = new List<SelectListItem>();
            foreach (var Model in coursepracticesList)
            {
                list3.Add(new SelectListItem { Text = Model.PracticeName, Value = Model.Id.ToString() });
            }
            //ViewBag.PracticeNameList = list3;

            return Json(list3);
        }

    }
}
