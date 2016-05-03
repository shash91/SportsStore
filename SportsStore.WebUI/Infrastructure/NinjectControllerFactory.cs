using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using System.Configuration;
using Moq;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory:DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)ninjectKernel.Get(controllerType);
        }
        public void AddBindings()
        {
            ninjectKernel.Bind<IProductsRepository>().To<EFProductRepository>();

            Emailsettings emailSettings = new Emailsettings
            {
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["email.WriteAsFile"] ?? "false")
            };
            ninjectKernel.Bind<IOrderprocessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);
        }
    }
}