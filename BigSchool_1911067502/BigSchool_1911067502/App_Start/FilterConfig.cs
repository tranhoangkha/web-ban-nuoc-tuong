﻿using System.Web;
using System.Web.Mvc;

namespace BigSchool_1911067502
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
