using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using pg_web.Models;

namespace pg_web.Controllers
{
	public class DevicesController : Controller {
		// GET: Devices
		public ActionResult Index() {
			return View();
		}

		public ActionResult DevicesList()
		{
			pgworkDBEntities db = new pgworkDBEntities();
			ViewBag.Deviceslist = db.Devices.ToList();
			ViewData["Devices"] = db.Devices.ToList();
			return View(db.Devices.ToList());
		}
	}
}