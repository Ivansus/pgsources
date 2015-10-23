using pg_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pg_web.sys.pg
{
	class WorkDayStartEvent : EventArgs
	{
		public WorkDayStartEvent(WorkDay _workDay) {
			workDay = _workDay;
		}

		public WorkDay workDay { get; }
	}

	class WorkDayStopEvent : EventArgs
	{
		public WorkDayStopEvent(WorkDay _workDay)
		{
			workDay = _workDay;
		}

		public WorkDay workDay { get; }

	}

	class WorkDayModule : IModule
	{
		public const String SERVICE_NAME = "WorkDay";
		private pgworkDBEntities db;

		private System.Timers.Timer m_updateTimer;
		private System.Timers.Timer m_startTimer;
		private System.Timers.Timer m_stopTimer;

		public event EventHandler<WorkDayStartEvent> WorkDayStartEventHandler;
		public event EventHandler<WorkDayStopEvent> WorkDayStopEventHandler;

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

		private void _startWorkDay(WorkDay _workDay)
		{
			_workDay.workDayState = WorkDayState.wdstInProgress;
			db.SaveChanges();
			WorkDayStartEventHandler(this, new WorkDayStartEvent(_workDay));
		}

		private void _stopWorkDay(WorkDay _workDay)
		{
			_workDay.workDayState = WorkDayState.wdstFinished;
			db.SaveChanges();
			WorkDayStopEventHandler(this, new WorkDayStopEvent(_workDay));
		}

		private void _finishAllWorkDays(WorkDay _excludeWorkDay)
		{
			try
			{
				List<WorkDay> startedWorkDays = (
					from m in db.WorkDays
					where m.workDayState == WorkDayState.wdstInProgress && m.startTime > Core.now() && m.endTime < Core.now()
					select m
				).ToList<WorkDay>();

				foreach (WorkDay _day in startedWorkDays)
				{
					if (_excludeWorkDay != null && _day.workDayId == _excludeWorkDay.workDayId)
						continue;
					_stopWorkDay(_day);
				}
			}
			catch (Exception)
			{
				currentWorkDay = null;
			}
		}
	}
}
