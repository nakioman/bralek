using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Treenks.Bralek.Web.Resources;

namespace Treenks.Bralek.Web.ViewModels.Subscription
{
    public class AddSubscriptionViewModel
    {
        [DataType(DataType.Url)]
        [Url(ErrorMessage = null, ErrorMessageResourceName = "URL_NOT_VALID", ErrorMessageResourceType = typeof(Messages))]
        [Required(ErrorMessageResourceName = "FIELD_REQUIRED", ErrorMessageResourceType = typeof(Messages))]
        public string FeedUrl { get; set; }
    }
}