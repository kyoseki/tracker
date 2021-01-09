using System;
using System.IO.Ports;
using osu.Framework.Logging;

namespace kyoseki.Game.Serial
{
    public class SystemSerialPort : SerialPort, ISerialPort
    {
        public string Name => base.PortName;

        public int Baud => base.BaudRate;

        public string NewLineRead
        {
            get => base.NewLine;
            set => base.NewLine = value;
        }

        public Action ConnectionLost { get; set; }

        public string NewLineWrite { get; set; }

        public SerialPortState State { get; private set; } = SerialPortState.Closed;

        public SystemSerialPort(string name, int baud)
            : base(name, baud)
        {
        }

        public void Flush() => BaseStream.Flush();

        public new void Open()
        {
            try
            {
                Logger.Log($"Connecting to serial port {Name}", LoggingTarget.Network);

                base.Open();
                State = SerialPortState.Open;
            }
            catch (UnauthorizedAccessException)
            {
                Logger.Log($"Access to port {Name} denied", LoggingTarget.Network);
                State = SerialPortState.AccessDenied;
            }
        }

        public new void Close()
        {
            base.Close();

            if (State == SerialPortState.Open)
                State = SerialPortState.Closed;
        }

        public void Release()
        {
            State = SerialPortState.Released;
            Close();
        }

        public new string ReadLine()
        {
            try
            {
                return base.ReadLine();
            }
            catch (Exception e)
                when (e is UnauthorizedAccessException || // Access to the port is denied (i.e. disconnected but not closed yet)
                      e is InvalidOperationException)     // The port is already closed
            {
                handleDisconnected();
                throw;
            }
        }

        public new void WriteLine(string text)
        {
            base.Write(text + (NewLineWrite ?? "\n"));
        }

        private void handleDisconnected()
        {
            Logger.Log($"Lost connection to {Name}", LoggingTarget.Network);
            State = SerialPortState.Disconnected;

            ConnectionLost?.Invoke();
        }
    }
}