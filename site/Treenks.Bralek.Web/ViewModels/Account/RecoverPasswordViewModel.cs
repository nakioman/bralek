using System.ComponentModel.DataAnnotations;
using Treenks.Bralek.Web.Resources;

namespace Treenks.Bralek.Web.ViewModels.Account
{
    public class RecoverPasswordViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessageResourceName = "EMAIL_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "EMAIL_NOT_VALID", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }
    }
}