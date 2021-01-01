using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Threading;

namespace kyoseki.Game.Serial
{
    public class ConnectionManager : Component
    {
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public List<string> PortNames = new List<string>();

        private List<SerialPort> ports = new List<SerialPort>();

        public Bindable<ConnectionState> State = new Bindable<ConnectionState>(ConnectionState.Resetting);

        public event Action<MessageInfo> MessageReceived;

        public event Action PortsUpdated;

        private Scheduler scheduler;

        public ConnectionManager()
        {
            var thread = new Thread(run)
            {
                Name = "Serial.ConnectionManager",
                IsBackground = true
            };

            thread.Start();
        }

        private void lookForPorts()
        {
            bool newPorts = !SerialPort.GetPortNames().All(p => PortNames.Contains(p));

            if (newPorts)
            {
                Logger.Log("New serial ports detected");
                State.Value = ConnectionState.Resetting;
            }
        }

        private void run()
        {
            scheduler = new Scheduler();
            scheduler.AddDelayed(lookForPorts, 2500, true);

            while (!cancellationToken.IsCancellationRequested)
            {
                switch (State.Value)
                {
                    case ConnectionState.Ready:
                        foreach (var port in ports)
                        {
                            if (!port.IsOpen)
                            {
                                State.Value = ConnectionState.Resetting;
                                continue;
                            }
                            try
                            {
                                bool reachedEnd = false;
                                int iter = 0;

                                while (!reachedEnd)
                                {
                                    var message = port.ReadLine().Replace(port.NewLine, string.Empty);

                                    MessageReceived?.Invoke(new MessageInfo
                                    {
                                        Port = port.PortName,
                                        Content = message
                                    });

                                    iter++;
                                }
                            }
                            catch (TimeoutException _) { }
                            catch (OperationCanceledException _) { }
                        }
                        break;
                    case ConnectionState.Resetting:
                        ports.ForEach(p => p.Dispose());
                        ports.Clear();

                        var newPorts = SerialPort.GetPortNames();

                        lock (PortNames)
                        {
                            PortNames.Clear();
                            PortNames.AddRange(newPorts);

                            PortsUpdated?.Invoke();
                        }

                        foreach (var port in newPorts)
                        {
                            Logger.Log($"Connecting to serial port {port}", LoggingTarget.Network);

                            SerialPort s = new SerialPort(port, 115200)
                            {
                                NewLine = "\r\n",
                                ReadTimeout = 1
                            };

                            try
                            {
                                s.Open();
                            }
                            catch (UnauthorizedAccessException _)
                            {
                                Logger.Log($"Access to port {port} denied", LoggingTarget.Network);
                                continue;
                            }
                            s.BaseStream.Flush();

                            ports.Add(s);
                        }

                        State.Value = ConnectionState.Ready;
                        break;
                }

                scheduler.Update();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            cancellationToken.Cancel();

            ports.ForEach(p => p.Dispose());
        }
    }

    public class MessageInfo
    {
        public string Port { get; set; }

        public string Content { get; set; }
    }

    public enum ConnectionState
    {
        Ready,
        Resetting
    }
}
