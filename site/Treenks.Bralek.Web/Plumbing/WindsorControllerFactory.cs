﻿using Castle.MicroKernel;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Treenks.Bralek.Web.Plumbing
{
    /// <summary>
    /// The Castle controller factory. It overrides the standard MVC controllers resolution. It simply wrap the get/release controllers methods over Castle kernel.
    /// </summary>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController)_kernel.Resolve(controllerType);
        }
    }
}