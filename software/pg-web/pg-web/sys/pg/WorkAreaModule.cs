using pg_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pg_web.sys.pg
{
	public class WorkAreaVisitEvent : EventArgs
	{

	}

	public class WorkAreaModule : IModule
	{
		public const String SERVICE_NAME = "WorkAreaModule";
		public event EventHandler<DeviceLabelEvent> WorkAreaVisitEventHandler;

		private pgworkDBEntities db;
		private WorkDayModule workDayModule;
		private AreaModule areaModule;

		public void init()
		{
			db = new pgworkDBEntities();
			workDayModule = (WorkDayModule)Core.Instance.getService(WorkDayModule.SERVICE_NAME);
			areaModule = (AreaModule)Core.Instance.getService(AreaModule.SERVICE_NAME);

			areaModule.AreaVisitEventHandler += _workAreaModule_AreaVisitEvent;
		}

		private void _workAreaModule_AreaVisitEvent(object sender, AreaVisitEvent e)
		{
			WorkDay wd = workDayModule.currentWorkDay;

		}
	}
}