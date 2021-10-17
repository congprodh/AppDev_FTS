using AppDev_FTS.Models;
using AppDev_FTS.Utils;
using AppDev_FTS.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppDev_FTS.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController()
        {
            _context = new ApplicationDbContext();
        }

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

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Account
        public async Task<ActionResult> Index()
        {
            var trainerRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == Role.Trainer);
            var staffRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == Role.Staff);

            var model = new UsersGroupViewModel()
            {
                Trainers = await _context.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == trainerRole.Id))
                    .ToListAsync(),
                Staffs = await _context.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == staffRole.Id))
                    .ToListAsync(),
            };
            return View(model);
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}