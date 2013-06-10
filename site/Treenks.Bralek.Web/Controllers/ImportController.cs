using System;
using System.Web.Mvc;
using System.Web.SessionState;
using Treenks.Bralek.Common.Services.ImportFeeds;
using Treenks.Bralek.Web.Resources;
using Treenks.Bralek.Web.ViewModels.Import;
using WebMatrix.WebData;

namespace Treenks.Bralek.Web.Controllers
{
    [Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ImportController : Controller
    {
        private readonly IImportFeedService _importFeedService;

        public ImportController(IImportFeedService importFeedService)
        {
            _importFeedService = importFeedService;
        }

        [HttpGet]
        public ActionResult GoogleReader()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GoogleReader(GoogleReaderImportViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _importFeedService.ImportOpml(model.File.InputStream, WebSecurity.CurrentUserName);
                    return RedirectToAction("Index", "Home");
                }
                catch (InvalidOperationException)
                {
                    ModelState.AddModelError(String.Empty, Messages.GOOGLE_READER_FILE_NOT_VALID);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(String.Empty, Messages.IMPORT_ERROR);
                }
            }
            return View(model);
        }

    }
}
