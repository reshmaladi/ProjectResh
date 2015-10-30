using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace HackathonPMA.Models
{

    [MetadataType(typeof(Fund.MetaData))]
    public partial class Fund
    {
        private class MetaData
        {

            [Remote(
                "doesFundNameExist",
                "Funds",
                AdditionalFields = "oldName",
                ErrorMessage = "Fund name already exists. Please enter a different fund name.",
                HttpMethod = "POST"
            )]
            [Required]
            public string Name { get; set; }

        }

    }
}