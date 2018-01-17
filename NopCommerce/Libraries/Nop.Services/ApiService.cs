using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Services
{
    public abstract class ApiService
    {
        public bool CheckUseApi
        {
            get
            {
                bool isAdmin = true;
                bool isUseApi = true;

                var context = HttpContext.Current.Request.RequestContext;
                isAdmin = context.RouteData.Values["area"] != null && context.RouteData.Values["area"].ToString().ToLower() == "admin";
                return isUseApi && !isAdmin;
            }
        }
    }
}
