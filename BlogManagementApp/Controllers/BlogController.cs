using BlogManagementApp.Models;
using BlogManagementApp.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using System.Net;

namespace BlogManagementApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogManagementSystemContext _appContext;
        private readonly IWebHostEnvironment _env;
        private readonly IDataProtector _protector;
        public BlogController(BlogManagementSystemContext context,
            IWebHostEnvironment env,DataSecurityKey key,IDataProtectionProvider provider
            )
        {
            _appContext=context;
            _env=env;
            _protector = provider.CreateProtector(key.key);
        }
        // GET: BlogController
        public ActionResult Index()
        {
            var b=_appContext.BlogPosts.ToList();
            var blogList = b.Select(e => new BlogPostEdit
            {
                Bid = e.Bid,
                SectionImage = e.SectionImage,
                SectionDescription = e.SectionDescription,
                SectionHedding=e.SectionHedding,
                EncId = _protector.Protect(e.Bid.ToString()),
                PostDate=e.PostDate
            }).ToList();           
            return View(blogList);
        }

        // GET: BlogController/Details/5
        public ActionResult Details(int id)
        {
            return PartialView("_Details");
        }

        // GET: BlogController/Create
        public ActionResult AddBlog()
        {
            
            return View();
        }

        // POST: BlogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBlog(BlogPostEdit edit)
        {
            short maxid;
            try
            {
                if (_appContext.BlogPosts.Any())
                    maxid = Convert.ToInt16(_appContext.BlogPosts.Max(x => x.Bid) + 1);
                else 
                    maxid = 1;
                edit.Bid = maxid;

                if (edit.BlogFile != null)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(edit.BlogFile.FileName);
                    string filePath = Path.Combine(_env.WebRootPath,"BlogImage",filename);
                    using(FileStream stream =new FileStream(filePath,FileMode.Create))
                    {
                        edit.BlogFile.CopyTo(stream);
                    }
                    edit.SectionImage = filename;
                }
                BlogPost p = new()
                {
                    Bid = edit.Bid,
                    SectionHedding = edit.SectionHedding,
                    SectionDescription = edit.SectionDescription,
                    SectionImage = edit.SectionImage,
                    PostDate = edit.PostDate,
                    UploadUserId = Convert.ToInt16(User.Identity.Name)
                };

                _appContext.Add(p);
                _appContext.SaveChanges();
                return Content("success");
            }
            catch
            {
                return View();
            }
        }
        

       

        // GET: BlogController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BlogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BlogController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BlogController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
