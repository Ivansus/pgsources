using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pg_web.Models;
using System.Data.Entity;
using System.Threading;

namespace pg_web.sys
{
    public class EventSystem
    {
        private pgworkDBEntities db = new pgworkDBEntities();
        private Thread m_systemThread;

        public EventSystem()
        {
            System.Diagnostics.Trace.Write("EventSystem::ctor");
            m_systemThread = new Thread(this.doWork);

            m_systemThread.Start("thread data");
        }

        public void doWork(object _data)
        {
			/*
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5000);
                Log newLog = db.Logs.Create();
                newLog.logString = "Event system thread: " + i;
                db.Logs.Add(newLog);
                db.SaveChanges();
                System.Diagnostics.Trace.Write("EventSystem::thread : " + i);
            }
			*/
        }
    }
}