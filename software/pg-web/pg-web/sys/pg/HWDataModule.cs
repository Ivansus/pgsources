using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using pg_web.sys.pg.hw;
using System.IO;

namespace pg_web.sys.pg
{

	public class DataEventArgs : EventArgs
	{
		private Packet m_packet;
		public DataEventArgs(Packet _packet)
		{
			m_packet = _packet;
		}

		public Packet packet {
			get { return m_packet; }
		}

	}
	public class HWDataModule : IModule, DataConnector.IProcessor{
		public const String SERVICE_NAME = "HWData";

		private DataConnector m_comPort;

		public event EventHandler<DataEventArgs> LabelInfoEvent;
		public event EventHandler<DataEventArgs> LabelBroadcastEvent;
        public event EventHandler<DataEventArgs> WorkerDeviceLabelEvent;

        public HWDataModule()
		{

		}

		public void init()
		{
			System.Diagnostics.Debug.Write("---- HWDataModule ---- init");
			//m_comPort = new DataConnector("COM1", 9600, this);
			m_comPort = new DataConnector("COM8", 19200, this);
			m_comPort.start();
		}

		public void parsePacket(byte[] _data, uint _uDataSize) {
			ushort uDeviceId = BitConverter.ToUInt16(_data, 0);
			PGVersion version = new PGVersion(_data, 2);
			byte btPacketType = _data[8];

			System.Diagnostics.Debug.Write("packet, device id:" + uDeviceId + ", type: " + btPacketType + ", version: " + version);
			// TODO: validate packet version!!!
			switch ((Packet.PacketTypes)btPacketType)
			{
				case Packet.PacketTypes.pgLabelInfoPacket:
						_onPacketEvent(new LabelInfoPacket(uDeviceId, version, _data, _uDataSize), LabelInfoEvent);
					break;
				case Packet.PacketTypes.pgLabelBroadcastPacket:
						_onPacketEvent(new LabelBroadcastPacket(uDeviceId, version, _data, _uDataSize), LabelBroadcastEvent);
					break;
                case Packet.PacketTypes.pgWorkerDeviceLabelPacket:
                    _onPacketEvent(new WorkerDeviceLabelPacket(uDeviceId, version, _data, _uDataSize), WorkerDeviceLabelEvent);
                    break;
                default:
					return;
			}

		}

		private void _onPacketEvent(Packet _packet, EventHandler<DataEventArgs> _handler)
		{
			if (_handler != null)
				_handler(this, new DataEventArgs(_packet));
		}

		private byte[] m_sendBuffer = new byte[DataConnector.mc_uPacketSize];
		BinaryWriter m_writer = null;
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public bool sendPacket(Packet _packet)
		{
			if (m_writer == null)
				m_writer = new BinaryWriter(new MemoryStream(m_sendBuffer));
			_clearSendBuffer();
			m_writer.Seek(0, SeekOrigin.Begin);
			_packet.write(m_writer);
			m_writer.Flush();
			return m_comPort.sendData(m_sendBuffer, 0U, DataConnector.mc_uPacketSize);
		}
		private void _clearSendBuffer()
		{
			for (uint i = 0; i < DataConnector.mc_uPacketSize; ++i)
				m_sendBuffer[i] = 0xAA;
		}

	}
}