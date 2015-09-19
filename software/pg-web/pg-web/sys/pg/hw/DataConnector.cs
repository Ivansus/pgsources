using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO.Ports;
using System.IO;

namespace pg_web.sys.pg.hw
{
	public class DataConnector
	{
		private SerialPort m_serialPort = null;
		private String m_strPortName = "COM8";
		private int m_nBaundRate = 19200;

		public const uint mc_uPacketSize = 22;
		const int mc_nOpenTimeoutMs = 10 * 1000;

		private byte[] m_dataBuffer = new byte[mc_uPacketSize];
		private uint m_uFilledSize = 0;

		public interface IProcessor
		{
			void parsePacket(byte[] _data, uint _uDataSize);
		};
		private IProcessor m_packetProcessor;

		public DataConnector(String _strPort, int _nBaundRate, IProcessor _packetProcessor)
		{
			m_packetProcessor = _packetProcessor;
			m_strPortName = _strPort;
			m_nBaundRate = _nBaundRate;
		}

		public void start()
		{
			if (!_openPort())
				_stopOpenTimer();
		}

		public bool sendData(byte[] _packetData, uint _uOffset, uint _uSize)
		{
			System.Diagnostics.Debug.Assert(m_serialPort != null);
			m_serialPort.Write(_packetData, (int)_uOffset, (int)_uSize);
			return true;
		}

		public bool isValid()
		{
			return m_serialPort != null;
		}
		public void stop()
		{
			if (m_serialPort != null)
			{
				m_serialPort.Close();
				m_serialPort.Dispose();
			}
			_stopOpenTimer();
		}

		private bool _openPort()
		{
			try
			{
				m_serialPort = new SerialPort();
				m_serialPort.BaudRate = m_nBaundRate;
				m_serialPort.DataBits = 8;
				m_serialPort.Handshake = Handshake.None;
				m_serialPort.Parity = Parity.None;
				m_serialPort.PortName = m_strPortName;
				m_serialPort.StopBits = StopBits.One;
				m_serialPort.DataReceived += new SerialDataReceivedEventHandler(this._onData);
				m_serialPort.Open();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.Write("_openPort error: " + e);
				return false;
			}
			try { m_serialPort.DtrEnable = true; } catch { }
			try { m_serialPort.RtsEnable = true; } catch { }

			_stopOpenTimer();
			return true;
		}

		private System.Timers.Timer m_openTimer;
		private void _startOpenTimer()
		{
			if (m_openTimer != null)
				return;
			m_openTimer = new System.Timers.Timer(mc_nOpenTimeoutMs);
			m_openTimer.Elapsed += _onOpenTimer;
			m_openTimer.Enabled = true;

		}

		private void _stopOpenTimer()
		{
			if (m_openTimer == null)
				return;
			m_openTimer.Stop();
			m_openTimer.Dispose();
			m_openTimer = null;
		}

		private void _onOpenTimer(object _sender, System.Timers.ElapsedEventArgs _args)
		{
			if (_openPort())
				_stopOpenTimer();
		}

		private void _onData(object _sender, SerialDataReceivedEventArgs _e)
		{
			if (!_sender.Equals(m_serialPort))
				return;
			System.Diagnostics.Debug.WriteLine("DataConnector on com port data.");
			uint nBytesToRead = (uint)m_serialPort.BytesToRead;
			while (nBytesToRead > 0)
			{
				uint nReadSize = (uint)m_serialPort.Read(m_dataBuffer, (int)m_uFilledSize, (int)Math.Min(mc_uPacketSize - m_uFilledSize, nBytesToRead));
				m_uFilledSize += nReadSize;
				nBytesToRead -= nReadSize;
				if (m_uFilledSize == mc_uPacketSize) {
					System.Diagnostics.Debug.WriteLine("Packet Data: " + m_dataBuffer);
					m_packetProcessor.parsePacket(m_dataBuffer, mc_uPacketSize);
					m_uFilledSize = 0;
				}
			}
		}

		private void _onError(object _sender, SerialErrorReceivedEventArgs _args)
		{
			if (!_sender.Equals(m_serialPort))
				return;
			stop();
			_stopOpenTimer();
		}
	}
}