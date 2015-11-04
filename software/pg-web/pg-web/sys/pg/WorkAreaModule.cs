using pg_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pg_web.sys.pg
{
	public class WorkAreaVisitEvent : EventArgs
	{
		public WorkAreaVisitEvent(WorkDay _workDay, WorkArea _workArea, AreaVisitEvent _event)
		{
			workDay = _workDay;
			workArea = _workArea;
			device = _event.device;
			areaEvent = _event;
		}

		public WorkDay workDay { get; }
		public WorkArea workArea { get; }
		public Device device { get; }
		public AreaVisitEvent areaEvent { get; }
	}

	public class WorkAreaModule : IModule
	{
		public const String SERVICE_NAME = "WorkAreaModule";
		public event EventHandler<WorkAreaVisitEvent> WorkAreaVisitEventHandler;

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

		private void _workAreaModule_AreaVisitEvent(object _sender, AreaVisitEvent _e)
		{
			WorkDay wd = workDayModule.currentWorkDay;
			if (wd == null)
			{
				System.Diagnostics.Debug.Write("Area visit event with no started work day.");
				return;
			}
			try
			{
				WorkArea wa = (
					from m in db.WorkAreas
					where m.workDayId == wd.workDayId &&
					m.areaId == _e.area.areaId
					select m
				).First<WorkArea>();
				WorkAreaVisitEventHandler(this, new WorkAreaVisitEvent(wd, wa, _e));
			}
			catch (Exception)
			{
				System.Diagnostics.Debug.Write("There is no work area associated with '" + _e.area.areaName +"' in current work day (" + wd.workDayId + ")");
			}
		}
	}
}