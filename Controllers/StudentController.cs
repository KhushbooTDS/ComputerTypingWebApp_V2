using ComputerTypingWebApp.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using System.Collections.Immutable;
using System.Linq;

namespace ComputerTypingWebApp.Controllers
{
    public class StudentController : Controller
    {
        private dbContext myDbContext;
        public StudentController(dbContext context)
        {
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
            var notice = myDbContext.Notices.Where(x => x.ToUserId == userid).OrderByDescending(X => X.CreatedAt).FirstOrDefault();
            string strNotice = string.Empty;
            if (notice != null) { 
                strNotice = notice.NoticeText;
            }
            else
            {
                strNotice = "";
            }
            ViewBag.Notice = strNotice;



            return View();
        }
        public IActionResult GetStudentProfileDetails()
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
                    Gender = myDbContext.Gender.Where(x=>x.Id == Convert.ToInt32(studentData.Gender)).Select(x=>x.Name).FirstOrDefault(),
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
                    StudentPicURL = studentData.StudentPicURL != null ? studentData.StudentPicURL.ToLower() : studentData.StudentPicURL,
                    IdentityPicURL = studentData.IdentityPicURL != null ? studentData.IdentityPicURL.ToLower() : studentData.IdentityPicURL
                };
                return View(studentProfileViewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> KeywordPractice()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                {
                    return RedirectToAction("Index", "Login");
                }
                string UserName = HttpContext.Session.GetString("UserName") ?? string.Empty;

                var instituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));

                TypingCourceVM typingCourceVM = new TypingCourceVM
                {
                    Listcoursepractices = myDbContext.coursepractices.Where(x=>x.InstituteId == instituteId).Select(course => new coursepractices
                    {
                        Id = course.Id,
                        SubjectId = course.SubjectId,
                        PracticeName = course.PracticeName
                    }).ToList(),

                    ListOfSubject = await myDbContext.Students
                        .Where(stu => stu.StudentUserName == UserName) 
                        .Select(stu => new Students
                        {
                            Id = stu.Id,
                            
                            SelectSub30wpm = stu.SelectSub30wpm,
                            SelectSub40wpm = stu.SelectSub40wpm
                            
                        }).ToListAsync() 
                };

                return View(typingCourceVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        public async Task<IActionResult> SpeedPractice()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                {
                    return RedirectToAction("Index", "Login");
                }
                string UserName = HttpContext.Session.GetString("UserName") ?? string.Empty;

                TypingCourceVM typingCourceVM = new TypingCourceVM
                {
                    Listcoursepractices = myDbContext.coursepractices.Select(course => new coursepractices
                    {
                        Id = course.Id,
                        PracticeName = course.PracticeName
                    }).ToList(),

                    ListOfSubject = await myDbContext.Students
                        .Where(stu => stu.StudentUserName == UserName)
                        .Select(stu => new Students
                        {
                            Id = stu.Id,

                            SelectSub30wpm = stu.SelectSub30wpm,
                            SelectSub40wpm = stu.SelectSub40wpm

                        }).ToListAsync()
                };

                return View(typingCourceVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        public IActionResult TypingPracticeChapter(int practiceId, int subjectId, int courseId = 1)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }

                int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                Random rand = new Random();               

                var courseList = myDbContext.CoursesUpload
                    .Where(e => e.PracticeId == practiceId && e.CourseId == courseId && e.SubjectId == subjectId && e.InstituteId == InstituteId)
                    .Select(e => new { e.SubjectId, e.PracticeData }).ToList();

                var data = new CoursesUpload();

                if (courseList != null)
                {
                    if (courseList.Count() > 0)
                    {
                        int toSkip = rand.Next(0, courseList.Count());
                        var courseName = courseList.Skip(toSkip).Take(1).First();

                        if (courseName == null)
                        {
                            return NotFound("No course data found.");
                        }

                        data = new CoursesUpload()
                        {
                            PracticeData = courseName.PracticeData,
                            SubjectId = courseName.SubjectId,
                            PracticeId = practiceId
                        };
                    }
                    else
                    {
                        return NotFound("No course data found.");
                    }
                }
                else
                {
                    return NotFound("No course data found.");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult TypingPracticeHindi(int practiceId, int subjectId, int courseId = 1)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }
                var courseName = myDbContext.CoursesUpload
                    .Where(e => e.PracticeId == practiceId && e.CourseId == courseId && e.SubjectId == subjectId)
                    .Select(e => new { e.SubjectId, e.PracticeData })
                    .FirstOrDefault();

                if (courseName == null)
                {
                    return NotFound("No data found for the given parameters.");
                }
                ViewBag.SubjectId = courseName.SubjectId;
                var data = new CoursesUpload()
                {
                    PracticeData = courseName.PracticeData,
                    SubjectId = courseName.SubjectId,
                    PracticeId = practiceId
                };

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult TypingPracticeMarathi(int practiceId, int subjectId, int courseId = 1)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }
                var courseName = myDbContext.CoursesUpload
                    .Where(e => e.PracticeId == practiceId && e.CourseId == courseId && e.SubjectId == subjectId)
                    .Select(e => new { e.SubjectId, e.PracticeData })
                    .FirstOrDefault();

                if (courseName == null)
                {
                    return NotFound("No data found for the given parameters.");
                }
                ViewBag.SubjectId = courseName.SubjectId;
                var data = new CoursesUpload()
                {
                    PracticeData = courseName.PracticeData,
                    SubjectId = courseName.SubjectId,
                    PracticeId = practiceId
                };

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult SaveProgress(SaveProgressModel saveProgressModel)
        {
            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            saveProgressModel.StudentName = HttpContext.Session.GetString("UserName");
            TypingResult typingResult = new TypingResult();
            //typingResult.StudentId = saveProgressModel.StudentId;
            typingResult.StudentId = userid;
            typingResult.UserName = HttpContext.Session.GetString("UserName");
            typingResult.PracticeId = saveProgressModel.PracticeId;
            typingResult.SubjectId = saveProgressModel.SubjectId;
            typingResult.TotalCorrectCharacters = saveProgressModel.TotalCorrectCharacters;
            typingResult.TotalIncorrectCharacters = saveProgressModel.TotalIncorrectCharacters;
            typingResult.GrossSpeedPerMinute = saveProgressModel.TotalCorrectCharacters == 0 ? 0 : saveProgressModel.TotalCorrectCharacters / 5 * 1;
            typingResult.NetSpeedPerMinute = typingResult.GrossSpeedPerMinute - saveProgressModel.TotalIncorrectCharacters / 5 * 1;
            typingResult.Accuracy = typingResult.GrossSpeedPerMinute == 0 ? 0 : typingResult.NetSpeedPerMinute * 100 / typingResult.GrossSpeedPerMinute;
            typingResult.CreateDate = DateTime.Now;
            myDbContext.TypingResult.Add(typingResult);
            myDbContext.SaveChanges();
            return Json(typingResult);
        }
        public IActionResult TheoryCourse()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public IActionResult FeesInstallmentDetails()
        {
            if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
            {
                return RedirectToAction("Index", "Login");
            }

            var receipts = myDbContext.Receipts.Where(x => x.StudentUserName == HttpContext.Session.GetString("UserName")).ToList();

            List<FeeInstallmentVM> list = new List<FeeInstallmentVM>();
            foreach (var rc in receipts)
            {
                FeeInstallmentVM feeInstallmentVM = new FeeInstallmentVM();
                feeInstallmentVM.StudentUserName = rc.StudentUserName;

                feeInstallmentVM.SubjectIds = rc.SubjectIds;
                var subjects = myDbContext.Subject.Where(x=>rc.SubjectIds.Contains(x.Id.ToString())).ToList();
                string strSubjects = string.Empty;
                foreach (var subject in subjects)
                {
                    strSubjects += subject.SubjectName + ",";
                }
                strSubjects = strSubjects.TrimEnd(',');
                feeInstallmentVM.SubjectName = strSubjects;

                feeInstallmentVM.TotalAmountDue = rc.TotalAmountDue;
                feeInstallmentVM.AmountPaid = rc.AmountPaid;
                feeInstallmentVM.BalanceAmountDue = rc.BalanceAmountDue;
                feeInstallmentVM.PaymentMadeBy = rc.PaymentMadeBy;
                feeInstallmentVM.InstallmentDate = rc.InstallmentDate;

                list.Add(feeInstallmentVM);
            }

            return View(list);
        }

        public IActionResult SpeedPassage(int practiceId, int subjectId, int courseId = 1)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }

                int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                Random rand = new Random();

                var courseList = myDbContext.speedPracticeUpload
                    .Where(e => e.PracticeId == practiceId && e.CourseId == courseId && e.SubjectId == subjectId && e.InstituteId == InstituteId)
                    .Select(e => new { e.SubjectId, e.FilePath, e.FilToken, e.UserId }).ToList();

                var data = new speedPracticeUpload();

                if (courseList != null)
                {
                    if (courseList.Count() > 0)
                    {
                        int toSkip = rand.Next(0, courseList.Count());
                        var courseName = courseList.Skip(toSkip).Take(1).First();

                        if (courseName == null)
                        {
                            return NotFound("No course data found.");
                        }

                        data = new speedPracticeUpload()
                        {
                            FilePath = courseName.FilePath,
                            SubjectId = courseName.SubjectId,
                            PracticeId = practiceId,
                            FilToken = courseName.FilToken,
                            UserId = courseName.UserId
                        };
                    }
                    else
                    {
                        return NotFound("No course data found.");
                    }
                }
                else
                {
                    return NotFound("No course data found.");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult SpeedLetter(int practiceId, int subjectId, int courseId = 1)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }

                int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                Random rand = new Random();

                var courseList = myDbContext.speedPracticeUpload
                    .Where(e => e.PracticeId == practiceId && e.CourseId == courseId && e.SubjectId == subjectId && e.InstituteId == InstituteId)
                    .Select(e => new { e.SubjectId, e.FilePath, e.FilToken, e.UserId }).ToList();

                var data = new speedPracticeUpload();

                if (courseList != null)
                {
                    if (courseList.Count() > 0)
                    {
                        int toSkip = rand.Next(0, courseList.Count());
                        var courseName = courseList.Skip(toSkip).Take(1).First();

                        if (courseName == null)
                        {
                            return NotFound("No course data found.");
                        }

                        data = new speedPracticeUpload()
                        {
                            FilePath = courseName.FilePath,
                            SubjectId = courseName.SubjectId,
                            PracticeId = practiceId,
                            FilToken = courseName.FilToken,
                            UserId = courseName.UserId
                        };
                    }
                    else
                    {
                        return NotFound("No course data found.");
                    }
                }
                else
                {
                    return NotFound("No course data found.");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult SpeedStatement(int practiceId, int subjectId, int courseId = 1)
        {
            try
            {
                if (HttpContext.Session.GetString("UserName") == null || HttpContext.Session.GetString("UserName") == "")
                {
                    return RedirectToAction("Index", "Login");
                }

                int InstituteId = Convert.ToInt32(HttpContext.Session.GetString("InstituteID"));
                Random rand = new Random();

                var courseList = myDbContext.speedPracticeUpload
                    .Where(e => e.PracticeId == practiceId && e.CourseId == courseId && e.SubjectId == subjectId && e.InstituteId == InstituteId)
                    .Select(e => new { e.SubjectId, e.FilePath, e.FilToken, e.UserId }).ToList();

                var data = new speedPracticeUpload();

                if (courseList != null)
                {
                    if (courseList.Count() > 0)
                    {
                        int toSkip = rand.Next(0, courseList.Count());
                        var courseName = courseList.Skip(toSkip).Take(1).First();

                        if (courseName == null)
                        {
                            return NotFound("No course data found.");
                        }

                        data = new speedPracticeUpload()
                        {
                            FilePath = courseName.FilePath,
                            SubjectId = courseName.SubjectId,
                            PracticeId = practiceId,
                            FilToken = courseName.FilToken,
                            UserId = courseName.UserId
                        };
                    }
                    else
                    {
                        return NotFound("No course data found.");
                    }
                }
                else
                {
                    return NotFound("No course data found.");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public FileResult DownloadExe()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EditorExe");
            //byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = "wordtest.exe";
            string fullPath = Path.Combine(path, fileName);
            return File(System.IO.File.ReadAllBytes(fullPath), System.Net.Mime.MediaTypeNames.Application.Octet, System.IO.Path.GetFileName(fullPath));
        }
    }
}
