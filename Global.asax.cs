using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Appnetwork2022
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public class TrimModelBinder : DefaultModelBinder {
            protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,System.ComponentModel.PropertyDescriptor propertyDescriptor, object value) {
                if (propertyDescriptor.PropertyType == typeof(string)) {
                    var stringValue = (string)value;
                    if (!string.IsNullOrWhiteSpace(stringValue)) {
                        value = stringValue.Trim();
                    } else {
                        value = null;
                    }
                }

                base.SetProperty(controllerContext, bindingContext,
                                    propertyDescriptor, value);
            }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();
            //  BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
