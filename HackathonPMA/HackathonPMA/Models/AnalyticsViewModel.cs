using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;


namespace HackathonPMA.Models
{
    public class ByProjectViewModel
    {
        public int ProjectId { get; set; }

        [Required]
        [Display(Name = "Project Name: ")]
        public string ProjectName { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> ProjectList { get; set; }

        [Display(Name = "Location: ")]
        public string Location { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> LocationList { get; set; }

        public int[] instancesList { get; set; }

    }

    public class ByFundsViewModel
    {
        [Display(Name = "Project Id")]
        public int ProjectId { get; set; }

        public double Fund { get; set; }

        public int[] Logins { get; set; }


    }

    public class CustomViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        public double Fund { get; set; }

    }
}







