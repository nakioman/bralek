using System.ComponentModel.DataAnnotations;
using System.Web;
using Treenks.Bralek.Web.Resources;

namespace Treenks.Bralek.Web.ViewModels.Import
{
    public class GoogleReaderImportViewModel
    {
        [DataType(DataType.Upload)]
        [Required(ErrorMessageResourceName = "FIELD_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        public HttpPostedFileBase File { get; set; }
    }
}