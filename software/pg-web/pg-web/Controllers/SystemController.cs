using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pg_web.Models;
using System.Data.Entity;

namespace pg_web.Controllers
{
    public class SystemController : Controller
    {

        private PGSystemEntities m_db = new PGSystemEntities();
        //
        // GET: /System/
        public ActionResult Index()
        {
            List<Timer> timersCollection = m_db.Timer.ToList();
            Timer newTimer  = m_db.Timer.Create();
            newTimer.lifeTime = 13;
            newTimer.createTime = 2222;
            newTimer.lastSignalTime = 13;
            newTimer.timerUrl = "New timer url 2";
            m_db.Timer.Add(newTimer);
            int nResult  = m_db.SaveChanges();
            return View(nResult);
            //Timer t = (from m in m_db.Timer where m.timerId == 1 select m).First<Timer>();
            //return View(timersCollection);
            //return View(m_db.Event.ToList());
            //return Json(1, JsonRequestBehavior.AllowGet);
        }
	}
}