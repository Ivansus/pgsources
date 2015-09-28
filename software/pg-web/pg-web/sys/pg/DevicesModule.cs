using pg_web.Models;
using pg_web.sys.pg.hw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pg_web.sys.pg {

	public class DeviceLabelEvent : EventArgs {
		public DeviceLabelEvent(Device _device, Label _label, int _nSignalPower) {
			device = _device;
			label = _label;
			signal = _nSignalPower;
		}
		public Device device { get; }
		public Label label { get; }
		public int signal { get; }
	}

	public class DevicesModule : IModule {
		public const String SERVICE_NAME = "Devices";

		public event EventHandler<DeviceLabelEvent> DeviceLabelEventHandler;

		private HWDataModule m_dataModule;
		private LabelModule m_labelModule;
		private int m_nIndex = 0;
		private pgworkDBEntities db;

		public void init () {
			m_dataModule = (HWDataModule)Core.Instance.getService(HWDataModule.SERVICE_NAME);
			m_labelModule = (LabelModule)Core.Instance.getService(LabelModule.SERVICE_NAME);
			_startOpenTimer();
			m_dataModule.WorkerDeviceLabelEvent += _dataModule_WorkerDeviceLabelEvent;
			db = new pgworkDBEntities();
		}

		public Device findDevice(ushort _uDeviceId)
		{
			try
			{
				return (
					from m in db.Devices
					where m.deviceData == _uDeviceId
					select m
				).First<Device>();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Device findOrCreateDevice(ushort _uDeviceId) {
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
			Device dev = findOrCreateDevice(packet.m_uDeviceId);
			Label label = m_labelModule.findOrCreateLabel(packet.m_uLabelId);
			DeviceLabelEventHandler(this, new DeviceLabelEvent(dev, label, packet.m_btSignalPower));
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