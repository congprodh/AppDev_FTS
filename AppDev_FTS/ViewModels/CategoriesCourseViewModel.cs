using AppDev_FTS.Models;
using Microsoft.Build.Framework.XamlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppDev_FTS.ViewModels
{
    public class CategoriesCourseViewModel
    {
        public IEnumerable<Categories> Category { get; set; }
    }
}