using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace HackathonPMA.Models
{

    [MetadataType(typeof(AspNetRole.MetaData))]
    public partial class AspNetRole
    {
        private class MetaData
        {

            [Remote(
                "doesRoleNameExist",
                "Roles",
                AdditionalFields = "oldName",
                ErrorMessage = "Role name already exists. Please enter a different role name.",
                HttpMethod = "POST"
            )]
            [Required]
            public string Name { get; set; }

        }

    }
}