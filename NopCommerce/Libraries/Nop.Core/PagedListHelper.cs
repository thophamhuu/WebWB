using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Nop.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public static class PagedListHelper
    {
        public static IAPIPagedList<T> ConvertPagedListToAPIPagedList<T>(this IPagedList<T> pagedList)
        {
            if (pagedList != null)
                return new APIPagedList<T>(pagedList.ToList(), pagedList.PageIndex, pagedList.PageSize, pagedList.TotalCount);
            return null;
        }
        public static IPagedList<T> ConvertAPIPagedListToPagedList<T>(this IAPIPagedList<T> apiPagedList)
        {
            if (apiPagedList != null)
                return new PagedList<T>(apiPagedList.Items, apiPagedList.PageIndex, apiPagedList.PageSize, apiPagedList.TotalCount);
            return null;
        }
    }
}
