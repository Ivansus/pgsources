using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using pg_web.sys;

namespace pg_web
{
	public class Global : HttpApplication {
		EventSystem m_eventSystem;
		void Application_Start(object sender, EventArgs e) {
			// Code that runs on application startup
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			m_eventSystem = new EventSystem();
			pg_web.sys.pg.Core.Instance.init();
		}
	}
}