using System;
using osu.Framework.Bindables;

namespace kyoseki.Game.Serial
{
    public interface ISerialPort : IDisposable
    {
        string Name { get; }

        int Baud { get; }

        Bindable<SerialPortState> State { get; }

        string NewLineRead { get; }

        string NewLineWrite { get; }

        string ReadLine();

        void WriteLine(string text);

        void Flush();

        void Open();

        void Close();

        void Release();

        Action ConnectionLost { get; set; }
    }
}