﻿using System.Web.Mvc;

public class AdminAreaRegistration : AreaRegistration
{
    public override string AreaName => "Admin";

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute(
            name: "Admin_default",
            url: "Admin/{controller}/{action}/{id}",
            defaults: new { action = "Index", id = UrlParameter.Optional },
            namespaces: new[] { "NguyenThanhTri.Areas.Admin.Controllers" }
        );
    }
}
