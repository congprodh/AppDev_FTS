using AppDev_FTS.Models;
using System.Collections.Generic;

namespace AppDev_FTS.ViewModels
{
    public class UsersGroupViewModel
    {
        public List<ApplicationUser> Staffs { get; set; }

        public List<ApplicationUser> Trainers { get; set; }
    }
}