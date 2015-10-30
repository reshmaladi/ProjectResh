using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

namespace HackathonPMA.Models
{

    [MetadataType(typeof(Project.MetaData))]
    public partial class Project
    {
        private class MetaData {

            [Remote(
                "doesProjectNameExist", 
                "Projects",
                AdditionalFields = "oldName",
                ErrorMessage = "Project name already exists or contains invalid character ','. Please enter valid Name.",
                HttpMethod = "POST"
            )]
            [Required]
            public string Name { get; set; }
        }
    }

    public class Employee
    {

        public ApplicationUser user {get; set; }
        public string roleName { get; set;}
    }

    public class ProjectDetailModel
    {

        public ProjectDetailModel()
        {
            this.stakeholders = new List<Employee>();
        }
        public Project project {get; set;}
        public Double SpentAmount { get; set; }
        public IList<Employee> stakeholders { get; set; }
    }
}