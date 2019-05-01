using System.Web;
using System.Web.Mvc;
using Cycloid.Handlers;

namespace Cycloid.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
