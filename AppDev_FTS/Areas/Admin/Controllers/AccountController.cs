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
using ResetPasswordViewModel = AppDev_FTS.Models.ResetPasswordViewModel;

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
                userinDb.Address = user.Address;
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

        [HttpGet]
        public async Task<ActionResult> Delete(string id, bool? saveChangesError = false)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new InfoViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(user.Id))
            };

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(model);
        }

        //[HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ViewBag.ErrorMessage = "The user does not exist";
                return View(model);
            }

            var roles = await UserManager.GetRolesAsync(user.Id);
            if (!roles.All(r => r == Role.Staff || r == Role.Trainer))
            {
                ViewBag.ErrorMessage = "The user cannot be reset. Permission is denied.";
                return View(model);
            }

            model.Code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);
            return View();
        }

        public async Task<ActionResult> ConfirmedDelete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index), "Account");
            }

            return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
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