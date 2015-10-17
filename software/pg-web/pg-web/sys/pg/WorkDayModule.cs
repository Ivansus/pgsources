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
		}
	}
}
