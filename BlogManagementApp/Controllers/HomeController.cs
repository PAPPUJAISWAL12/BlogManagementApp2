using BlogManagementApp.Models;
using BlogManagementApp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogManagementApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private readonly IDataProtector _protector;
        private readonly BlogManagementSystemContext _appContext;
        private readonly IWebHostEnvironment _env;
        public HomeController(DataSecurityKey dkey,IDataProtectionProvider provider,BlogManagementSystemContext context,IWebHostEnvironment env)
        {
           
            _protector = provider.CreateProtector(dkey.key);
            _appContext = context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<UserList> user = _appContext.UserLists.ToList();
            List<UserListEdit> u = user.Select(e => new UserListEdit
            {
                UserId = e.UserId,
                FullName = e.FullName,
                CurrentAddress = e.CurrentAddress,
                EmailAddress = e.EmailAddress,
                UserPhoto = e.UserPhoto,
                UserPassword = e.UserPassword,
                UserRole = e.UserRole,
                EncId = _protector.Protect(e.UserId.ToString())
            }).ToList();
            return View(u);
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create(UserListEdit e)
        {
            try
            {
                var users=_appContext.UserLists.Where(u=>u.EmailAddress==e.EmailAddress).FirstOrDefault();
                if (users == null)
                {
                    short maxid;
                    if (_appContext.UserLists.Any())
                        maxid = Convert.ToInt16(_appContext.UserLists.Max(x => x.UserId) + 1);
                    else
                        maxid = 1;
                    e.UserId = maxid;

                    if (e.UserFile != null)
                    {
                        string fileName = "UserImage" + Guid.NewGuid() + Path.GetExtension(e.UserFile.FileName);
                        string filePath = Path.Combine(_env.WebRootPath, "UserImage", fileName);
                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            e.UserFile.CopyTo(stream);
                        }
                        e.UserPhoto = fileName;
                    }

                    //auto Mapping
                    UserList u = new()
                    {
                        UserId = e.UserId,
                        FullName = e.FullName,
                        UserPassword = _protector.Protect(e.UserPassword),
                        CurrentAddress = e.CurrentAddress,
                        EmailAddress = e.EmailAddress,
                        UserPhoto = e.UserPhoto,
                        UserRole = e.UserRole
                    };
                    _appContext.Add(u);
                    _appContext.SaveChanges();

                    return RedirectToAction("Login", "Account");

                }
                else
                {
                    ModelState.AddModelError("", "This Email-Address is already exist.");
                    return View(e);
                }

            }
            catch
            {
                ModelState.AddModelError("","User Registration has been failed. Please, try again!.");
                return View(e);
            }
        }

        public IActionResult Details(string id)
        {
            int userid= Convert.ToInt32(_protector.Unprotect(id));
            var u = _appContext.UserLists.Where(i => i.UserId.Equals(userid));
            return Json(u);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult ProfileImage()
        {
            var p=_appContext.UserLists.Where(u=>u.UserId.Equals(Convert.ToInt16(User.Identity!.Name))).FirstOrDefault();
            ViewData["img"] = p.UserPhoto;
            return PartialView("_Profile");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ProfileUpdate()
        {
            UserList p = _appContext.UserLists.Where(x => x.UserId == Convert.ToInt16(User.Identity!.Name)).First();
            UserListEdit u = new()
            {
                UserId = p.UserId,
                FullName = p.FullName,
                UserPassword = p.UserPassword,
                UserPhoto = p.UserPhoto,
                EmailAddress = p.EmailAddress,
                CurrentAddress = p.CurrentAddress,
                UserRole = p.UserRole
            };
            return View(u);
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProfileUpdate(UserListEdit edit)
        {
            
            if (edit.UserFile != null)
            {
                string filename = "updateProfile" + Guid.NewGuid() + Path.GetExtension(edit.UserFile.FileName);
                string filePath = Path.Combine(_env.WebRootPath,"UserImage",filename);
                using (FileStream stream =new FileStream(filePath, FileMode.Create))
                {
                    edit.UserFile.CopyTo(stream);
                }
                edit.UserPhoto = filename;
            }
            UserList u = new()
            {
                UserId = edit.UserId,
                FullName = edit.FullName,
                CurrentAddress = edit.CurrentAddress,
                EmailAddress = edit.EmailAddress,
                UserPassword = edit.UserPassword,
                UserPhoto = edit.UserPhoto,
                UserRole = edit.UserRole
            };

            _appContext.Update(u);
            _appContext.SaveChanges();
            return RedirectToAction("ProfileUpdate");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
