using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Mvc;
using System.Web.SessionState;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Web.Helpers;
using Treenks.Bralek.Web.Resources;
using Treenks.Bralek.Web.ViewModels.Shared;
using WebMatrix.WebData;

namespace Treenks.Bralek.Web.Controllers
{
    [Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class SharedController : Controller
    {
        private readonly BralekDbContext _dbContext;

        public SharedController(BralekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ActionResult Alerts()
        {
            var resourceManager = new ResourceManager("Treenks.Bralek.Web.Resources.Alerts", typeof(Alerts).Assembly);

            var resources = resourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true);
            var model = string.Empty;
            var user = _dbContext.Users.Single(x => x.Email == WebSecurity.CurrentUserName);
            foreach (DictionaryEntry resource in resources)
            {
                bool isNewAlert = user.AlertsSeen.All(x => x.Key != (string)resource.Key);
                if (isNewAlert)
                {
                    var value = (string)resource.Value;
                    model = value;
                    var actionStart = value.IndexOf('$', 0);
                    while (actionStart != -1)
                    {
                        var actionEnd = value.IndexOf('$', actionStart + 1);
                        var action = value.Substring(actionStart, actionEnd - actionStart + 1);
                        var actionUrl = GetActionUrl(action.Replace('$', ' ').Trim());
                        model = model.Replace(action, actionUrl);
                        actionStart = value.IndexOf('$', actionEnd + 1);
                    }

                    var alertSeen = new AlertInfo
                                        {
                                            Key = (string)resource.Key,
                                            ViewedBy = user,
                                            ViewedOnUTC = DateTime.UtcNow
                                        };
                    user.AlertsSeen.Add(alertSeen);
                    _dbContext.SaveChanges();

                    return PartialView("_Alerts", model);
                }
            }
            return PartialView("_Alerts", model);
        }

        private static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
        }

        private string GetActionUrl(string extensionMethod)
        {
            var extensionMethods = GetExtensionMethods(typeof(SharedUrlHelper).Assembly, typeof(UrlHelper));
            var actionMethod = extensionMethods.Single(x => x.Name == extensionMethod);
            return (string)actionMethod.Invoke(null, new object[] { Url });
        }
    }
}
