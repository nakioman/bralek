using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Microsoft.Web.Helpers;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Web.Resources;
using Treenks.Bralek.Web.ViewModels.Account;
using WebMatrix.WebData;

namespace Treenks.Bralek.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly BralekDbContext _dbContext;
        private const string ReCaptchaPrivateKey = "6Lc_lOESAAAAAHEqpTsmUChezFtqf6EpPauV5Nv1";

        public AccountController(BralekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isAuthenticated = WebSecurity.Login(model.Email, model.Password, model.RememberMe);
                if (isAuthenticated)
                {
                    if (!String.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(String.Empty, Messages.USER_PASSWORD_WRONG);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
#if DEBUG
                if (true)
#else
                if (ReCaptcha.Validate(ReCaptchaPrivateKey))
#endif
                {
                    if (_dbContext.Users.Any(x => x.Email == model.Email))
                    {
                        ModelState.AddModelError(string.Empty, Messages.EMAIL_ALREADY_EXISTS);
                    }
                    else
                    {
                        WebSecurity.CreateUserAndAccount(model.Email, model.Password);
                        WebSecurity.Login(model.Email, model.Password, false);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.RECAPTCHA_INVALID);
                }
            }

            return View(model);
        }

        [HttpGet]
        public JsonResult CheckEmail(string email)
        {
            if (_dbContext.Users.Any(x => x.Email == email))
            {
                return Json(Messages.EMAIL_ALREADY_EXISTS, JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userExists = WebSecurity.UserExists(model.Email);

                if (userExists)
                {
#if DEBUG
                    if (true)
#else
                    if (ReCaptcha.Validate(ReCaptchaPrivateKey))
#endif
                    {
                        var resetToken = WebSecurity.GeneratePasswordResetToken(model.Email);

                        var requestUrl = Url.RequestContext.HttpContext.Request.Url;
                        var urlAction = Url.Action("ResetPassword", new { token = resetToken });
                        var resetUrl = string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, urlAction);
                        var html = String.Format(Messages.BODY_RESET_PASSWORD_EMAIL, resetUrl);
                        var message = new MailMessage();
                        var smtp = new SmtpClient();
                        message.To.Add(new MailAddress(model.Email));
                        message.Subject = Messages.SUBJECT_RESET_PASSWORD_EMAIL;
                        message.Body = html;
                        message.IsBodyHtml = true;

                        smtp.Send(message);

                        return RedirectToAction("RecoverPasswordSent", "Account");
                    }

                    ModelState.AddModelError(String.Empty, Messages.RECAPTCHA_INVALID);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, Messages.EMAIL_NOT_EXISTS);
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordViewModel { ResetToken = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var passReseted = WebSecurity.ResetPassword(model.ResetToken, model.Password);
                if (passReseted)
                {
                    return RedirectToAction("PasswordResetComplete", "Account");
                }

                ModelState.AddModelError(String.Empty, Messages.RESET_PASSWORD_ERROR);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult RecoverPasswordSent()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PasswordResetComplete()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword(string successMsg)
        {
            ViewBag.SuccessMessage = successMsg;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var passwordChange = WebSecurity.ChangePassword(WebSecurity.CurrentUserName, model.OldPassword, model.Password);
                if (passwordChange)
                {
                    return RedirectToAction("ChangePassword", "Account", new { successMsg = Messages.PASSWORD_CHANGED });
                }
                else
                {
                    ModelState.AddModelError(String.Empty, Messages.PASSWORD_CHANGE_ERROR);
                }
            }

            return View(model);
        }
    }
}
