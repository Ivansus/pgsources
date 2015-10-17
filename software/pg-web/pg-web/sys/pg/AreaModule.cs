using pg_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pg_web.sys.pg
{
	public class AreaVisitEvent : EventArgs
	{
		public AreaVisitEvent(DeviceLabelEvent _labelEvent,  Area _area, LastAreaAccess _areaAccess, int _nTimeDelay)
		{
			device = _labelEvent.device;
			label = _labelEvent.label;
			signal = _labelEvent.signal;
			area = _area;
			areaAccess = _areaAccess;
			timeDelay = _nTimeDelay;
		}
		public Device device { get; }
		public Label label { get; }
		public int signal { get; }
		public Area area { get; }
		public LastAreaAccess areaAccess { get; }
		public int timeDelay { get; }
	}

	class AreaModule : IModule
	{
		public const String SERVICE_NAME = "Areas";

		private pgworkDBEntities db;

		public event EventHandler<AreaVisitEvent> AreaVisitEventHandler;

		public void init()
		{
			db = new pgworkDBEntities();
			DevicesModule deviceModule = (DevicesModule)Core.Instance.getService(DevicesModule.SERVICE_NAME);
			deviceModule.DeviceLabelEventHandler += _deviceModule_WorkerDeviceLabelEvent;
		}

		public Area findArea(int _nAreaId)
		{
			try
			{
				return (
					from m in db.Areas
					where m.areaId == _nAreaId
					select m
				).First<Area>();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public LastAreaAccess findOrCreateLastAreaAccess(Area _area, Device _device)
		{
			try
			{
				return (
					from m in db.LastAreaAccesses
					where m.areaId == _area.areaId && m.deviceId == _device.deviceId
					select m
				).First<LastAreaAccess>();
			}
			catch (Exception)
			{
				TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
				LastAreaAccess record = db.LastAreaAccesses.Create();
				record.Area = _area;
				record.Device = _device;
				record.accessTime = (int)span.TotalSeconds;
				db.LastAreaAccesses.Add(record);
				db.SaveChanges();
				System.Diagnostics.Debug.Write("\n Create new area access, area:" + _area.areaId + ", device: " + _device.deviceId + ", devicename: '" + _device.deviceName + "'");
				return record;
			}
		}

		private void _deviceModule_WorkerDeviceLabelEvent(object _sender, DeviceLabelEvent _e)
		{
			Area area = findArea(_e.label.areaId);
			if (area == null)
			{
				System.Diagnostics.Debug.Write("\n Device found label from unknown area: label id" + _e.label.labelId + ", labelData: " + _e.label.labelData + ", areaId: " + _e.label.areaId);
				return;
			}
			TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
			int nTimeNow = (int)span.TotalSeconds;
			LastAreaAccess areaAccess = findOrCreateLastAreaAccess(area, _e.device);
			int nAccessDelay = nTimeNow - areaAccess.accessTime;
			areaAccess.accessTime = nTimeNow;
			db.SaveChanges();
			AreaVisitEventHandler(this, new AreaVisitEvent(_e, area, areaAccess, nAccessDelay));
		}
	}
}
