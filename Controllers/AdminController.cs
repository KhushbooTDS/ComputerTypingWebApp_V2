using ClosedXML.Excel;
using ClosedXML.Parser;
using ComputerTypingWebApp.Enums;
using ComputerTypingWebApp.Models;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Users = ComputerTypingWebApp.Models.Users;

namespace ComputerTypingWebApp.Controllers
{
    public class AdminController : Controller
    {
        private dbContext myDbContext;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public AdminController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, dbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            myDbContext = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["UserName"] = HttpContext.Session.GetString("UserName");
            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var notice = myDbContext.Notices.Where(x => x.ToUserId == userid).OrderByDescending(X => X.CreatedAt).FirstOrDefault();
            string strNoticeText = string.Empty;
            if (notice != null)
            {
                strNoticeText = notice.NoticeText;
            }
            else
            {
                strNoticeText = "";
            }
            ViewBag.Notice = strNoticeText;

            //var TotalRevenue = myDbContext.Payment.Select(x => x.Amount).Sum();
            int studentRole = Convert.ToInt32(RoleEnums.Student);
            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            var TotalRevenue = (from pay in myDbContext.Payment
                                join us in myDbContext.Users on pay.UserId equals us.Id
                                where us.RoleId == studentRole
                                && us.InstituteId == instituteId
                                && pay.CreatedAt.Month == DateTime.Now.Month
                                && pay.CreatedAt.Year == DateTime.Now.Year
                                select new { Amount = pay.Amount }).ToList();
            decimal sumRevnue = TotalRevenue.Sum(x => x.Amount);

            ViewBag.Revenue = sumRevnue;

            var studentList = (from s in myDbContext.Students
                               join h in myDbContext.Handicap on s.Handicap equals h.Id
                               join g in myDbContext.Gender on s.Gender equals g.Id
                               join us in myDbContext.Users on s.StudentUserName equals us.Username
                               where s.IsDeleted == false && s.Status == "Active"
                               && us.IsActive == true && us.RoleId == studentRole
                               && us.InstituteId == instituteId
                               select new StudentViewVM
                               {
                                   Students = s,
                                   Handicaps = h,
                                   Genders = g
                               }).Count();
            ViewBag.StudentCount = studentList;


            return View();

        }
        public IActionResult InstituteSessions()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var institutesessions = (from i in myDbContext.InstituteSessions
                                     join ins in myDbContext.Institute on i.InstituteId equals ins.Id
                                     select new InstituteSessionsVM
                                     {
                                         instituteName = ins.InstituteName,
                                         StartSession = i.StartSession,
                                         startSessionYY = i.startSessionYY,
                                         EndSession = i.EndSession,
                                         endSessionYY = i.endSessionYY
                                     }).ToList();

            return View(institutesessions);
        }
        public IActionResult AddInstituteSessions()
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
        public IActionResult SaveInstituteSessions()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            int InstituteId = Convert.ToInt32(Request.Form["Institute"]);
            //int startsession = Convert.ToInt32(Request.Form["StartSessionMM"]);
            //int endsession = Convert.ToInt32(Request.Form["EndSessionsMM"]);
            //string startsessionyy = Request.Form["StartSessionYY"];
            //string endsessionyy = Request.Form["EndSessionsYY"];

            string startSession = Request.Form["StartSession"];
            string endSession = Request.Form["EndSessions"];

            DateTime dtStartSession = Convert.ToDateTime(startSession);
            DateTime dtEndSession = Convert.ToDateTime(endSession);

            if (myDbContext.InstituteSessions.Any(x => x.StartSession == dtStartSession.ToString("MMMM") && x.startSessionYY == dtStartSession.ToString("yyyy") && x.EndSession == dtEndSession.ToString("MMMM") && x.endSessionYY == dtEndSession.ToString("yyyy") && x.InstituteId == InstituteId))
            {
                TempData["ErrorMsg"] = "InstituteSessions already exists!";
                return RedirectToAction("AddInstituteSessions");
            }

            InstituteSessions report = new InstituteSessions();
            report.InstituteId = InstituteId;
            //report.StartSession = GetMonthName(startsession);
            report.StartSession = dtStartSession.ToString("MMMM");
            //report.EndSession = GetMonthName(endsession);
            report.EndSession = dtEndSession.ToString("MMMM");
            //report.startSessionYY = startsessionyy;
            report.startSessionYY = dtStartSession.ToString("yyyy");
            //report.endSessionYY = endsessionyy;
            report.endSessionYY = dtEndSession.ToString("yyyy");
            //report.startMonth = startsession;
            report.startMonth = dtStartSession.Month;
            //report.EndMonth = endsession;
            report.EndMonth = dtEndSession.Month;

            myDbContext.InstituteSessions.Add(report);
            myDbContext.SaveChanges();
            return RedirectToAction("InstituteSessions", "Admin");
        }
        public string GetMonthName(int mm)
        {
            switch (mm)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
            }
            return "0";
        }
        public IActionResult InstituteTimingsReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            var instituteTimingsList = myDbContext.InstituteTimings.Where(x => x.InstituteId == Convert.ToInt32(HttpContext.Session.GetString("InstituteID"))).ToList();

            return View(instituteTimingsList);
        }
        public IActionResult AddInstituteTimings()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInstitute(int id)
        {
            var Institutetimeing = await myDbContext.InstituteTimings.FindAsync(id);
            if (Institutetimeing == null)
            {
                return Json(new { success = false, message = "Institute not found!" });
            }
            myDbContext.InstituteTimings.Remove(Institutetimeing);
            myDbContext.SaveChanges();
            return Json(new { success = true, message = "Institute deleted successfully!" });
        }
        public IActionResult SaveInstituteTimings()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            string instituteID = HttpContext.Session.GetString("InstituteID");
            //duplicate validation
            if (myDbContext.InstituteTimings.Any(x => x.InstituteId == Convert.ToInt32(instituteID) && x.StartTime == Request.Form["starttime"] + " " + Request.Form["StartAMPM"] && x.EndTime == Request.Form["endtime"] + " " + Request.Form["EndAMPM"]))
            {
                TempData["ErrorMsg"] = "Institute Timings already exists!";
                return RedirectToAction("AddInstituteTimings");
            }
            string startime = Request.Form["starttime"] + " " + Request.Form["StartAMPM"];
            string endtime = Request.Form["endtime"] + " " + Request.Form["EndAMPM"];
            InstituteTimings instituteTimings = new InstituteTimings();
            instituteTimings.StartTime = startime;
            instituteTimings.EndTime = endtime;
            instituteTimings.InstituteId = Convert.ToInt32(instituteID);
            myDbContext.InstituteTimings.Add(instituteTimings);
            myDbContext.SaveChanges();

            return RedirectToAction("InstituteTimingsReport");
        }
        public IActionResult CoursefeeReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            int Instituteid = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var coursefeeList = (from a in myDbContext.Course
                                 join b in myDbContext.Coursefee on a.Id equals b.CourseId
                                 join c in myDbContext.Subject on b.subjectid equals c.Id
                                 where b.Instituteid == Instituteid
                                 select new CoursefeeVM { Id = b.Id, Course = a.CourseName, Fees = b.Fees, Subject = c.SubjectName }).ToList();

            return View(coursefeeList);
        }
        public IActionResult AddCoursefee()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

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
                list2.Add(new SelectListItem { Text = Model.StudentTypeDesc, Value = Model.Id.ToString() });
            }
            ViewBag.StudentTypeList = list2;

            List<SelectListItem> list3 = new List<SelectListItem>();
            list3.Add(new SelectListItem { Text = "", Value = "0" });
            ViewBag.SubjectList = list3;

            return View();
        }
        public IActionResult SaveCoursefee()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            //duplicate coursefees
            var courseExists = myDbContext.Coursefee
                .Where(x => x.CourseId == Convert.ToInt32(Request.Form["Course"])
                 && x.StudentType == Request.Form["StudentType"].ToString()
                 && x.subjectid == Convert.ToInt32(Request.Form["Subject"])
                 && x.Instituteid == instituteId)
                .FirstOrDefault();
            if (courseExists != null)
            {
                TempData["ErrorMsg"] = "Course fee already exists for this course, student type and subject!";
                return RedirectToAction("AddCoursefee");
            }
            int course = Convert.ToInt32(Request.Form["Course"]);
            int studenttype = Convert.ToInt32(Request.Form["StudentType"]);
            int fees = Convert.ToInt32(Request.Form["fees"]);
            int subjectid = Convert.ToInt32(Request.Form["Subject"]);
            int Instituteid = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            Coursefee coursefee = new Coursefee();
            coursefee.CourseId = course;
            coursefee.StudentType = Convert.ToString(studenttype);
            coursefee.Fees = fees;
            coursefee.subjectid = subjectid;
            coursefee.Instituteid = Instituteid;
            myDbContext.Coursefee.Add(coursefee);
            myDbContext.SaveChanges(true);
            return RedirectToAction("CoursefeeReport");
        }
        public JsonResult GetSubjects(string courseId)
        {
            var subList = myDbContext.Subject.Where(x => x.CourseId == Convert.ToInt32(courseId)).ToList();

            //List<SelectListItem> list = new List<SelectListItem>();
            //foreach (var Model in subList)
            //{
            //    list.Add(new SelectListItem { Text = Model.SubjectName, Value = Model.Id.ToString() });
            //}
            //ViewBag.SubjectList = list;

            var jsonResult = JsonConvert.SerializeObject(subList);

            return Json(jsonResult);
        }
        public IActionResult GrNumberReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            var grnumberreport = myDbContext.GrNumber.Where(x => x.InstituteId == instituteId).ToList();
            return View(grnumberreport);
        }
        public IActionResult AddGrNumber()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public IActionResult SaveGrNumber()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            string grnumber = Request.Form["grnumber"];
            GrNumber gr = new GrNumber();
            gr.Gr_Number = grnumber;
            gr.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            gr.InstituteId = instituteId;
            myDbContext.GrNumber.Add(gr);
            myDbContext.SaveChanges();
            return RedirectToAction("GrNumberReport");
        }
        public IActionResult StudentReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            int studentRole = Convert.ToInt32(RoleEnums.Student);
            var studentList = (from s in myDbContext.Students
                               join h in myDbContext.Handicap on s.Handicap equals h.Id
                               join g in myDbContext.Gender on s.Gender equals g.Id
                               join us in myDbContext.Users on s.StudentUserName equals us.Username
                               where s.IsDeleted == false && s.Status == "Active"
                               && us.IsActive == true && us.RoleId == studentRole
                               && us.InstituteId == instituteId
                               select new StudentViewVM
                               {
                                   Students = s,
                                   Handicaps = h,
                                   Genders = g
                               }).ToList();


            //ViewBag.ErrorMessage = ""; //TempData["ErrorMessage"];
            return View(studentList);
        }
        public IActionResult AddStudent()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var genderList = myDbContext.Gender.ToList();
            List<SelectListItem> genderlist = new List<SelectListItem>();
            foreach (var Model in genderList)
            {
                genderlist.Add(new SelectListItem { Text = Model.Name, Value = Model.Id.ToString() });
            }
            ViewBag.GenderList = genderlist;


            var studentTypeList = myDbContext.studenttype.ToList();
            List<SelectListItem> StudentTypeList = new List<SelectListItem>();
            foreach (var Model in studentTypeList)
            {
                StudentTypeList.Add(new SelectListItem { Text = Model.StudentType, Value = Model.Id.ToString() });
            }
            ViewBag.StudentTypeList = StudentTypeList;


            var HandicapList = myDbContext.Handicap.ToList();
            List<SelectListItem> handicaplist = new List<SelectListItem>();
            foreach (var Model in HandicapList)
            {
                handicaplist.Add(new SelectListItem { Text = Model.HadicapName, Value = Model.Id.ToString() });
            }
            ViewBag.HandicapList = handicaplist;

            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }
            var sessionList = myDbContext.InstituteSessions.Where(x => x.InstituteId == instituteId).ToList();

            List<SelectListItem> sessionlist = new List<SelectListItem>();
            foreach (var Model in sessionList)
            {
                sessionlist.Add(new SelectListItem { Text = Model.StartSession + Model.startSessionYY + "-" + Model.EndSession + Model.endSessionYY, Value = Model.Id.ToString() });
            }
            ViewBag.SessionList = sessionlist;

            return View();
        }
        public async Task<IActionResult> SaveStudent(StudentVM stu, IFormCollection form)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Index", "Login");
            }

            // Validate duplicate student record
            var existingStudent = myDbContext.Students
                .FirstOrDefault(s => s.StudentUserName == stu.StudentUserName && s.FirstName == stu.FirstName && s.LastName == stu.LastName && s.FatherName == stu.FatherName && s.MobileNumber == stu.MobileNo && s.EmailId == stu.Email);
            if (existingStudent != null)
            {
                TempData["Msg"] = "Student already exists!";
                return RedirectToAction("AddStudent");
            }

            string[] SelectSub30WPM = form["SelectSub30WPM"].ToString().Split(',');
            string[] SelectSub40WPM = form["SelectSub40WPM"].ToString().Split(',');
            var combinedSelection = new List<string>();
            combinedSelection.AddRange(SelectSub30WPM);
            combinedSelection.AddRange(SelectSub40WPM);

            var currentUserId = HttpContext.Session.GetString("UserId");
            var currentUserName = HttpContext.Session.GetString("UserName");
            var instituteId = HttpContext.Session.GetString("InstituteID");

            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("Index", "Login");
            }

            string msg = AddStudentLogin(stu.StudentUserName, stu.StudentPassword, stu.Email);

            if (msg != "Success")
            {
                TempData["Msg"] = msg;
                return RedirectToAction("AddStudent");
            }

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            var studentPicResult = await UploadFile(stu.StudentPicURL, uploadDirectory);

            var photoIdentityResult = await UploadFile(stu.PhotoIdentity, uploadDirectory);

            var identityPicResult = await UploadFile(stu.IdentityPicURL, uploadDirectory);

            var otherIdentityResult = await UploadFile(stu.OtherIdentity, uploadDirectory);
            try
            {
                // Adding Enrolled Subjects with incremental GR No.

                var userId = Convert.ToInt64(currentUserId);
                var instId = Convert.ToInt64(instituteId);
                Int64 maxGrNu = 0;

                var maxGREnrolled = myDbContext.EnrolledSubject.Where(x => x.instituteid == instId).Select(x => x.GRNumber).OrderByDescending(x => x).FirstOrDefault();
                if (maxGREnrolled == null || maxGREnrolled == 0)
                {
                    var maxGrNumber = myDbContext.GrNumber
                    .Where(e => e.InstituteId == instId)
                    .Select(e => e.Gr_Number)
                    .OrderByDescending(e => e)
                    .FirstOrDefault();

                    maxGrNu = Convert.ToInt64(maxGrNumber);
                }
                else
                {
                    maxGrNu = Convert.ToInt64(maxGREnrolled);
                }

                foreach (var subject in combinedSelection)
                {
                    var enrolledSubject = new EnrolledSubject
                    {
                        SubjectName = subject,
                        GRNumber = (int)maxGrNu + 1,
                        UserName = stu.StudentUserName,
                        CreateBy = (int)userId,
                        instituteid = (int)instId
                    };
                    myDbContext.EnrolledSubject.Add(enrolledSubject);
                    maxGrNu++;
                }
                myDbContext.SaveChanges();


                var newStudent = new Students
                {
                    LastName = stu.LastName,
                    FirstName = stu.FirstName,
                    FatherName = stu.FatherName,
                    MotherName = stu.MotherName,
                    MobileNumber = stu.MobileNo,
                    EmailId = stu.Email,
                    Gender = stu.Gender,
                    Handicap = stu.Handicap,
                    PaermentAddress = stu.PaermentAddress,
                    School = stu.School,
                    Education = stu.Education,
                    UID = stu.UID,
                    Cast = stu.Cast,
                    StudentType = stu.StudentType,
                    IdentityNo = stu.IdentityNo,
                    DOB = stu.DOB,
                    DateAdd = stu.DateAdd,
                    SelectSub30wpm = form["SelectSub30WPM"],
                    SelectSub40wpm = form["SelectSub40WPM"],
                    Session = stu.Session,
                    StudentUserName = stu.StudentUserName,
                    StudentPassword = stu.StudentPassword,
                    Status = "Active",
                    IsDeleted = false,
                    StudentPicURL = studentPicResult,
                    PhotoIdentityURL = photoIdentityResult,
                    IdentityPicURL = identityPicResult,
                    OtherIdentityURL = otherIdentityResult
                };

                myDbContext.Students.Add(newStudent);
                await myDbContext.SaveChangesAsync();

                return RedirectToAction("StudentReport");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving student: {ex.Message}");
                return View("Error");
            }
        }
        private async Task<string> UploadFile(IFormFile file, string uploadDirectory)
        {
            try
            {
                var filename = Guid.NewGuid().ToString() + '_' + DateTime.Now.Day + '_' + DateTime.Now.Month + '_' + DateTime.Now.Year + '_' + DateTime.Now.Hour + '_' + DateTime.Now.Minute + '_' + DateTime.Now.Second + '_' + DateTime.Now.Millisecond;
                if (file == null) return null;

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                string fileExtension = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(uploadDirectory, filename + fileExtension);

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    return "A file with the same name already exists.";
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
        [HttpGet]
        public IActionResult ViewStudent(int id)
        {
            var result = myDbContext.Students.Where(e => e.Id == id).Select(e => new { SelectSub30wpm = e.SelectSub30wpm, SelectSub40wpm = e.SelectSub40wpm }).FirstOrDefault();
            ViewBag.SelectSub30wpm = result.SelectSub30wpm;
            ViewBag.SelectSub40wpm = result.SelectSub40wpm;

            var studentList = (from s in myDbContext.Students
                               join h in myDbContext.Handicap on s.Handicap equals h.Id
                               join g in myDbContext.Gender on s.Gender equals g.Id
                               where s.Status == "Active" && s.Id == id
                               select new StudentViewVM
                               {
                                   Students = s,
                                   Handicaps = h,
                                   Genders = g
                               }).FirstOrDefault();

            if (studentList == null)
            {
                return NotFound();
            }
            return View(studentList);
        }
        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            var result = myDbContext.Students.FindAsync(id);
            if (result != null)
            {
                ViewBag.SelectSub30wpm = result.Result.SelectSub30wpm;
                ViewBag.SelectSub40wpm = result.Result.SelectSub40wpm;
                ViewBag.IdentityPicURL = result.Result.IdentityPicURL == null ? "" : result.Result.IdentityPicURL;
                ViewBag.OtherIdentityURL = result.Result.OtherIdentityURL == null ? "" : result.Result.OtherIdentityURL;
                ViewBag.StudentPicURL = result.Result.StudentPicURL == null ? "" : result.Result.StudentPicURL;
                ViewBag.PhotoIdentityURL = result.Result.PhotoIdentityURL == null ? "" : result.Result.PhotoIdentityURL;
            }

            var student = myDbContext.Students
                .Where(s => s.Id == id)
                .Select(s => new StudentEditVM
                {
                    StudentId = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    MobileNo = s.MobileNumber,
                    Email = s.EmailId,
                    FatherName = s.FatherName,
                    MotherName = s.MotherName,
                    Handicap = s.Handicap,
                    School = s.School,
                    Education = s.Education,
                    IdentityNo = s.IdentityNo,
                    DateAdd = s.DateAdd,
                    Session = s.Session,
                    StudentUserName = s.StudentUserName,
                    Gender = s.Gender,
                    PaermentAddress = s.PaermentAddress,
                    StudentPicURL = s.StudentPicURL,
                    PhotoIdentity = s.PhotoIdentityURL,
                    OtherIdentity = s.OtherIdentityURL,
                    IdentityPicURL = s.IdentityPicURL,
                    DOB = s.DOB,
                    Status = s.Status
                })
                .FirstOrDefault();

            var HandicapList = myDbContext.Handicap.ToList();
            List<SelectListItem> handicaplist = new List<SelectListItem>();
            foreach (var Model in HandicapList)
            {
                handicaplist.Add(new SelectListItem { Text = Model.HadicapName, Value = Model.Id.ToString() });
            }
            ViewBag.HandicapList = handicaplist;

            var GenderList = myDbContext.Gender.ToList();
            List<SelectListItem> Genderlst = new List<SelectListItem>();
            foreach (var item in GenderList)
            {
                Genderlst.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.GenderList = Genderlst;

            var sessionList = myDbContext.InstituteSessions.ToList();
            List<SelectListItem> sessionlist = new List<SelectListItem>();
            foreach (var Model in sessionList)
            {
                sessionlist.Add(new SelectListItem { Text = Model.StartSession + "-" + Model.EndSession, Value = Model.Id.ToString() });
            }
            ViewBag.SessionList = sessionlist;

            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStudent(StudentEditVM stu)
        {
            #region
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            string studentPic_Name;
            string photoIdentity_Name;
            string identityPic_Name;
            string otherIdentity_Name;
            if (stu.StudentPicURL == null && stu.StudentPic_File != null)
            {
                studentPic_Name = await UploadFile(stu.StudentPic_File, uploadDirectory);
            }
            else
            {
                studentPic_Name = Path.GetFileName(stu.StudentPicURL);
            }

            if (stu.PhotoIdentity == null && stu.PhotoIdentity_File != null)
            {
                photoIdentity_Name = await UploadFile(stu.PhotoIdentity_File, uploadDirectory);
            }
            else
            {
                photoIdentity_Name = Path.GetFileName(stu.PhotoIdentity);
            }
            if (stu.IdentityPicURL == null && stu.IdentityPic_File != null)
            {
                identityPic_Name = await UploadFile(stu.IdentityPic_File, uploadDirectory);
            }
            else
            {
                identityPic_Name = Path.GetFileName(stu.IdentityPicURL);
            }
            if (stu.OtherIdentity == null && stu.OtherIdentity_File != null)
            {
                otherIdentity_Name = await UploadFile(stu.OtherIdentity_File, uploadDirectory);
            }
            else
            {
                otherIdentity_Name = Path.GetFileName(stu.OtherIdentity);
            }

            #endregion
            string SelectSub40wpm = null;
            if (stu.SelectSub40wpm != null)
            {
                SelectSub40wpm = stu.SelectSub40wpm.Replace("subject", "");
            }

            string SelectSub30wpm = null;
            if (stu.SelectSub30wpm != null)
            {
                SelectSub30wpm = stu.SelectSub30wpm.Replace("subject", "");
            }


            var newStudent = await myDbContext.Students.FindAsync(stu.StudentId);

            if (newStudent != null)
            {
                newStudent.Id = stu.StudentId;
                newStudent.LastName = stu.LastName;
                newStudent.FirstName = stu.FirstName;
                newStudent.FatherName = stu.FatherName;
                newStudent.MotherName = stu.MotherName;
                newStudent.MobileNumber = stu.MobileNo;
                newStudent.EmailId = stu.Email;
                newStudent.Gender = stu.Gender;
                newStudent.Handicap = stu.Handicap;
                newStudent.PaermentAddress = stu.PaermentAddress;
                newStudent.School = stu.School;
                newStudent.Education = stu.Education;
                newStudent.IdentityNo = stu.IdentityNo;
                newStudent.DOB = stu.DOB;
                newStudent.DateAdd = stu.DateAdd;
                newStudent.SelectSub30wpm = SelectSub30wpm;
                newStudent.SelectSub40wpm = SelectSub40wpm;
                newStudent.Session = stu.Session;
                newStudent.StudentUserName = stu.StudentUserName;
                newStudent.StudentPassword = stu.StudentPassword;
                newStudent.Status = "Active";
                newStudent.StudentPicURL = studentPic_Name;
                newStudent.PhotoIdentityURL = photoIdentity_Name;
                newStudent.IdentityPicURL = identityPic_Name;
                newStudent.OtherIdentityURL = otherIdentity_Name;
            }

            myDbContext.Entry(newStudent).State = EntityState.Modified;
            await myDbContext.SaveChangesAsync();

            return RedirectToAction("StudentReport");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await myDbContext.Students.FindAsync(id);
            if (student == null)
            {
                return Json(new { success = false, message = "Student not found!" });
            }
            student.IsDeleted = true;
            student.Status = "InActive";

            var user = await myDbContext.Users.FirstOrDefaultAsync(u => u.Username == student.StudentUserName);
            if (user != null)
            {
                user.IsActive = false;
                myDbContext.Users.Update(user);
            }
            myDbContext.Students.Update(student);
            await myDbContext.SaveChangesAsync();

            return Json(new { success = true, message = "Student and related user deleted successfully!" });
        }
        public string AddStudentLogin(string username, string password, string EmailId)
        {
            string errorMsg = string.Empty;
            var userList = myDbContext.Users.Where(x => x.Username.ToLower() == username.ToLower()).ToList();
            if (userList.Count > 0)
            {
                errorMsg = "Username already exists!";
                return errorMsg;
            }

            Users studentlogin = new Users();
            studentlogin.Username = username;
            studentlogin.Password = password;
            studentlogin.IsActive = true;
            studentlogin.RoleId = 1;
            studentlogin.Email = EmailId;
            studentlogin.InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            myDbContext.Users.Add(studentlogin);
            myDbContext.SaveChanges();
            return "Success";
        }
        public string AddInstructorLogin(string username, string password, string EmailId)
        {
            string errorMsg = string.Empty;
            var userList = myDbContext.Users.Where(x => x.Username.ToLower() == username.ToLower()).ToList();
            if (userList.Count > 0)
            {
                errorMsg = "Username already exists!";
                return errorMsg;
            }

            Users studentlogin = new Users();
            studentlogin.Username = username;
            studentlogin.Password = password;
            studentlogin.IsActive = true;
            studentlogin.RoleId = 4;
            studentlogin.Email = EmailId;
            studentlogin.InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            myDbContext.Users.Add(studentlogin);
            myDbContext.SaveChanges();
            return "Success";
        }
        public IActionResult InstructorReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var report1 = (from inst in myDbContext.Instructor
                           join gd in myDbContext.Gender on Convert.ToInt32(inst.Gender) equals gd.Id
                           where inst.InstituteId == Convert.ToInt32(HttpContext.Session.GetString("InstituteID"))
                           select new InstructorVM
                           {
                               InstructorFirstName = inst.InstructorFirstName,
                               MobileNo = inst.MobileNo,
                               Gender = gd.Name,
                               Status = inst.Status
                           }).ToList();

            return View(report1);
        }
        public IActionResult AddInstructor()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var genderList = myDbContext.Gender.ToList();
            List<SelectListItem> genderlist = new List<SelectListItem>();
            foreach (var Model in genderList)
            {
                genderlist.Add(new SelectListItem { Text = Model.Name, Value = Model.Id.ToString() });
            }
            ViewBag.GenderList = genderlist;

            return View();
        }
        public async Task<IActionResult> SavInstructor(InstructorVM instructor, IFormCollection form)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InstructorImage");

            var InstructorPicResult = await UploadFile(instructor.Identity, uploadDirectory);
            if (InstructorPicResult.Contains("already exists"))
            {
                TempData["ErrorMessage"] = "The student picture file already exists. Please upload a different file.";
                return RedirectToAction("StudentReport");
            }

            var InstructorPic = await UploadFile(instructor.Pic, uploadDirectory);
            if (InstructorPic.Contains("already exists"))
            {
                TempData["ErrorMessage"] = "The photo identity file already exists. Please upload a different file.";
                return RedirectToAction("StudentReport");
            }

            var inst = new Instructor()
            {
               
                LastName = instructor.LastName,
                InstructorFirstName = instructor.InstructorFirstName,
                FatherName = instructor.FatherName,
                MotherName = instructor.MotherName,
                MobileNo = instructor.MobileNo,
                Email = instructor.Email,
                InstructorUserName = instructor.InstructorUserName,
                InstructorPassword = instructor.InstructorPassword,
                Gender = instructor.Gender,
                PermanentAddress = instructor.PermanentAddress,
                Education = instructor.Education,
                IdentityNo = instructor.IdentityNo,
                Identity = InstructorPicResult,
                Pic = InstructorPic,
                InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID")),
                Status = "Active",
            };

            AddInstructorLogin(instructor.InstructorUserName, instructor.InstructorPassword, instructor.Email);
            //duplicate validation
            if (myDbContext.Instructor.Any(x => x.InstructorUserName == inst.InstructorUserName && x.InstituteId == inst.InstituteId && x.InstructorFirstName== inst.InstructorFirstName && x.MobileNo==inst.MobileNo))
            {
                TempData["ErrorMessage"] = "Instructor already exists!";
                return RedirectToAction("AddInstructor");
            }
            myDbContext.Instructor.Add(inst);
            myDbContext.SaveChanges();
            return RedirectToAction("InstructorReport");
        }
        public IActionResult HallTicketReport()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            var report = myDbContext.HallTicket.ToList();
            return View(report);
        }
        public IActionResult AddHallTicket()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            var studentsList = (from i in myDbContext.Students
                                join ins in myDbContext.Users
                                on i.StudentUserName equals ins.Username
                                where ins.InstituteId == instituteId
                                && i.Session == HttpContext.Session.GetString("CurrentSessionId")
                                && ins.IsActive == true
                                select i).ToList();

            List<SelectListItem> studentlist = new List<SelectListItem>();
            foreach (var Model in studentsList)
            {
                studentlist.Add(new SelectListItem { Text = Model.FirstName, Value = Model.StudentUserName });
            }
            ViewBag.StudentList = studentlist;

            List<SelectListItem> list2 = new List<SelectListItem>();
            var subList = myDbContext.Subject.ToList();
            foreach (var Model in subList)
            {
                list2.Add(new SelectListItem { Text = Model.SubjectName, Value = Model.SubjectName.ToString() });
            }
            ViewBag.StudentSubList = list2;

            return View();
        }
        public IActionResult SaveHallTicket()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            string StudentName = Request.Form["StudentName"];
            string subjectName = Request.Form["StudentSub"];
            string CenterName = Request.Form["CenterName"];
            string ExamTime = Request.Form["ExamTime"];

            Int64 gr = myDbContext.EnrolledSubject
                .Where(x => x.SubjectName == subjectName && x.instituteid == instituteId && x.UserName == StudentName)
                .Select(x => x.GRNumber).FirstOrDefault();


            HallTicket ht = new HallTicket();
            ht.GrNumber = gr.ToString();
            ht.StudentName = StudentName;
            ht.StudentSub = subjectName;
            ht.CenterName = CenterName;
            ht.ExamTime = Convert.ToDateTime(ExamTime);
            myDbContext.HallTicket.Add(ht);
            myDbContext.SaveChanges();

            return RedirectToAction("HallTicketReport");
        }
        public IActionResult TakeInstallment()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            int studentRole = Convert.ToInt32(RoleEnums.Student);
            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            var studentsList = (from i in myDbContext.Students
                                join ins in myDbContext.Users
                                on i.StudentUserName equals ins.Username
                                where ins.InstituteId == instituteId
                                && i.Session == HttpContext.Session.GetString("CurrentSessionId")
                                select i).ToList();

            List<SelectListItem> stlist = new List<SelectListItem>();
            foreach (var Model in studentsList)
            {
                stlist.Add(new SelectListItem { Text = Model.StudentUserName, Value = Model.StudentUserName });
            }
            ViewBag.StudentList = stlist;

            var subList = myDbContext.Subject.ToList();
            ViewBag.SubjectsList = subList;

            var receipt = myDbContext.Receipts.OrderByDescending(x => x.Id).FirstOrDefault();

            ReceiptVM receiptVM = new ReceiptVM();
            receiptVM.ReceiptNo = receipt == null ? 1 : receipt.Id + 1;
            receiptVM.InstallmentDate = DateTime.Now.Date;

            var courseList = myDbContext.Course.ToList();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var Model in courseList)
            {
                list.Add(new SelectListItem { Text = Model.CourseName, Value = Model.Id.ToString() });
            }
            ViewBag.CourseList = list;

            return View(receiptVM);
        }
        public IActionResult InstructorPayment()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var instructorsList = myDbContext.Users.Where(x => x.IsActive == true && x.RoleId == 4).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var Model in instructorsList)
            {
                list.Add(new SelectListItem { Text = Model.Username, Value = Model.Username });
            }
            ViewBag.InstructorList = list;


            var payment = myDbContext.Payment.OrderByDescending(x => x.Id).FirstOrDefault();
            PaymentVM paymentVM = new PaymentVM();
            paymentVM.PaymentID = payment == null ? 1 : payment.Id + 1;

            return View(paymentVM);
        }
        public IActionResult TakeAmount()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            string paymentDate = Request.Form["InstallmentDate"];
            string UserName = Request.Form["ddlStudentUserName"];
            int? userId = myDbContext.Users.Where(x => x.Username == UserName).Select(x => x.Id).FirstOrDefault();
            string AmountPaid = Request.Form["AmountPaid"];
            string ddlPaymentMadeBy = Request.Form["ddlPaymentMadeBy"];
            string ChequeNo = Request.Form["ChequeNo"];
            string ChequeDate = Request.Form["ChequeDate"];
            string Course = Request.Form["Course"];
            string nextInstallmentDate = Request.Form["NextInstallmentDate"];

            var currentSessionId = Convert.ToInt32(HttpContext.Session.GetString("CurrentSessionId"));

            Payment pay = new Payment();
            pay.PaymentDate = Convert.ToDateTime(paymentDate);
            pay.UserId = userId;
            pay.Amount = Convert.ToDecimal(AmountPaid);
            pay.PaymentMode = ddlPaymentMadeBy;
            if (!string.IsNullOrEmpty(ChequeNo))
            {
                pay.ChequeNo = ChequeNo;
                pay.ChequeDate = Convert.ToDateTime(ChequeDate);
            }
            pay.CreatedAt = DateTime.Now;
            pay.CourseId = Convert.ToInt32(Course);
            pay.NextInstallmentDate = Convert.ToDateTime(nextInstallmentDate);
            pay.SessionId = currentSessionId;

            myDbContext.Payment.Add(pay);
            myDbContext.SaveChanges();

            Receipts receipts = new Receipts();
            receipts.InstallmentDate = Convert.ToDateTime(paymentDate);
            int? studType = myDbContext.Students.Where(x => x.StudentUserName == UserName).Select(x => x.StudentType).FirstOrDefault();
            receipts.StudentType = Convert.ToString(studType);
            receipts.StudentUserName = UserName;

            //string subjects = Request.Form["chkSubjects"];
            string subjects = myDbContext.Students.Where(x => x.StudentUserName == UserName).Select(x => x.SelectSub30wpm + "," + x.SelectSub40wpm).FirstOrDefault();
            string[] subs = subjects.Split(',');
            string subjectIds = string.Empty;
            foreach (string sub in subs)
            {
                subjectIds = subjectIds + myDbContext.Subject.Where(x => x.SubjectName == sub).Select(x => x.Id).FirstOrDefault() + ",";
            }
            subjectIds.TrimEnd(',');
            receipts.SubjectIds = subjectIds;

            string TotalAmountDue = Request.Form["TotalAmountDue"];
            receipts.TotalAmountDue = Convert.ToDecimal(TotalAmountDue);
            receipts.AmountPaid = Convert.ToDecimal(AmountPaid);
            receipts.BalanceAmountDue = (Convert.ToDecimal(TotalAmountDue) - Convert.ToDecimal(AmountPaid));
            receipts.PaymentMadeBy = ddlPaymentMadeBy;
            if (!string.IsNullOrEmpty(ChequeNo))
            {
                receipts.ChequeNo = ChequeNo;
                receipts.ChequeDate = Convert.ToDateTime(ChequeDate);
            }
            receipts.CreatedAt = DateTime.Now;
            receipts.SessionId = currentSessionId;

            myDbContext.Receipts.Add(receipts);
            myDbContext.SaveChanges();

            //return RedirectToAction("TakeInstallment");
            return PaymentReceipt(receipts);
        }
        public IActionResult PaymentReceipt(Receipts receipts)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var studentDetail = myDbContext.Users.Where(x => x.Username == receipts.StudentUserName).FirstOrDefault();
            var instituteDetail = myDbContext.Institute.Where(x => x.Id == studentDetail.InstituteId).FirstOrDefault();
            var studentData = myDbContext.Students.Where(x => x.StudentUserName == receipts.StudentUserName).FirstOrDefault();

            ReceiptVM receiptVM = new ReceiptVM();
            receiptVM.ReceiptNo = receipts.Id;
            receiptVM.InstituteName = instituteDetail.InstituteName;
            receiptVM.InstituteAddress = instituteDetail.InstituteAddress;
            receiptVM.InstallmentDate = receipts.InstallmentDate;
            receiptVM.StudentName = studentData.FirstName + " " + studentData.LastName;
            receiptVM.FatherName = studentData.FatherName;
            receiptVM.AmountPaid = receipts.AmountPaid;
            receiptVM.TotalAmountDue = receipts.TotalAmountDue;
            receiptVM.BalanceAmountDue = receipts.BalanceAmountDue;

            return View("PaymentReceipt", receiptVM);
        }
        public IActionResult ExportToExcelGR()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            var GRDetails = from s in myDbContext.Students
                            join e in myDbContext.EnrolledSubject
                                on s.Id equals e.EnrolledSubjectId into seGroup
                            join usr in myDbContext.Users
                                on s.StudentUserName equals usr.Username
                            where usr.InstituteId == instituteId
                            from e in seGroup.DefaultIfEmpty()
                            select new GRDetailsExport
                            {
                                Id = s.Id,
                                GeneralRegNo = e != null ? e.GRNumber : null,
                                UID = s.UID,
                                AddmissionDate = s.DateAdd,
                                Subject = e.SubjectName,
                                FirstName = s.FirstName,
                                FatherName = s.FatherName,
                                Surname = s.LastName,
                                MotherName = s.MotherName,
                                Cast = s.Cast,
                                CityVillage = s.PaermentAddress,
                                TalukaDistrict = "",
                                DateOfBirth = s.DOB,
                                Education = s.Education,
                                SchoolCollegeName = s.School,
                                Remark = ""

                            };

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet();

            // Inserts the collection to Excel as a table with a header row.
            ws.Cell("A2").InsertTable(GRDetails);
            ws.Range("B1:I1").Merge();
            ws.Range("B1:I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("B1:I1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(1, 2).Value = "Basic Information";
            ws.Range("A1:A2").Merge();
            ws.Range("A1:A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("A1:A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("K1:L1").Merge();
            ws.Range("K1:L1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("K1:L1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(1, 11).Value = "Address";
            ws.Range("P1:P2").Merge();
            ws.Range("P1:P2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("P1:P2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(1, 1).Value = "Student ID";
            ws.Cell(1, 16).Value = "Remarks";
            ws.Columns("A:P").Width = 20; // Set width for columns A to C
            ws.Rows("1").Height = 25;      // Set height for the first row
            ws.Rows("2").Height = 25;      // Set height for the first row
            ws.Column("1").Width = 50;
            ws.Column("16").Width = 30;
            ws.Range("A1:P1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("A2:P2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("A1:P1").Style.Font.Bold = true;
            ws.Range("A2:P2").Style.Font.Bold = true;
            // Adjust column size to contents.
            ws.Columns().AdjustToContents();

            // Save to local file system.
            var filename = $"Export - {DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(content, contentType, filename);
        }
        public async Task<IActionResult> StudentAttendance()
        {
            TempData["DashId"] = HttpContext.Session.GetString("DashId");
            return View();
        }
        public async Task<IActionResult> DownloadStudentAttendance()
        {
            // Get institute name from session
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            var InstituteName = HttpContext.Session.GetString("InstituteName");
            var InstId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            var currentSessionId = Convert.ToInt32(HttpContext.Session.GetString("CurrentSessionId"));

            string fromDate = Request.Form["fromdate"].ToString();

            var students = (from us in myDbContext.Users
                            where us.InstituteId == InstId && us.RoleId == 1 && us.IsActive == true
                            select us).ToList();

            ExcelPackage.License.SetNonCommercialPersonal("DSequence"); // Set the license context for EPPlus
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("AttendanceReport");

                worksheet.Cells[1, 1, 1, 2].Value = "Report Month";
                worksheet.Cells[1, 1, 1, 2].Merge = true;
                worksheet.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 3, 1, 10].Value = Convert.ToDateTime(fromDate).ToString("MMM") + " - " + Convert.ToDateTime(fromDate).Year;
                worksheet.Cells[1, 3, 1, 10].Merge = true;
                worksheet.Cells[1, 3, 1, 10].Style.Font.Bold = true;
                worksheet.Cells[1, 3, 1, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 11, 1, 15].Value = "Institute Name";
                worksheet.Cells[1, 11, 1, 15].Merge = true;
                worksheet.Cells[1, 11, 1, 15].Style.Font.Bold = true;
                worksheet.Cells[1, 11, 1, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                worksheet.Cells[1, 16, 1, 25].Value = InstituteName;
                worksheet.Cells[1, 16, 1, 25].Merge = true;
                worksheet.Cells[1, 16, 1, 25].Style.Font.Bold = true;
                worksheet.Cells[1, 16, 1, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                int row = 2;
                foreach (var st in students)
                {
                    int iPresent = 0;
                    int iAbsent = 0;

                    worksheet.Cells[row, 2, row, 7].Value = st.Username;
                    worksheet.Cells[row, 2, row, 7].Merge = true;
                    worksheet.Cells[row, 2, row, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 2, row, 7].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 7].Style.Font.Size = 12;
                    worksheet.Cells[row, 2, row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int rowPresent = row;
                    int rowAbsent = row;

                    worksheet.Cells[row, 8, row, 14].Value = "Present";
                    worksheet.Cells[row, 8, row, 14].Merge = true;
                    worksheet.Cells[row, 15, row, 15].Value = 0;
                    worksheet.Cells[row, 15, row, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[row, 16, row, 23].Value = "Absent";
                    worksheet.Cells[row, 16, row, 23].Merge = true;
                    worksheet.Cells[row, 24, row, 24].Value = 31;
                    worksheet.Cells[row, 24, row, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    row++;

                    worksheet.Cells[8, 1, 8, 1].Value = "Status";
                    worksheet.Cells[8, 1, 8, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int maxDays = DateTime.DaysInMonth(Convert.ToDateTime(fromDate).Year, Convert.ToDateTime(fromDate).Month);

                    worksheet.Cells[row, 1, row, 1].Value = "";
                    worksheet.Cells[row, 1, row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int col1 = 2;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        worksheet.Cells[row, col1, row, col1].Value = i;
                        worksheet.Cells[row, col1, row, col1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        col1++;
                    }

                    row++;

                    worksheet.Cells[row, 1, row, 1].Value = "";
                    worksheet.Cells[row, 1, row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int col2 = 2;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        // Get all days of a month
                        DateTime date = new DateTime(Convert.ToDateTime(fromDate).Year, Convert.ToDateTime(fromDate).Month, i);
                        worksheet.Cells[row, col2, row, col2].Value = date.ToString("ddd");
                        worksheet.Cells[row, col2, row, col2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        col2++;
                    }

                    row++;

                    worksheet.Cells[row, 1, row, 1].Value = "IN";
                    worksheet.Cells[row, 1, row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int col3 = 2;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        DateTime date = new DateTime(Convert.ToDateTime(fromDate).Year, Convert.ToDateTime(fromDate).Month, i);
                        var inTime = myDbContext.UserLogins.Where(x => x.UserId == st.Id
                        && (x.Login.Date.Day == date.Day && x.Login.Date.Month == date.Month && x.Login.Date.Year == date.Year))
                            .Select(x => x.Login.ToShortTimeString()).FirstOrDefault();

                        worksheet.Cells[row, col3, row, col3].Value = inTime;
                        worksheet.Cells[row, col3, row, col3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        col3++;
                    }

                    row++;

                    worksheet.Cells[row, 1, row, 1].Value = "OUT";
                    worksheet.Cells[row, 1, row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int col4 = 2;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        DateTime date = new DateTime(Convert.ToDateTime(fromDate).Year, Convert.ToDateTime(fromDate).Month, i);
                        var outTime = myDbContext.UserLogins.Where(x => x.UserId == st.Id
                        && (x.LogOut.Date.Day == date.Day && x.LogOut.Date.Month == date.Month && x.LogOut.Date.Year == date.Year))
                            .Select(x => x.LogOut.ToShortTimeString()).FirstOrDefault();

                        worksheet.Cells[row, col4, row, col4].Value = outTime;
                        worksheet.Cells[row, col4, row, col4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        col4++;
                    }

                    row++;

                    worksheet.Cells[row, 1, row, 1].Value = "Work";
                    worksheet.Cells[row, 1, row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    int col5 = 2;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        DateTime date = new DateTime(Convert.ToDateTime(fromDate).Year, Convert.ToDateTime(fromDate).Month, i);

                        var inTime = myDbContext.UserLogins.Where(x => x.UserId == st.Id
                        && (x.Login.Date.Day == date.Day && x.Login.Date.Month == date.Month && x.Login.Date.Year == date.Year))
                            .Select(x => x.Login.ToShortTimeString()).FirstOrDefault();

                        var outTime = myDbContext.UserLogins.Where(x => x.UserId == st.Id
                        && (x.LogOut.Date.Day == date.Day && x.LogOut.Date.Month == date.Month && x.LogOut.Date.Year == date.Year))
                            .Select(x => x.LogOut.ToShortTimeString()).FirstOrDefault();

                        var work = outTime != null && inTime != null ? Convert.ToDateTime(outTime).Subtract(Convert.ToDateTime(inTime)).TotalMinutes : 0;

                        worksheet.Cells[row, col5, row, col5].Value = work;
                        worksheet.Cells[row, col5, row, col5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        col5++;
                    }

                    row++;

                    int col6 = 2;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        DateTime date = new DateTime(Convert.ToDateTime(fromDate).Year, Convert.ToDateTime(fromDate).Month, i);

                        var inTime = myDbContext.UserLogins.Where(x => x.UserId == st.Id
                        && (x.Login.Date.Day == date.Day && x.Login.Date.Month == date.Month && x.Login.Date.Year == date.Year))
                            .Select(x => x.Login.ToShortTimeString()).FirstOrDefault();

                        if (inTime != null)
                        {
                            iPresent++;
                            worksheet.Cells[row, col6, row, col6].Value = "P";
                            worksheet.Cells[row, col6, row, col6].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                            worksheet.Cells[row, col6, row, col6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }
                        else
                        {
                            iAbsent++;
                            worksheet.Cells[row, col6, row, col6].Value = "A";
                            worksheet.Cells[row, col6, row, col6].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, col6, row, col6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        col6++;
                    }

                    row++;

                    int col7 = 1;
                    for (int i = 1; i <= maxDays; i++)
                    {
                        worksheet.Cells[row, col7, row, col7].Style.Fill.SetBackground(System.Drawing.Color.Gray);
                        col7++;
                    }

                    row++;

                    worksheet.Cells[rowPresent, 15, rowPresent, 15].Value = iPresent;
                    worksheet.Cells[rowAbsent, 24, rowAbsent, 24].Value = iAbsent;

                }


                // save & download the file
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "AttendanceFile", "studentattendance.xlsx");
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await package.SaveAsAsync(stream);
                }
                // Return the file as a download
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentAttendance.xlsx");

            }

        }
        public async Task<IActionResult> TypingTestReport()
        {
            return View();
        }
        public async Task<IActionResult> StudentGrowthReport()
        {
            var getStudent = myDbContext.Students.ToList();
            return View(getStudent);
        }
        public async Task<IActionResult> TotalStudentsReports()
        {
            var InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var currentSessionId = Convert.ToInt32(HttpContext.Session.GetString("CurrentSessionId"));

            TempData["DashId"] = HttpContext.Session.GetString("DashId");

            var getsession = myDbContext.InstituteSessions.Where(x => x.Id == currentSessionId).FirstOrDefault();
            ViewBag.startsession = getsession.StartSession;
            ViewBag.endsession = getsession.EndSession;

            var students = (from i in myDbContext.Students
                            join ins in myDbContext.Users
                            on i.StudentUserName equals ins.Username
                            where ins.InstituteId == InstituteId
                            && i.Session == HttpContext.Session.GetString("CurrentSessionId")
                            select i).ToList();
            return View(students);
        }
        public async Task<IActionResult> ViewStudentGrowthDetails(int id)
        {
            StudentGrowthDetails studentGrowthDetails = new StudentGrowthDetails();

            try
            {
                var InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                var getsession = await myDbContext.InstituteSessions.FindAsync(InstituteId);

                if (getsession != null)
                {
                    ViewBag.startsession = getsession.StartSession;
                    ViewBag.endsession = getsession.EndSession;
                }

                var Practicedata = await (from tr in myDbContext.TypingResult
                                          join p in myDbContext.coursepractices on tr.PracticeId equals p.Id
                                          where tr.StudentId == id
                                          select new
                                          {
                                              tr.PracticeId,
                                              p.PracticeName,
                                          })
                                           .Distinct()
                                           .ToListAsync();

                var Subjectdata = await (from tr in myDbContext.TypingResult
                                         join s in myDbContext.Subject on tr.SubjectId equals s.Id
                                         where tr.StudentId == id
                                         select new
                                         {
                                             tr.SubjectId,
                                             s.SubjectName
                                         })
                                          .Distinct()
                                          .ToListAsync();

                studentGrowthDetails.PracticeList = Practicedata
                    .Select(p => new TypingResultVM
                    {
                        PracticeId = p.PracticeId,
                        PracticName = p.PracticeName
                    })
                    .ToList();


                studentGrowthDetails.SubjectList = Subjectdata
                    .Select(s => new TypingResultVM
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.SubjectName,
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching student growth details.", ex);
            }

            return View(studentGrowthDetails);
        }
        public async Task<IActionResult> ViewPracticesDetails(int id)
        {
            StudentGrowthDetails studentGrowthDetails = new StudentGrowthDetails();

            try
            {
                var InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                var getsession = await myDbContext.InstituteSessions.FindAsync(InstituteId);

                if (getsession != null)
                {
                    ViewBag.startsession = getsession.StartSession;
                    ViewBag.endsession = getsession.EndSession;
                }

                var Practicedata = await (from tr in myDbContext.TypingResult
                                          join p in myDbContext.coursepractices on tr.PracticeId equals p.Id
                                          where tr.StudentId == id
                                          select new
                                          {
                                              tr.PracticeId,
                                              p.PracticeName,
                                          })
                                           .Distinct()
                                           .ToListAsync();

                var Subjectdata = await (from tr in myDbContext.TypingResult
                                         join s in myDbContext.Subject on tr.SubjectId equals s.Id
                                         where tr.StudentId == id
                                         select new
                                         {
                                             tr.SubjectId,
                                             s.SubjectName
                                         })
                                          .Distinct()
                                          .ToListAsync();

                studentGrowthDetails.PracticeListshow = Practicedata
                    .Select(p => new TypingResultVM
                    {
                        PracticeId = p.PracticeId,
                        PracticName = p.PracticeName
                    })
                    .ToList();

                studentGrowthDetails.SubjectListshow = Subjectdata
                    .Select(s => new TypingResultVM
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.SubjectName,
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching student growth details.", ex);
            }

            return View(studentGrowthDetails);
        }

        [HttpGet]
        public async Task<IActionResult> GetInstallments()
        {
            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }

            InstallmentDetailsVM installmentDetailsVM = new InstallmentDetailsVM();
            var sessionList = myDbContext.InstituteSessions.Where(x => x.InstituteId == instituteId).ToList();

            List<SelectListItem> sessionlist = new List<SelectListItem>();
            foreach (var Model in sessionList)
            {
                sessionlist.Add(new SelectListItem { Text = Model.StartSession + Model.startSessionYY + "-" + Model.EndSession + Model.endSessionYY, Value = Model.Id.ToString() });
            }
            TempData["SessionList"] = sessionlist;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetInstallments(InstallmentDetailsVM receipts)
        {
            int instituteId = 0;
            if (HttpContext.Session.GetString("InstituteID") != null && HttpContext.Session.GetString("InstituteID") != "")
            {
                instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            }
            InstallmentDetailsVM installmentDetailsVM = new InstallmentDetailsVM();

            if (!string.IsNullOrEmpty(Request.Form["SearchSession"]))
            {
                int sessionId = Convert.ToInt32(Request.Form["SearchSession"]);

                if (TempData["SessionList"] == null)
                {
                    var sessionList = myDbContext.InstituteSessions.Where(x => x.InstituteId == instituteId).ToList();
                    List<SelectListItem> sessionlist = new List<SelectListItem>();
                    foreach (var Model in sessionList)
                    {
                        sessionlist.Add(new SelectListItem { Text = Model.StartSession + Model.startSessionYY + "-" + Model.EndSession + Model.endSessionYY, Value = Model.Id.ToString() });
                    }
                    TempData["SessionList"] = sessionlist;
                }

                int studentRole = Convert.ToInt32(RoleEnums.Student);
                string studentType = Convert.ToInt32(StudentTypeEnum.RegularStudent).ToString();

                var recps = (from rec in myDbContext.Receipts
                             join us in myDbContext.Users on rec.StudentUserName equals us.Username
                             where us.IsActive == true && us.RoleId == studentRole
                             && us.InstituteId == instituteId && rec.StudentType == studentType
                             && rec.SessionId == sessionId
                             select rec).ToList();

                installmentDetailsVM.ReceiptsList = recps;
            }
            else
            {
                if (TempData["SessionList"] == null)
                {
                    var sessionList = myDbContext.InstituteSessions.Where(x => x.InstituteId == instituteId).ToList();
                    List<SelectListItem> sessionlist = new List<SelectListItem>();
                    foreach (var Model in sessionList)
                    {
                        sessionlist.Add(new SelectListItem { Text = Model.StartSession + Model.startSessionYY + "-" + Model.EndSession + Model.endSessionYY, Value = Model.Id.ToString() });
                    }
                    TempData["SessionList"] = sessionlist;
                }
            }

            return View(installmentDetailsVM);
        }
        public async Task<IActionResult> GetShowPaidInstallmentDetails(int id)
        {
            //InstallmentDetailsVM installmentDetailsVM = new InstallmentDetailsVM();


            //installmentDetailsVM.PaidCourseFeeList = await (from tr in myDbContext.Receipts
            //                                                join s in myDbContext.Students
            //                                                on tr.StudentUserName equals s.StudentUserName
            //                                                where tr.Id == id &&
            //                                                tr.StudentUserName == s.StudentUserName
            //                                                select new Receipts
            //                                                {
            //                                                    Id = tr.Id,
            //                                                    AmountPaid = tr.AmountPaid,
            //                                                    ChequeDate = tr.ChequeDate,
            //                                                    ChequeNo = tr.ChequeNo,
            //                                                    PaymentMadeBy = tr.PaymentMadeBy,
            //                                                    StudentUserName = tr.StudentUserName
            //                                                })
            //                                                .Distinct()
            //                                                .ToListAsync();

            //string strStudentUserName = installmentDetailsVM.PaidCourseFeeList
            //    .Select(x => x.StudentUserName)
            //    .FirstOrDefault();

            //var data = await myDbContext.Receipts
            //     .Where(tr => tr.StudentUserName == strStudentUserName)
            //     .SumAsync(tr => Convert.ToDecimal(tr.AmountPaid));
            //ViewBag.Totalpaid = data;

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var instituteDetail = myDbContext.Institute.Where(x => x.Id == instituteId).FirstOrDefault();

            var receiptDetails = myDbContext.Receipts.Where(x => x.Id == id).FirstOrDefault();
            var studentData = myDbContext.Students.Where(x => x.StudentUserName == receiptDetails.StudentUserName).FirstOrDefault();

            ReceiptVM receiptVM = new ReceiptVM();
            receiptVM.ReceiptNo = receiptDetails.Id;
            receiptVM.InstituteName = instituteDetail.InstituteName;
            receiptVM.InstituteAddress = instituteDetail.InstituteAddress;
            receiptVM.InstallmentDate = receiptDetails.InstallmentDate;
            receiptVM.StudentName = studentData.FirstName + " " + studentData.LastName;
            receiptVM.FatherName = studentData.FatherName;
            receiptVM.AmountPaid = receiptDetails.AmountPaid;
            receiptVM.TotalAmountDue = receiptDetails.TotalAmountDue;
            receiptVM.BalanceAmountDue = receiptDetails.BalanceAmountDue;


            //return View(installmentDetailsVM);
            return View("PaymentReceipt", receiptVM);
        }

        public async Task<IActionResult> ProfileDetails()
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }
                string currentUser = HttpContext.Session.GetString("UserName");

                var studentData = myDbContext.Students
                    .Where(s => s.StudentUserName == currentUser)
                    .FirstOrDefault();
                var UserDetails = myDbContext.Users
                   .Where(s => s.Username == studentData.StudentUserName)
                   .FirstOrDefault();

                if (studentData == null)
                {
                    TempData["ErrorMessage"] = "Student profile not found.";
                    return RedirectToAction("Error");
                }
                var instituteData = myDbContext.Institute
                    .Where(i => i.Id == UserDetails.InstituteId)
                    .FirstOrDefault();

                if (instituteData == null)
                {
                    TempData["ErrorMessage"] = "Institute information not found.";
                    return RedirectToAction("Error");
                }

                var studentProfileViewModel = new StudentProfileDetailsVM
                {
                    FirstName = studentData.FirstName,
                    LastName = studentData.LastName,
                    FatherName = studentData.FatherName,
                    MotherName = studentData.MotherName,
                    MobileNumber = studentData.MobileNumber,
                    EmailId = studentData.EmailId,
                    Gender = myDbContext.Gender.Where(x => x.Id == Convert.ToInt32(studentData.Gender)).Select(x => x.Name).FirstOrDefault(),
                    Handicap = myDbContext.Handicap.Where(x => x.Id == Convert.ToInt32(studentData.Handicap)).Select(x => x.HadicapName).FirstOrDefault(),
                    PaermentAddress = studentData.PaermentAddress,
                    School = studentData.School,
                    Education = studentData.Education,
                    PhotoIdentityURL = studentData.PhotoIdentityURL,
                    OtherIdentityURL = studentData.OtherIdentityURL,
                    IdentityNo = studentData.IdentityNo,
                    DOB = studentData.DOB,
                    DateAdd = studentData.DateAdd,
                    SelectSub30wpm = studentData.SelectSub30wpm,
                    SelectSub40wpm = studentData.SelectSub40wpm,
                    InstituteName = instituteData.InstituteName,
                    InstituteAddress = instituteData.InstituteAddress,
                    StudentPicURL = studentData.StudentPicURL.ToLower(),
                    IdentityPicURL = studentData.IdentityPicURL.ToLower()
                };
                return View(studentProfileViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult DeleteInstituteTiming(string id)
        {
            // Delete institute timings

            string errorresult = string.Empty;
            string successresult = string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                errorresult = "Invalid timing ID.";
            }
            int timingId;
            if (!int.TryParse(id, out timingId))
            {
                errorresult = "Invalid timing ID format.";
            }
            var timing = myDbContext.InstituteTimings.Where(x => x.Id == timingId).FirstOrDefault();
            if (timing == null)
            {
                errorresult = "Timing not found.";
            }
            myDbContext.InstituteTimings.Remove(timing);
            myDbContext.SaveChanges();

            if (string.IsNullOrEmpty(errorresult))
            {
                successresult = "Timing deleted successfully.";
                return Json(new { success = true, message = successresult });
            }
            else
            {
                return Json(new { success = false, message = errorresult });
            }
        }
        public JsonResult DeleteCourseFee(string id)
        {
            // Delete institute timings

            string errorresult = string.Empty;
            string successresult = string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                errorresult = "Invalid Course Fee ID.";
            }
            int timingId;
            if (!int.TryParse(id, out timingId))
            {
                errorresult = "Invalid Course Fee ID format.";
            }
            var timing = myDbContext.Coursefee.Where(x => x.Id == timingId).FirstOrDefault();
            if (timing == null)
            {
                errorresult = "Course Fee not found.";
            }
            myDbContext.Coursefee.Remove(timing);
            myDbContext.SaveChanges();

            if (string.IsNullOrEmpty(errorresult))
            {
                successresult = "Course Fee deleted successfully.";
                return Json(new { success = true, message = successresult });
            }
            else
            {
                return Json(new { success = false, message = errorresult });
            }
        }
        public IActionResult UpdateCourseFee()
        {
            int Id = Convert.ToInt32(Request.Form["hdnCourseFeeId"]);
            double courseFee = Convert.ToDouble(Request.Form["fees"]);



            // Update course fees based on Id
            var courseFeeRecord = myDbContext.Coursefee.FirstOrDefault(x => x.Id == Id);
            if (courseFeeRecord != null)
            {
                courseFeeRecord.Fees = courseFee;
                myDbContext.Coursefee.Update(courseFeeRecord);
                myDbContext.SaveChanges();
            }

            return RedirectToAction("CoursefeeReport");
        }
        public IActionResult UpdateTime()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }


            int Id = Convert.ToInt32(Request.Form["hdnTimingId"]);
            string starttime = Request.Form["starttime"];
            string starttimeAMPM = Request.Form["StartAMPM"];

            string endtime = Request.Form["endtime"];
            string EndAMPM = Request.Form["EndAMPM"];

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));


            //dplicate validation
            var existingTiming = myDbContext.InstituteTimings
                .FirstOrDefault(x => x.StartTime == starttime + " " + starttimeAMPM && x.EndTime == endtime + " " + EndAMPM && x.InstituteId == instituteId);

            // Update course fees based on Id
            var timeRecord = myDbContext.InstituteTimings.FirstOrDefault(x => x.Id == Id);
            if (timeRecord != null)
            {
                timeRecord.StartTime = starttime + " " + starttimeAMPM;
                timeRecord.EndTime = endtime + " " + EndAMPM;
                if (existingTiming != null)
                {
                    // If duplicate timing exists, return an error message
                    TempData["ErrorMessage"] = "This timing already exists. Please choose a different time.";
                    return RedirectToAction("InstituteTimingsReport");
                }
                myDbContext.InstituteTimings.Update(timeRecord);
                myDbContext.SaveChanges();
            }

            return RedirectToAction("InstituteTimingsReport");
        }
        public IActionResult UploadMasterKeyboard()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            List<CoursePracticeVM> coursePracticeList = new List<CoursePracticeVM>();
            coursePracticeList = (from cp in myDbContext.coursepractices
                                  join s in myDbContext.Subject on cp.SubjectId equals s.Id
                                  where cp.InstituteId == instituteId
                                  select new CoursePracticeVM
                                  {
                                      Id = cp.Id,
                                      PracticeName = cp.PracticeName,
                                      SubjectName = s.SubjectName
                                  }).ToList();


            return View(coursePracticeList);

        }
        public IActionResult AddMasterKeyboard()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var subjectList = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();
            ViewBag.SubjectList = subjectList;
            return View();
        }
        public IActionResult SaveMasterKeyboard()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            string practiceName = Request.Form["practicename"];
            string subjectId = Request.Form["Subject"];
            coursepractices coursePractice = new coursepractices();
            coursePractice.PracticeName = practiceName;
            coursePractice.SubjectId = Convert.ToInt32(subjectId);
            coursePractice.InstituteId = instituteId;
            myDbContext.coursepractices.Add(coursePractice);
            myDbContext.SaveChanges();
            return RedirectToAction("UploadMasterKeyboard");
        }
        public JsonResult DeleteCoursePractice(string id)
        {
            // Delete institute timings

            string errorresult = string.Empty;
            string successresult = string.Empty;

            if (string.IsNullOrEmpty(id))
            {
                errorresult = "Invalid Practice ID.";
            }
            int practId;
            if (!int.TryParse(id, out practId))
            {
                errorresult = "Invalid Practice ID format.";
            }
            var prac = myDbContext.coursepractices.Where(x => x.Id == practId).FirstOrDefault();
            if (prac == null)
            {
                errorresult = "Practice not found.";
            }
            myDbContext.coursepractices.Remove(prac);
            myDbContext.SaveChanges();

            if (string.IsNullOrEmpty(errorresult))
            {
                successresult = "Practice deleted successfully.";
                return Json(new { success = true, message = successresult });
            }
            else
            {
                return Json(new { success = false, message = errorresult });
            }
        }
        public IActionResult UpdatePractice()
        {
            int Id = Convert.ToInt32(Request.Form["hdnId"]);
            string subjectname = Request.Form["subjectname"];
            string practicename = Request.Form["practicename"];

            var Record = myDbContext.coursepractices.FirstOrDefault(x => x.Id == Id);
            if (Record != null)
            {
                Record.PracticeName = practicename;
                myDbContext.coursepractices.Update(Record);
                myDbContext.SaveChanges();
            }

            return RedirectToAction("UploadMasterKeyboard");
        }
        public IActionResult UploadSpeedPractice()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            var CourseList1 = myDbContext.Course
                .Select(x => new SelectListItem
                {
                    Text = x.CourseName,
                    Value = x.Id.ToString()
                }).ToList();
            var SubjectList1 = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();

            ViewBag.SubjectList1 = SubjectList1;
            ViewBag.CourseList1 = CourseList1;
            List<SpeedPracticeVM> speedPracticeList = new List<SpeedPracticeVM>();

            speedPracticeList = (from sp in myDbContext.speedPracticeUpload
                                 join sb in myDbContext.Subject on sp.SubjectId equals sb.Id
                                 join s in myDbContext.section on sp.sectionid equals s.Id
                                 where sp.InstituteId == instituteId && sp.IsDeleted == false
                                 select new SpeedPracticeVM
                                 {
                                     Id = sp.Id,
                                     SubjectName = sb.SubjectName,
                                     SectionName = s.SectionName,
                                     DateUploaded = sp.DateUploaded,
                                     FileName = sp.FileName
                                 }).ToList();
            return View(speedPracticeList);
        }
        public IActionResult AddSpeedPractice()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            var courseList = myDbContext.Course.ToList();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var Model in courseList)
            {
                list.Add(new SelectListItem { Text = Model.CourseName, Value = Model.Id.ToString() });
            }
            ViewBag.CourseList = list;

            var subjectList = myDbContext.Subject
               .Select(x => new SelectListItem
               {
                   Text = x.SubjectName,
                   Value = x.Id.ToString()
               }).ToList();
            ViewBag.SubjectList = subjectList;

            var sectionList3 = myDbContext.section
               .Select(x => new SelectListItem
               {
                   Text = x.SectionName,
                   Value = x.Id.ToString()
               }).ToList();
            ViewBag.SectionList3 = sectionList3;

            return View();
        }
        public IActionResult SearchSpeedPractice()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            string course = Request.Form["Course"];
            string subject = Request.Form["Subject"];
            string uploadDate = Request.Form["uploadDate"];

            List<SpeedPracticeVM> speedPracticeList = new List<SpeedPracticeVM>();

            if (uploadDate != "" || uploadDate != null)
            {
                DateTime dtUploadDateFormat = Convert.ToDateTime(uploadDate);

                speedPracticeList = (from sp in myDbContext.speedPracticeUpload
                                         join sb in myDbContext.Subject on sp.SubjectId equals sb.Id
                                         join s in myDbContext.section on sp.sectionid equals s.Id
                                         where sp.InstituteId == instituteId
                                         && (string.IsNullOrEmpty(course) || sp.CourseId.ToString() == course)
                                         && (string.IsNullOrEmpty(subject) || sp.SubjectId.ToString() == subject)
                                         && (sp.DateUploaded.Date == dtUploadDateFormat.Date)
                                     select new SpeedPracticeVM
                                         {
                                             Id = sp.Id,
                                             SubjectName = sb.SubjectName,
                                             SectionName = s.SectionName,
                                             DateUploaded = sp.DateUploaded,
                                             FileName = sp.FileName
                                         }).ToList();
            }
            else
            {
                speedPracticeList = (from sp in myDbContext.speedPracticeUpload
                                         join sb in myDbContext.Subject on sp.SubjectId equals sb.Id
                                         join s in myDbContext.section on sp.sectionid equals s.Id
                                         where sp.InstituteId == instituteId
                                         && (string.IsNullOrEmpty(course) || sp.CourseId.ToString() == course)
                                         && (string.IsNullOrEmpty(subject) || sp.SubjectId.ToString() == subject)
                                         select new SpeedPracticeVM
                                         {
                                             Id = sp.Id,
                                             SubjectName = sb.SubjectName,
                                             SectionName = s.SectionName,
                                             DateUploaded = sp.DateUploaded,
                                             FileName = sp.FileName
                                         }).ToList();
            }

            var CourseList1 = myDbContext.Course
                .Select(x => new SelectListItem
                {
                    Text = x.CourseName,
                    Value = x.Id.ToString()
                }).ToList();
            var SubjectList1 = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();

            ViewBag.SubjectList1 = SubjectList1;
            ViewBag.CourseList1 = CourseList1;

            return View("UploadSpeedPractice", speedPracticeList);
        }
        //public JsonResult DeleteSpeedPractice(string Id)
        //{

        //    string errorresult = string.Empty;
        //    string successresult = string.Empty;

        //    if (string.IsNullOrEmpty(Id))
        //    {
        //        errorresult = "Invalid SpeedPractice ID.";
        //    }
        //    int PracticeId;
        //    if (!int.TryParse(Id, out PracticeId))
        //    {
        //        errorresult = "Invalid SpeedPractice ID format.";
        //    }
        //    var record = myDbContext.speedPracticeUpload.Where(x => x.Id == PracticeId).FirstOrDefault();
        //    if (record == null)
        //    {
        //        errorresult = "SpeedPractice not found.";
        //    }
        //    myDbContext.speedPracticeUpload.Remove(record);
        //    myDbContext.SaveChanges();

        //    if (string.IsNullOrEmpty(errorresult))
        //    {
        //        successresult = "SpeedPractice deleted successfully.";
        //        return Json(new { success = true, message = successresult });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = errorresult });
        //    }


        //}
        public IActionResult AddMCQSection()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var AddMCQSection = (from mcqs in myDbContext.mcqsection
                                 join s in myDbContext.Subject on mcqs.SubjectId equals s.Id
                                 where mcqs.InstituteId == Convert.ToInt32(HttpContext.Session.GetString("InstituteID"))
                                 select new mcqsectionVM
                                 {
                                     Id = mcqs.Id,
                                     SectionName = mcqs.SectionName,
                                     SubjectName = s.SubjectName,
                                 }).ToList();

            return View(AddMCQSection);
        }
        public IActionResult AddMCQSec()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var subjectList2 = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();

            ViewBag.SubjectList2 = subjectList2;
            return View();
        }
        public IActionResult SaveMCQSection()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            string subjectId = Request.Form["Subject"];
            string sectionName = Request.Form["sectionname"];

            mcqsection mcqSection = new mcqsection();
            mcqSection.SubjectId = Convert.ToInt32(subjectId);
            mcqSection.SectionName = sectionName;
            mcqSection.InstituteId = instituteId;
            myDbContext.mcqsection.Add(mcqSection);
            myDbContext.SaveChanges();

            return RedirectToAction("AddMCQSection");
        }
        public IActionResult UploadMCQ()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var UploadMcqReport = (from mcq in myDbContext.mcqquestions
                                   join s in myDbContext.Subject on mcq.SubjectId equals s.Id
                                   join sec in myDbContext.mcqsection on mcq.SectionId equals sec.Id
                                   where mcq.InstituteId == instituteId && mcq.IsDeleted == false
                                   select new UploadMcqVM
                                   {
                                       Id = mcq.Id,
                                       SubjectName = s.SubjectName,
                                       SectionName = sec.SectionName,
                                       Question = mcq.QuesTitle,
                                       Option1 = mcq.Option1,
                                       Option2 = mcq.Option2,
                                       Option3 = mcq.Option3,
                                       Option4 = mcq.Option4,
                                       Answer = mcq.CorrectAnswer,
                                   }).ToList();


            return View(UploadMcqReport);

        }
        public IActionResult AddMCQ()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            var subjectList3 = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();

            ViewBag.SubjectList3 = subjectList3;

            var sectionList3 = myDbContext.mcqsection
                .Where(x => x.InstituteId == instituteId)
                .Select(x => new SelectListItem
                {
                    Text = x.SectionName,
                    Value = x.Id.ToString()
                }).ToList();
            ViewBag.SectionList3 = sectionList3;
            return View();

        }

        [HttpPost]
        public IActionResult UploadMCQQuestions(IFormFile file)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            int SubjectId = Convert.ToInt32(Request.Form["Subject"]);
            int sectionId = Convert.ToInt32(Request.Form["Section"]);

            // Upload excel file data to mcqquestions table
            //IFormFile file = Request.Form.Files["mcqFile"];
            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    ExcelPackage.License.SetNonCommercialPersonal("DSequence"); // Set the license context for EPPlus
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var questionText = worksheet.Cells[row, 1].Text;
                            var optionA = worksheet.Cells[row, 2].Text;
                            var optionB = worksheet.Cells[row, 3].Text;
                            var optionC = worksheet.Cells[row, 4].Text;
                            var optionD = worksheet.Cells[row, 5].Text;
                            var correctAnswer = worksheet.Cells[row, 6].Text;

                            mcqquestions mcq = new mcqquestions
                            {
                                SubjectId = SubjectId,
                                SectionId = sectionId,
                                QuesTitle = questionText,
                                Option1 = optionA,
                                Option2 = optionB,
                                Option3 = optionC,
                                Option4 = optionD,
                                CorrectAnswer = correctAnswer,
                                InstituteId = instituteId,
                                DateUploaded = DateTime.Now,
                                IsDeleted = false
                            };
                            myDbContext.mcqquestions.Add(mcq);
                        }
                        myDbContext.SaveChanges();
                    }
                }
            }

            return RedirectToAction("UploadMCQ");

        }
        public IActionResult TypingTest()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var SubjectList4 = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();
            var SectionList4 = myDbContext.section
                .Select(x => new SelectListItem
                {
                    Text = x.SectionName,
                    Value = x.Id.ToString()
                }).ToList();

            ViewBag.SubjectList4 = SubjectList4;
            ViewBag.SectionList4 = SectionList4;

            return View();
        }
        public IActionResult MCQTest()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            var SubjectList5 = myDbContext.Subject
                .Select(x => new SelectListItem
                {
                    Text = x.SubjectName,
                    Value = x.Id.ToString()
                }).ToList();

            var SectionList5 = myDbContext.mcqsection
                .Where(x => x.InstituteId == instituteId)
                .Select(x => new SelectListItem
                {
                    Text = x.SectionName,
                    Value = x.Id.ToString()
                }).ToList();

            ViewBag.SubjectList5 = SubjectList5;
            ViewBag.SectionList5 = SectionList5;

            return View();
        }

        [HttpPost]
        public IActionResult MCQTestSubmit(IFormFile file1)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

            DateTime TestDate = Convert.ToDateTime(Request.Form["date"]);
            int subjectId = Convert.ToInt32(Request.Form["Subject5"]);
            int sectionId = Convert.ToInt32(Request.Form["Section5"]);
            int NoOfQuestion = Convert.ToInt32(Request.Form["noofquestion"]);
            int MarkQuestion = Convert.ToInt32(Request.Form["markquestion"]);
            int PassingMarks = Convert.ToInt32(Request.Form["passingmarks"]);
            int testduration = Convert.ToInt32(Request.Form["testduration"]);

            mcqtestschedule mcqtestschedule = new mcqtestschedule();
            mcqtestschedule = new mcqtestschedule
            {
                TestDate = TestDate,
                SubjectId = subjectId,
                SectionId = sectionId,
                NoOfQuest = NoOfQuestion,
                EachQuesMark = MarkQuestion,
                PassingMarks = PassingMarks,
                TestDuration = testduration,
                InstituteId = instituteId
            };
            myDbContext.mcqtestschedule.Add(mcqtestschedule);
            myDbContext.SaveChanges();
            int testScheduleId = mcqtestschedule.Id;

            // Upload excel file data to mcqquestions table
            if (file1 != null && file1.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    file1.CopyTo(stream);
                    ExcelPackage.License.SetNonCommercialPersonal("DSequence"); // Set the license context for EPPlus
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var questionText = worksheet.Cells[row, 1].Text;
                            var optionA = worksheet.Cells[row, 2].Text;
                            var optionB = worksheet.Cells[row, 3].Text;
                            var optionC = worksheet.Cells[row, 4].Text;
                            var optionD = worksheet.Cells[row, 5].Text;
                            var correctAnswer = worksheet.Cells[row, 6].Text;

                            mcqquestions mcq = new mcqquestions
                            {
                                SubjectId = subjectId,
                                SectionId = sectionId,
                                QuesTitle = questionText,
                                Option1 = optionA,
                                Option2 = optionB,
                                Option3 = optionC,
                                Option4 = optionD,
                                CorrectAnswer = correctAnswer,
                                InstituteId = instituteId,
                                DateUploaded = DateTime.Now,
                                IsDeleted = false
                            };
                            myDbContext.mcqquestions.Add(mcq);
                            myDbContext.SaveChanges();
                            int instertedId = mcq.Id;

                            // Associate the questions with the test schedule
                            if (instertedId > 0)
                            {
                                mcqtestquestpaper mcqtestquestpaper = new mcqtestquestpaper
                                {
                                    QuestId = instertedId,
                                    TestScheduleId = testScheduleId
                                };
                                myDbContext.mcqtestquestpaper.Add(mcqtestquestpaper);
                                myDbContext.SaveChanges();
                            }
                        }

                    }
                }
            }


            return RedirectToAction("MCQTest");
        }
        public IActionResult TypingTestSubmit()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            DateTime dateTime = Convert.ToDateTime(Request.Form["dateTime"]);
            int subjectId = Convert.ToInt32(Request.Form["Subject4"]);
            int sectionId = Convert.ToInt32(Request.Form["Section4"]);

            var speedUploadId = myDbContext.speedPracticeUpload.Where(x => x.SubjectId == subjectId && x.sectionid == sectionId && x.InstituteId == instituteId).Select(x => x.Id).FirstOrDefault();
            typingtestschedule typingTest = new typingtestschedule();
            typingTest = new typingtestschedule
            {
                SubjectId = subjectId,
                SectionId = sectionId,
                SpeedPracticeUploadId = speedUploadId,
                TestDate = dateTime,
                InstituteId = instituteId
            };

            myDbContext.typingtestschedule.Add(typingTest);
            myDbContext.SaveChanges();

            TempData["Message"] = "Test scheduled successfully!!";

            return RedirectToAction("TypingTest");
        }

        [HttpPost]
        public IActionResult SaveSpeedPractice(IFormFile file)
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
            var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            int courseId = Convert.ToInt32(Request.Form["Course"]);
            int SubjectId = Convert.ToInt32(Request.Form["Subject"]);
            int sectionId = Convert.ToInt32(Request.Form["Section"]);

            string sectionName = myDbContext.section
                .Where(x => x.Id == sectionId)
                .Select(x => x.SectionName)
                .FirstOrDefault();

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Courses");
            var sampleFileFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SampleFile\\Passage\\Samplefile.docx");
            var sampleExcelFileFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SampleFile\\Statement\\SampleExcel.xlsx");
            var samplePowerpointFileFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SampleFile\\Powerpoint\\Samplefile.pptx");

            string newPath = Path.Combine(uploadDirectory, sectionName);
            string institutePath = Path.Combine(newPath, instituteId.ToString());

            if (!Directory.Exists(institutePath))
            {
                Directory.CreateDirectory(institutePath);
            }

            string sampleFilePath = string.Empty;
            if (sectionName == "Passage" || sectionName == "Letter")
            {
                sampleFilePath = sampleFileFolder;
            }

            if (sectionName == "Statement")
            {
                sampleFilePath = sampleExcelFileFolder;
            }

            if (sectionName == "Powerpoint")
            {
                sampleFilePath = samplePowerpointFileFolder;
            }

            string subjectName = myDbContext.Subject
                .Where(x => x.Id == SubjectId)
                .Select(x => x.SubjectName)
                .FirstOrDefault();
            string subjectPath = Path.Combine(institutePath, subjectName);
            // Create Subject Folder
            if (!Directory.Exists(subjectPath))
            {
                Directory.CreateDirectory(subjectPath);
            }

            string destinationSamplePath = string.Empty;
            if (sectionName == "Passage" || sectionName == "Letter")
            {
                destinationSamplePath = Path.Combine(subjectPath, "Samplefile.docx");
            }
            if (sectionName == "Statement")
            {
                destinationSamplePath = Path.Combine(subjectPath, "SampleExcel.xlsx");
            }

            if (sectionName == "Powerpoint")
            {
                destinationSamplePath = Path.Combine(subjectPath, "Samplefile.pptx");
            }

            if (!System.IO.File.Exists(destinationSamplePath))
            {
                System.IO.File.Copy(sampleFilePath, destinationSamplePath, true);
            }

            // Save the uploaded file
            if (file != null && file.Length > 0)
            {
                string lastFileToken = myDbContext.speedPracticeUpload.Where(x => x.SubjectId == SubjectId && x.sectionid == sectionId && x.InstituteId == instituteId && x.CourseId == courseId).Select(x => x.FilToken).Take(1).OrderByDescending(x => x).FirstOrDefault();
                int sectionOrder = 1;

                if (!string.IsNullOrEmpty(lastFileToken))
                {
                    sectionOrder = Convert.ToInt32(lastFileToken) + 1;
                }

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(subjectPath, fileName);

                string destinationPath = string.Empty;
                string newFileName = string.Empty;

                //if (subjectName == "English40" && sectionName == "Letter")
                //{
                //    if (fileName.Contains("Unformat"))
                //    {
                //        newFileName = sectionName + "_" + sectionOrder;
                //        destinationPath = Path.Combine(subjectPath, newFileName + Path.GetExtension(fileName));
                //        using (var stream = new FileStream(destinationPath, FileMode.Create))
                //        {
                //            file.CopyTo(stream);
                //        }
                //    }
                //}
                //else
                //{
                //    newFileName = sectionName + "_" + sectionOrder;
                //    destinationPath = Path.Combine(subjectPath, newFileName + Path.GetExtension(fileName));
                //    using (var stream = new FileStream(destinationPath, FileMode.Create))
                //    {
                //        file.CopyTo(stream);
                //    }
                //}

                newFileName = sectionName + "_" + sectionOrder;
                destinationPath = Path.Combine(subjectPath, newFileName + Path.GetExtension(fileName));
                using (var stream = new FileStream(destinationPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                string answerFileName = string.Empty;

                //if (subjectName == "English40" && sectionName == "Letter")
                //{
                //    if (fileName.Contains("Answer"))
                //    {
                //        answerFileName = sectionName + "_" + sectionOrder + "Answer" + Path.GetExtension(fileName);
                //        string destinationAnswerFilePath = Path.Combine(subjectPath, answerFileName);
                //        using (var stream = new FileStream(destinationAnswerFilePath, FileMode.Create))
                //        {
                //            file.CopyTo(stream);
                //        }

                //        // Save Use File
                //        string UseFileName = sectionName + "_" + sectionOrder + "Use" + Path.GetExtension(fileName);
                //        string destinationFileUsePath = Path.Combine(subjectPath, UseFileName);
                //        using (var stream = new FileStream(destinationFileUsePath, FileMode.Create))
                //        {
                //            file.CopyTo(stream);
                //        }
                //    }
                //}
                //else
                //{
                //    // Save Use File
                //    string UseFileName = sectionName + "_" + sectionOrder + "Use" + Path.GetExtension(fileName);
                //    string destinationFileUsePath = Path.Combine(subjectPath, UseFileName);
                //    using (var stream = new FileStream(destinationFileUsePath, FileMode.Create))
                //    {
                //        file.CopyTo(stream);
                //    }
                //}

                if (fileName.Contains("Answer"))
                {
                    answerFileName = sectionName + "_" + sectionOrder + "Answer" + Path.GetExtension(fileName);
                    string destinationAnswerFilePath = Path.Combine(subjectPath, answerFileName);
                    using (var stream = new FileStream(destinationAnswerFilePath, FileMode.Create))
                    {
                        var sampleFileStream = new FileStream(destinationSamplePath, FileMode.Open);
                        sampleFileStream.CopyTo(stream);
                    }

                    // Save Use File
                    string UseFileName = sectionName + "_" + sectionOrder + "Use" + Path.GetExtension(fileName);
                    string destinationFileUsePath = Path.Combine(subjectPath, UseFileName);
                    using (var stream = new FileStream(destinationFileUsePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                speedPracticeUpload speedPractice = new speedPracticeUpload
                {
                    CourseId = courseId,
                    SubjectId = SubjectId,
                    sectionid = sectionId,
                    FileName = newFileName,
                    FilePath = destinationPath,
                    DateUploaded = DateTime.Now,
                    InstituteId = instituteId,
                    IsDeleted = false,
                    FilToken = sectionOrder.ToString(),
                    UserId = userId,
                };
                myDbContext.speedPracticeUpload.Add(speedPractice);
                myDbContext.SaveChanges();
            }


            return RedirectToAction("UploadSpeedPractice");

        }
        public async Task<IActionResult> DeleteMCQ(int id)
        {
            var MCQQuestion = await myDbContext.mcqquestions.FindAsync(id);
            if (MCQQuestion == null)
            {
                return Json(new { success = false, message = "MCQ Question not found!" });
            }
            MCQQuestion.IsDeleted = true;
            myDbContext.mcqquestions.Update(MCQQuestion);
            await myDbContext.SaveChangesAsync();

            return Json(new { success = true, message = " MCQ Question deleted successfully!" });
        }

        public async Task<IActionResult> DeleteSpeedPractice(int id)
        {
            var speedPracticeUpload = await myDbContext.speedPracticeUpload.FindAsync(id);
            if (speedPracticeUpload == null)
            {
                return Json(new { success = false, message = "Speed Practice not found!" });
            }
            speedPracticeUpload.IsDeleted = true;
            myDbContext.speedPracticeUpload.Update(speedPracticeUpload);
            await myDbContext.SaveChangesAsync();

            return Json(new { success = true, message = " Speed Practice deleted successfully!" });
        }

    }

}
