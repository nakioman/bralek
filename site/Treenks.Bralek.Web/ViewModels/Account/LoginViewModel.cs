using System.ComponentModel.DataAnnotations;
using Treenks.Bralek.Web.Resources;

namespace Treenks.Bralek.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceName = "EMAIL_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "EMAIL_NOT_VALID", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceName = "FIELD_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}