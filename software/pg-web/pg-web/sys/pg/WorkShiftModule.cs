using pg_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pg_web.sys.pg
{
	class WorkShiftModule : IModule
	{
		public const String SERVICE_NAME = "WorkShiftModule";

		private pgworkDBEntities db;
		private WorkDayModule workDayModule;
		private WorkAreaModule workAreaModule;

		public void init()
		{
			db = new pgworkDBEntities();
			workDayModule = (WorkDayModule)Core.Instance.getService(WorkDayModule.SERVICE_NAME);
			workAreaModule = (WorkAreaModule)Core.Instance.getService(WorkAreaModule.SERVICE_NAME);
			// TODO: start timers for initialize and deinitialize work shifts.
		}
	}
}
