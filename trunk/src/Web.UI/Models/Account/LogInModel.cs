using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Consumentor.Shopgun.Web.UI.Models.Account
{
    public class LogInModel
    {
        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool RememberMe { get; set; }
    }
}