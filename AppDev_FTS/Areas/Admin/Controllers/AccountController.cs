using AppDev_FTS.Models;
using AppDev_FTS.Utils;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace AppDev_FTS.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public ApplicationSignInManager SignInManger
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        // GET: Admin/Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new ViewModels.AccountViewModel()
            {
                Roles = new List<string>() { Role.Trainer, Role.Staff }
            };
            return View(model);
        }



    }
}