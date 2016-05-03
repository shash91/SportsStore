using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Entities;
using System.Web.ModelBinding;



namespace SportsStore.WebUI.Binders
{
    public  class CartModelBinder : System.Web.Mvc.IModelBinder
    {
        private const string sessionKey = "Cart";

        

        

        public object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingcontext)
        {
            Cart cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            if(cart==null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[sessionKey] = cart;
            }
            return cart;
        }
    }
}