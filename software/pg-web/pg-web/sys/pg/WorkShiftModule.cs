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

			workAreaModule.WorkAreaVisitEventHandler += _workShiftModule_process_WorkAreaVisitEvent;
			// TODO: start timers for initialize and deinitialize work shifts.
		}

		private void _workShiftModule_process_WorkAreaVisitEvent(Object _sender, WorkAreaVisitEvent _event)
		{
			try
			{
				WorkEmployer we = (from w in db.WorkEmployers
					where w.deviceId == _event.device.deviceId
					select w).First<WorkEmployer>();
				List<WorkShift> shiftsToSignaled = (from shift in db.WorkShifts
					join accessEmployers in db.WorkShiftAccessEmployers on shift.workShiftId equals accessEmployers.workShiftId
					where shift.workAreaId == _event.workArea.workAreaId &&
					accessEmployers.workEmployersId == we.employerId &&
					shift.shiftState == WorkShiftState.wssActive
					select shift).ToList<WorkShift>();
				foreach(WorkShift shift in shiftsToSignaled)
					_resetWorkShift(shift, we, _event);

			}
			catch (Exception)
			{
				System.Diagnostics.Debug.Write("Exception on getting wirk shifts for work area vivst event '" + _event.workArea.workAreaName + "'.");
			}
		}

		private void _resetWorkShift(WorkShift _shift, WorkEmployer _employer, WorkAreaVisitEvent _event)
		{

		}
	}

}
