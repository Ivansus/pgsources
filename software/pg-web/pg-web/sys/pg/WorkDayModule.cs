using pg_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pg_web.sys.pg
{
	class WorkDayModule : IModule
	{
		public const String SERVICE_NAME = "WorkDay";
		private pgworkDBEntities db;

		public void init()
		{
			db = new pgworkDBEntities();
			try
			{
				currentWorkDay = (
					from m in db.WorkDays
					where m.workDayState == WorkDayState.wdstInProgress
					select m
				).First<WorkDay>();
			}
			catch (Exception)
			{
				currentWorkDay = null;
			}

			_initTimers();
		}

		public WorkDay currentWorkDay { get; private set; }

		private void _initTimers()
		{

		}
	}
}
