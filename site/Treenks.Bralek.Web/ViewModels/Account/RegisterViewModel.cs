using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Treenks.Bralek.Web.Resources;

namespace Treenks.Bralek.Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [DataType(DataType.EmailAddress)]        
        [Required(ErrorMessageResourceName = "EMAIL_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceName = "EMAIL_NOT_VALID", ErrorMessageResourceType = typeof(Messages))]
        [Remote("CheckEmail", "Account", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "EMAIL_ALREADY_EXISTS")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceName = "FIELD_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceName = "PASWORD_NOT_MATCH", ErrorMessageResourceType = typeof(Messages))]
        public string ConfirmPassword { get; set; }
    }
}