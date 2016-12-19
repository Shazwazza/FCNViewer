using System;
using System.Web.Http.Controllers;

namespace FCNViewer.Controllers
{
    public class UseFcnReportFormatterAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings settings,
            HttpControllerDescriptor descriptor)
        {
            settings.Formatters.Add(new FcnReportFormatter());
        }
    }
}