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

		private System.Timers.Timer m_updateTimer;
		private System.Timers.Timer m_startTimer;
		private System.Timers.Timer m_stopTimer;

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
			m_updateTimer = new System.Timers.Timer();
			m_updateTimer.Elapsed += _updateWorkdayTimer;
			m_startTimer = new System.Timers.Timer();
			m_startTimer.Elapsed += _startWorkdayTimer;
			m_stopTimer = new System.Timers.Timer();
			m_stopTimer.Elapsed += _stopWorkdayTimer;

			int nTimeNow = Core.now();

			if (currentWorkDay == null)
			{
				// create start timer.
				try
				{
					WorkDay nextWorkDay = (
						from m in db.WorkDays
						where m.workDayState == WorkDayState.wdstNotStarted && m.endTime > nTimeNow
						orderby m.startTime
						select m
					).First<WorkDay>();
					if (nextWorkDay.startTime < nTimeNow)
						m_startTimer.Interval = 5 * 1000;
					else
						m_startTimer.Interval = nextWorkDay.startTime - nTimeNow;
					m_startTimer.Start();
				}
				catch (Exception)
				{
				}
			}
			else
			{
				// create stop timer
				if (currentWorkDay.endTime > nTimeNow)
				{
					// stop work day with 5 second delay
					m_stopTimer.Interval = 5 * 1000;
					m_stopTimer.Start();
				}
				else
				{
					m_stopTimer.Interval = (nTimeNow - currentWorkDay.endTime) * 1000;
					m_stopTimer.Start();
				}
			}
			// Check work day state every 10 sec.
			m_updateTimer.Interval = 10000;
			m_updateTimer.Start();
		}

		private void _updateWorkdayTimer(object _sender, System.Timers.ElapsedEventArgs _args) {
		}

		private void _startWorkdayTimer(object _sender, System.Timers.ElapsedEventArgs _args) {
			m_startTimer.Stop();
		}

		private void _stopWorkdayTimer(object _sender, System.Timers.ElapsedEventArgs _args) {
			m_stopTimer.Stop();
		}
	}
}
