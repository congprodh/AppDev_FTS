using AppDev_FTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppDev_FTS.Areas.Staff.Controllers
{
    public class CategoryController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        // GET: Staff/Category
        public CategoryController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}