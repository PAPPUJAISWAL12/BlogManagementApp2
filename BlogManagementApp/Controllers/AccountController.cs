using BlogManagementApp.Models;
using BlogManagementApp.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace BlogManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogManagementSystemContext _appContext;
        private readonly IDataProtector _protector;
        public AccountController(BlogManagementSystemContext context,DataSecurityKey key, IDataProtectionProvider provider) {
            _appContext = context;
            _protector = provider.CreateProtector(key.key);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserListEdit uEdit) {
            var users=_appContext.UserLists.ToList();
            if (users != null)
            {
                var u = users.Where(x => x.EmailAddress.ToUpper()
                .Equals(uEdit.EmailAddress.ToUpper()) 
                && _protector.Unprotect(x.UserPassword).Equals(uEdit.UserPassword)).FirstOrDefault();
                if (u != null)
                {
                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.Name,u.UserId.ToString()),
                        new Claim(ClaimTypes.Role,u.UserRole),
                        new Claim("FullName",u.FullName)
                    };
                    var identity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.
                        AuthenticationScheme,new ClaimsPrincipal(identity),new AuthenticationProperties { IsPersistent=true});

                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                ModelState.AddModelError("","Invalid User");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.
                AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return RedirectToAction("Index","Home");
        }


        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePassword c)
        {
            if (ModelState.IsValid)
            {
                var u=_appContext.UserLists.Where(e=>e.UserId ==Convert.ToInt16(User.Identity!.Name)).FirstOrDefault();
                if (_protector.Unprotect(u.UserPassword)!=c.CurrentPasswod)
                {
                    ModelState.AddModelError("","Check your current Password");
                    return View(c);
                }
                else
                {
                    if (c.NewPassword == c.ConfirmPassword)
                    {
                        u.UserPassword = c.NewPassword;
                        _appContext.Update(u);
                        _appContext.SaveChanges();
                        return Content("success");
                    }
                    else
                    {
                        ModelState.AddModelError("","Confirm Password  Doesn't matched.");
                        return View(c);
                    }
                }
            }
            return Json("failed");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(UserListEdit edit)
        {
           
            if(edit.EmailAddress != null)
            {
                Random r = new Random();
                HttpContext.Session.SetString("token",r.Next(9999).ToString());
                var token = HttpContext.Session.GetString("token");

                var user = _appContext.UserLists.Where(u => u.EmailAddress == edit.EmailAddress).FirstOrDefault();
                if (user != null) {
                   
                    try
                    {
                        SmtpClient s = new()
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential("jaiswalpappu873@gmail.com", "tbsd sxte srxb ublz"),
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network
                        };

                        MailMessage m = new()
                        {
                            From = new MailAddress("jaiswalpappu873@gmail.com"),
                            Subject = "Forgot Password token",
                            Body = $"<a href='https://localhost:44332/Account/ResetPassword/{_protector.Protect(token!)}' style='background-color:green; color:white; padding:10px; width:200px; border-radius:4px;'>ResetPasssword</a> <p>token number</p>:{token}",
                            IsBodyHtml = true
                        };
                        HttpContext.Session.SetString("email",user.EmailAddress);
                        m.To.Add(user.EmailAddress);
                        s.Send(m);
                       
                    }
                    catch(Exception ex)
                    {
                        return Json(ex);
                    }
                    return RedirectToAction("VerrifyToken");
                } else
                {
                    ModelState.AddModelError("", "This Email-Address is not register.");
                    return View(edit);
                }
            }
            return Json("failed");
        }

        [HttpGet]
        public IActionResult VerrifyToken()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerrifyToken(UserListEdit e)
        {
            var token = HttpContext.Session.GetString("token");
            if (token == e.EmailToken)
            {
                var et = _protector.Protect(e.EmailToken!);
                return RedirectToAction("ResetPassword", new { id = et });
            }
            else
            {
                return Json("failed");
            }
           
        }
        [AllowAnonymous]
        public IActionResult ResetPassword(string id)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                var eToken = _protector.Unprotect(id);
                if (token == eToken)
                {
                    return View(new ChangePassword { EmailToken=id});
                }
                else
                {
                    return RedirectToAction("ForgotPassword");
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("ForgotPassword");
            }        
        }

        [HttpPost]
        public IActionResult ResetPassword(ChangePassword c)
        {
            var token = HttpContext.Session.GetString("token");
            var eToken = _protector.Unprotect(c.EmailToken!);
            if (token == eToken)
            {
                var email = HttpContext.Session.GetString("email");
                  var us = _appContext.UserLists.Where(u => u.EmailAddress == email).FirstOrDefault();
                    if (us != null)
                    {
                        if (c.NewPassword == c.ConfirmPassword)
                        {
                            us.UserPassword = _protector.Protect(c.NewPassword);
                            _appContext.UserLists.Update(us);
                            _appContext.SaveChanges();
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                        ModelState.AddModelError("","Confirn Password does not matched. Please Try Again!.");
                          return View(c);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "failed");
                        return View(c);
                    }              
            }
            else
            {
                return RedirectToAction("ForgotPassword");
            }
        }
    }
}
