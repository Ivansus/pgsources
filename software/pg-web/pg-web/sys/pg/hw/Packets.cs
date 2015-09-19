using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.IO;

namespace pg_web.sys.pg.hw
{
	public class PGVersion
	{
		public byte btHardwareMajor;
		public byte btHardwareMinor;
		public byte btSoftwareMajor;
		public byte btSoftwareMinor;
		public ushort wBuild;

		public PGVersion(byte _btHWMaj, byte _btHWMin, byte _btSwMaj, byte _btSwMin, ushort _wBuild)
		{
			btHardwareMajor = _btHWMaj;
			btHardwareMinor = _btHWMin;
			btSoftwareMajor = _btSwMaj;
			btSoftwareMinor = _btSwMin;
			wBuild = _wBuild;
		}

		public PGVersion(byte[] _data, uint _uOffset)
		{
			btHardwareMajor = _data[0];
			btHardwareMinor = _data[1];
			btSoftwareMajor = _data[2];
			btSoftwareMinor = _data[3];
			wBuild = BitConverter.ToUInt16(_data, 4);
		}

		public void write(BinaryWriter _writer)
		{
			_writer.Write(btHardwareMajor);
			_writer.Write(btHardwareMinor);
			_writer.Write(btSoftwareMajor);
			_writer.Write(btSoftwareMinor);
			_writer.Write(wBuild);
		}

		override public string ToString()
		{
			return btHardwareMajor + "." + btHardwareMinor + "." + btSoftwareMajor + "." + btSoftwareMinor + "." + wBuild;
		}
	}

	public abstract class Packet
	{
		public enum PacketTypes
		{
			pgLabelInfoPacket = 0x00, // Label => Server. +
			pgLabelBroadcastPacket = 0x01, // Label => All. +
			pgLabelControlPacket = 0x80, // Server => Label. -

			pgWorkerDeviceInfoPacket = 0x40, // Worker Device (clock) => Server. -
			pgWorkerDeviceControlPacket = 0x81, // Server => Worker Device (clock). -
			pgWorkerDeviceLabelPacket = 0x41, // Worker Device (clock) => Server. +
			pgWorkerDeviceMessagePacket = 0x82, // Server => Worker Device (clock). -

			pgCookStationSignalPacket = 0x20, // Cook station => Server. -
			pgCookStationControlPacket = 0x83, // Server => Cook station. -
		};

		//const Version mc_currentVersion = new Version(0, 0, 0, 0, 0);

		protected PacketTypes m_type;

		protected PGVersion m_version;

		protected Packet(PGVersion _version, PacketTypes _type)
		{
			m_version = _version;
			m_type = _type;
		}

		public PacketTypes Type
		{
			get
			{
				return m_type;
			}
		}

		public PGVersion Ver
		{
			get
			{
				return m_version;
			}
		}


		private static PGVersion ms_version;
		public static PGVersion version {
			get {
				if (ms_version == null)
					ms_version = new PGVersion(1, 0, 0, 1, 0);
				return ms_version;
			}
		}

		public abstract void write(BinaryWriter _writer);
	}

	// pgLabelInfoPacket = 0x00, // Label => Server.
	public class LabelInfoPacket : Packet
	{

		private ushort m_uSourceId;
		private ushort m_uBatteryVoltage;
		private byte m_btState;

		public LabelInfoPacket(ushort _uDeviceId, PGVersion _version, byte[] _data, uint _uPacketSize)
			: base(_version, Packet.PacketTypes.pgLabelInfoPacket)
		{
			m_uSourceId = _uDeviceId;
			m_uBatteryVoltage = BitConverter.ToUInt16(_data, 9);
			m_btState = _data[11];
		}

		public override void write(BinaryWriter _writer)
		{
			System.Diagnostics.Debug.Assert(false, "This packet never sends from server.");
		}
	}

	// pgLabelBroadcastPacket = 0x01, // Label => All.
	public class LabelBroadcastPacket : Packet
	{
		private ushort m_uSourceId;
		public LabelBroadcastPacket(ushort _uDeviceId, PGVersion _version, byte[] _data, uint _uPacketSize)
			: base(_version, Packet.PacketTypes.pgLabelBroadcastPacket)
		{ m_uSourceId = _uDeviceId; }

		public override void write(BinaryWriter _writer)
		{
			System.Diagnostics.Debug.Assert(false, "This packet never sends from server.");
		}
	}

	// pgLabelControlPacket = 0x80, // Server => Label.
	public class LabelControlPacket : Packet
	{
		private ushort m_uTargetId;
		private byte m_btState;

		public LabelControlPacket(ushort _uLabelId, byte _btState)
			: base(Packet.version, Packet.PacketTypes.pgLabelControlPacket)
		{
			m_uTargetId = _uLabelId;
			m_btState = _btState;
		}

		public override void write(BinaryWriter _writer)
		{
			_writer.Write(m_uTargetId);
			m_version.write(_writer);
			_writer.Write((byte)m_type);
			_writer.Write(m_btState);

		}
	}

	// pgWorkerDeviceInfoPacket = 0x40, // Worker Device => Server. -
	public class WorkerDeviceInfoPacket : Packet {
		public ushort m_uDeviceId;
		public ushort m_uBatteryVoltage;
		public byte m_btCharging;
		public byte m_btLabelMinSignalLevel;
		public byte m_btLabelObserveTimeoutSec;

		public WorkerDeviceInfoPacket(ushort _uDeviceId, PGVersion _version, byte[] _data, uint _uPacketSize)
			: base(_version, Packet.PacketTypes.pgWorkerDeviceInfoPacket)
		{
			m_uDeviceId = _uDeviceId;
			m_uBatteryVoltage = BitConverter.ToUInt16(_data, 9);
			m_btCharging = _data[11];
			m_btLabelMinSignalLevel = _data[12];
			m_btLabelObserveTimeoutSec = _data[13];
			System.Diagnostics.Debug.Write("----WorkerDeviceInfoPacket --" + m_uDeviceId + ":" + m_btCharging + ": " + m_btLabelMinSignalLevel + " : " + m_btLabelObserveTimeoutSec + "\n");
		}

		public override void write(BinaryWriter _writer)
		{
			System.Diagnostics.Debug.Assert(false, "This packet never sends from server.");
		}
	}

	// pgWorkerDeviceLabelPacket = 0x41, // Worker Device (clock) => Server. -
	public class WorkerDeviceLabelPacket : Packet {
		public ushort m_uDeviceId;
		public ushort m_uLabelId;
		public byte m_btSignalPower;

		public WorkerDeviceLabelPacket(ushort _uDeviceId, PGVersion _version, byte[] _data, uint _uPacketSize)
			: base(_version, Packet.PacketTypes.pgWorkerDeviceLabelPacket)
		{
			m_uDeviceId = _uDeviceId;
			m_uLabelId = BitConverter.ToUInt16(_data, 9);
			m_btSignalPower = _data[11];
			System.Diagnostics.Debug.Write("----WorkerDeviceLabelPacket --"+ m_uDeviceId + ":" + m_uLabelId + ": " + m_btSignalPower + "\n");
		}

		public override void write(BinaryWriter _writer) {
			System.Diagnostics.Debug.Assert(false, "This packet never sends from server.");
		}
	}

	/*
typedef struct {
	uint16_t AdressDst;
	uint8_t btPacketType; // == pgWorkerDeviceMessagePacket
	uint8_t btMessageMask;
	uint8_t strMessage[18]; // null terminated, cp1251 encoded string.
}WorkerDeviceMessagePacket ;
	*/

	// pgWorkerDeviceMessagePacket = 0x82, // Server => Worker Device (clock). -
	public class DeviceMessagePacket : Packet
	{
		private ushort m_uTargetId;
		private byte m_btMessageMask;
		private string m_strMessage;

		public DeviceMessagePacket(ushort _uTarget, byte _btMask, string _strMessage)
			: base(Packet.version, Packet.PacketTypes.pgWorkerDeviceMessagePacket)
		{
			m_uTargetId = _uTarget;
			m_btMessageMask = _btMask;
			m_strMessage = _strMessage;
		}

		public override void write(BinaryWriter _writer)
		{
			_writer.Write(m_uTargetId);
			_writer.Write((byte)PacketTypes.pgWorkerDeviceMessagePacket);
			_writer.Write(m_btMessageMask);

			System.Text.Encoding srcEncodingFormat = System.Text.Encoding.UTF8;
			System.Text.Encoding dstEncodingFormat = System.Text.Encoding.GetEncoding("windows-1251");
			byte[] originalByteString = srcEncodingFormat.GetBytes(m_strMessage);
			byte[] convertedByteString = System.Text.Encoding.Convert(
				srcEncodingFormat,
				dstEncodingFormat, originalByteString
			);
			_writer.Write(convertedByteString);
			_writer.Write((byte)0);
			//string finalString = dstEncodingFormat.GetString(convertedByteString);
		}
	}
}