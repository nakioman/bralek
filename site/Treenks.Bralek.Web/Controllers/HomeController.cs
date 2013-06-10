using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Web.ViewModels.Home;
using WebMatrix.WebData;

namespace Treenks.Bralek.Web.Controllers
{
    [Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class HomeController : Controller
    {
        private readonly BralekDbContext _dbContext;

        public HomeController(BralekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Landing", "Home");
            }

            var user = _dbContext.Users.Single(x => x.Email == WebSecurity.CurrentUserName);
            var model = new HomeIndexViewModel
                            {
                                OrderByOldestFirst = user.OrderByOldest,
                                ShowAllItems = user.ShowAllItems
                            };
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Landing()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
