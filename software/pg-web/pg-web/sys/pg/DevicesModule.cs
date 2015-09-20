using pg_web.Models;
using pg_web.sys.pg.hw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pg_web.sys.pg {
	public class DevicesModule : IModule {
		public const String SERVICE_NAME = "Devices";

		private HWDataModule m_dataModule;
		private int m_nIndex = 0;
		private pgworkDBEntities db;

		public void init () {
			m_dataModule = (HWDataModule)Core.Instance.getService(HWDataModule.SERVICE_NAME);
			_startOpenTimer();
			m_dataModule.WorkerDeviceLabelEvent += _dataModule_WorkerDeviceLabelEvent;
			db = new pgworkDBEntities();
		}

		private Device _findOrCreate(ushort _uDeviceId) {
			try {
				return (
					from m in db.Devices
					where m.deviceData == _uDeviceId
					select m
				).First<Device>();
			} catch (Exception) {
				Device dev = db.Devices.Create();
				dev.deviceData = _uDeviceId;
				dev.deviceName = "Unknown device: 0x" + _uDeviceId.ToString("X4");
				dev.deviceType = 1;
				db.Devices.Add(dev);
				db.SaveChanges();
				System.Diagnostics.Debug.Write("\n New Device device id:" + _uDeviceId);
				return dev;
			}
		}

		private void _dataModule_WorkerDeviceLabelEvent(object sender, DataEventArgs e) {
			WorkerDeviceLabelPacket packet = (WorkerDeviceLabelPacket)e.packet;
			System.Diagnostics.Debug.Write("\n device label packet: device id:" + packet.m_uDeviceId + ", label id: " + packet.m_uLabelId);
			try {
				Device dev = (
					from m in db.Devices
					where m.deviceData == packet.m_uDeviceId
					select m
				).First<Device>();
				System.Diagnostics.Debug.Write("\n device already exist device id:" + packet.m_uDeviceId);
			} catch (Exception) {
				Device dev = db.Devices.Create();
				dev.deviceData = packet.m_uDeviceId;
				dev.deviceName = "name";
				dev.deviceType = 1;
				db.Devices.Add(dev);
				db.SaveChanges();
				System.Diagnostics.Debug.Write("\n New Device device id:" + packet.m_uDeviceId);
			}
		}

		private System.Timers.Timer m_openTimer;
		private void _startOpenTimer() {
			if (m_openTimer != null)
				return;
			m_openTimer = new System.Timers.Timer(10000);
			m_openTimer.Elapsed += _onOpenTimer;
			m_openTimer.Enabled = true;
		}

		private void _onOpenTimer(object _sender, System.Timers.ElapsedEventArgs _args) {
			m_nIndex++;
			_stopOpenTimer();
			System.Diagnostics.Debug.Write("----------------DevicesModule-----------------\n");
			m_dataModule.sendPacket(new hw.DeviceMessagePacket(0x0200, 0, m_nIndex+"!!!"));
			_startOpenTimer();
		}

		private void _stopOpenTimer() {
			if (m_openTimer == null)
				return;
			m_openTimer.Stop();
			m_openTimer.Dispose();
			m_openTimer = null;
		}
	}
}