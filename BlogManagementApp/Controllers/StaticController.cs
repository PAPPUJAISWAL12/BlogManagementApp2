using BlogManagementApp.Models;
using BlogManagementApp.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementApp.Controllers
{
    public class StaticController : Controller
    {
        private readonly BlogManagementSystemContext _appContext;
        private readonly IDataProtector _protector;
        public StaticController(BlogManagementSystemContext context,DataSecurityKey key,IDataProtectionProvider provider)
        {
            _appContext = context;
            _protector = provider.CreateProtector(key.key);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index","Home");
            }

            var b =await _appContext.BlogPosts.ToListAsync();
            var blogList = b.Select(e => new BlogPostEdit
            {
                Bid = e.Bid,
                SectionImage = e.SectionImage,
                SectionDescription = e.SectionDescription,
                SectionHedding=e.SectionHedding,
                EncId = _protector.Protect(e.Bid.ToString()),
                PostDate = e.PostDate
            }).ToList();

            return View(blogList);
        }
    }
}
