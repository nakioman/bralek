using System.ComponentModel.DataAnnotations;
using Treenks.Bralek.Web.Resources;

namespace Treenks.Bralek.Web.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceName = "FIELD_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "PASWORD_NOT_MATCH", ErrorMessageResourceType = typeof(Messages))]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceName = "FIELD_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        public string ResetToken { get; set; }
    }
}