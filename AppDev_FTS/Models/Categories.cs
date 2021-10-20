using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppDev_FTS.Models
{
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }
}