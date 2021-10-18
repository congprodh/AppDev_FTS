using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AppDev_FTS.Models;
using AppDev_FTS.Utils;
using AppDev_FTS.ViewModels;

namespace AppDev_FTS.Areas.Staff.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _context;

        // GET: Staff/Account
        public AccountController()
        {
            _context = new ApplicationDbContext();
        }

        public ApplicationSignInManager SignInManager
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

        public async Task<ActionResult> Index()
        {
         
            var traineeRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == Role.Trainee);

            var model = new UsersGroupViewModel
            {
                Trainees = await _context.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == traineeRole.Id))
                    .ToListAsync(),
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new AccountViewModel()
            {
                Roles = new List<string>() { Role.Trainee}
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role);
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }
            model.Roles = new List<string>() { Role.Trainee};       //load khi submit form

            return View(model);      // If we got this far, something failed, redisplay form
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new InfoViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(id))
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.User;
                var userinDb = await UserManager.FindByIdAsync(user.Id);

                if (userinDb == null)
                    return HttpNotFound();
                userinDb.FullName = user.FullName;
                userinDb.Age = user.Age;
                userinDb.Email = user.Email;
                userinDb.UserName = user.Email;

                IdentityResult result = await UserManager.UpdateAsync(userinDb);

                // if (model.Specialty != null)
                // {
                //    var profile = await _context.TrainerProfiles.SingleOrDefaultAsync(p => p.UserId == userinDb.Id);
                //    profile.Specialty = model.Specialty;
                // }
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                    AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new InfoViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(id))
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