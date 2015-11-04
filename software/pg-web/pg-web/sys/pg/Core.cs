using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pg_web.sys.pg
{
	public class Core {
		private static Core instance;
		private Dictionary<String, IModule> m_services = new Dictionary<String, IModule>();
		private Core() {}

		public void init() {
			System.Diagnostics.Debug.Write("---- core ---- init");
			registerService(WorkDayTimer.SERVICE_NAME, new WorkDayTimer());
			registerService(DevicesModule.SERVICE_NAME, new DevicesModule());
			registerService(HWDataModule.SERVICE_NAME, new HWDataModule());
			registerService(LabelModule.SERVICE_NAME, new LabelModule());
			registerService(AreaModule.SERVICE_NAME, new AreaModule());
			registerService(WorkDayModule.SERVICE_NAME, new WorkDayModule());
			registerService(WorkAreaModule.SERVICE_NAME, new WorkAreaModule());

			foreach (KeyValuePair<String, IModule> pair in m_services) {
				pair.Value.init();
			}
		}

		public void registerService(String _strName, IModule _service)
		{
			System.Diagnostics.Debug.Assert(!m_services.ContainsKey(_strName));
			m_services[_strName] = _service;
		}

		public object getService(String _strName)
		{
			if (!m_services.ContainsKey(_strName))
				return null;
			return m_services[_strName];
		}

		public static Core Instance
		{
			get
			{
				if (instance == null)
					instance = new Core();
				return instance;
			}
		}

		public static int now()
		{
			return (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}
	}
}