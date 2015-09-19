using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pg_web.Models;
using System.Diagnostics;

namespace pg_web.sys.pg
{
	public class WorkDayEventArgs : EventArgs
	{
		public WorkDayEventArgs(WorkDay _day)
		{
			m_workDay = _day;
		}
		private WorkDay m_workDay;
		public WorkDay workDay
		{
			get { return m_workDay; }
		}
	}

	public class WorkDayTimer : IModule
	{
		public const String SERVICE_NAME = "WorkDayTimer";
		private const double UpdateTimerout = 60 * 1000;
		private pgworkDBEntities db;

		private WorkDay m_currentWorkDay;

		private System.Timers.Timer m_updateTimer;
		private System.Timers.Timer m_startTimer;
		private System.Timers.Timer m_stopTimer;


		public event EventHandler<WorkDayEventArgs> WorkDayStartEvent;
		public event EventHandler<WorkDayEventArgs> WorkDayStopEvent;

		public WorkDayTimer()
		{
			db = new pgworkDBEntities();

			m_updateTimer = new System.Timers.Timer(UpdateTimerout);
			m_updateTimer.Elapsed += this.timer_updateWorkDay;
			m_updateTimer.Enabled = true;

			m_startTimer = new System.Timers.Timer(UpdateTimerout);
			m_startTimer.Elapsed += timer_startWorkDay;
			m_startTimer.Enabled = false;

			m_stopTimer = new System.Timers.Timer(UpdateTimerout);
			m_stopTimer.Elapsed += timer_stopWorkDay;
			m_stopTimer.Enabled = false;
		}

		public void init() { }

		private WorkDay _findCurrentWorkDay()
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

			try {
				return (
					from m in db.WorkDays
					where m.startTime <= unixTimestamp && m.endTime > unixTimestamp
					select m
				).First<WorkDay>();
			} catch (Exception e)
			{
				return null;
			}
		}

		private WorkDay _findNextWorkDay()
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			try
			{
				return (
					from m in db.WorkDays
					where m.startTime > unixTimestamp
					orderby m.startTime ascending
					select m
				).First<WorkDay>();
			}
			catch (Exception e)
			{
				return null;
			}
		}


		private void _startWorkDay(WorkDay _day)
		{
			m_startTimer.Enabled = false;

			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			m_currentWorkDay = _day;
			m_currentWorkDay.workDayState = (int)WorkDay.State.stActive;
			_riseWorkDayStart(m_currentWorkDay);
			m_stopTimer.Interval = m_currentWorkDay.startTime - unixTimestamp;
			m_stopTimer.Enabled = true;
		}

		private void _stopWorkDay()
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			WorkDay nextDay = _findNextWorkDay();
			if (nextDay != null) {
				m_startTimer.Interval = nextDay.startTime - unixTimestamp;
				m_startTimer.Enabled = true;
			}
			if (m_currentWorkDay == null)
				return;

			m_stopTimer.Enabled = false;

			m_currentWorkDay.workDayState = (int)WorkDay.State.stFinished;
			_riseWorkDayEnd(m_currentWorkDay);
			m_currentWorkDay = null;

			if (nextDay != null) {
				m_startTimer.Interval = nextDay.startTime - unixTimestamp;
				m_startTimer.Enabled = true;
			}
		}

		private void timer_startWorkDay(object _sender, System.Timers.ElapsedEventArgs _args)
		{
			Debug.Assert(m_currentWorkDay == null);

			WorkDay day = _findCurrentWorkDay();
			Debug.Assert(day != null);
			if (day != null)
				_startWorkDay(day);
			db.SaveChangesAsync();
		}

		private void timer_stopWorkDay(object _sender, System.Timers.ElapsedEventArgs _args)
		{

			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			WorkDay day = _findCurrentWorkDay();
			WorkDay nextDay = _findNextWorkDay();

			if (day == null || day.workDayId == m_currentWorkDay.workDayId) {
				_stopWorkDay();
			}
			else {
				_stopWorkDay();
				_startWorkDay(day);
			}
			db.SaveChangesAsync();
		}

		private void timer_updateWorkDay(object _sender, System.Timers.ElapsedEventArgs _args)
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			WorkDay day = _findCurrentWorkDay();
			WorkDay nextDay = _findNextWorkDay();

			if (day == null && m_currentWorkDay == null)
				_stopWorkDay();
			else if (day == null && m_currentWorkDay != null)
				_stopWorkDay();
			else if (day != null && m_currentWorkDay == null)
				_startWorkDay(day);
			else if (m_currentWorkDay.workDayId != day.workDayId) {
				_stopWorkDay();
				_startWorkDay(day);
			}
			db.SaveChangesAsync();
		}

		private void _riseWorkDayStart(WorkDay _day)
		{
			EventHandler<WorkDayEventArgs> handler = WorkDayStartEvent;
			if (handler != null)
				handler(this, new WorkDayEventArgs(_day));
		}

		private void _riseWorkDayEnd(WorkDay _day)
		{
			EventHandler<WorkDayEventArgs> handler = WorkDayStartEvent;
			if (handler != null)
				handler(this, new WorkDayEventArgs(_day));
		}
	}
}