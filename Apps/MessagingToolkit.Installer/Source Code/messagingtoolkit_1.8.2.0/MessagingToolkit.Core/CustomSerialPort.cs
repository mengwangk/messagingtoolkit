//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.IO.Ports;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace MessagingToolkit.Core
{
    public sealed class CustomSerialPort : SerialPort
    {
        public CustomSerialPort()
            : base()
        {
        }

        public CustomSerialPort(IContainer container)
            : base(container)
        {
        }

        public CustomSerialPort(string portName)
            : base(portName)
        {
        }

        public CustomSerialPort(string portName, int baudRate)
            : base(portName, baudRate)
        {
        }

        public CustomSerialPort(string portName, int baudRate, Parity parity)
            : base(portName, baudRate, parity)
        {
        }

        public CustomSerialPort(string portName, int baudRate, Parity parity, int dataBits)
            : base(portName, baudRate, parity, dataBits)
        {
        }

        public CustomSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
        }

        // private member access via reflection
        int fd;
        FieldInfo disposedFieldInfo;
        object data_received;

        public new void Open()
        {
            base.Open();

            if (!IsWindows)
            {
                FieldInfo fieldInfo = BaseStream.GetType().GetField("fd", BindingFlags.Instance | BindingFlags.NonPublic);
                fd = (int)fieldInfo.GetValue(BaseStream);
                disposedFieldInfo = BaseStream.GetType().GetField("disposed", BindingFlags.Instance | BindingFlags.NonPublic);
                fieldInfo = typeof(SerialPort).GetField("data_received", BindingFlags.Instance | BindingFlags.NonPublic);
                data_received = fieldInfo.GetValue(this);

                new System.Threading.Thread(new System.Threading.ThreadStart(this.EventThreadFunction)).Start();
            }
        }

        static bool IsWindows
        {
            get
            {
                PlatformID id = Environment.OSVersion.Platform;
                return id == PlatformID.Win32Windows || id == PlatformID.Win32NT; // WinCE not supported
            }
        }

        static bool IsMono
        {
            get
            {
                Type t = Type.GetType("Mono.Runtime");
                if (t != null)
                    return true;
                else
                    return false;
            }

        }

        private void EventThreadFunction()
        {
            do
            {
                try
                {
                    var _stream = BaseStream;
                    if (_stream == null)
                    {
                        return;
                    }
                    if (Poll(_stream, ReadTimeout))
                    {
                        var constructor = typeof(SerialDataReceivedEventArgs).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(SerialData) }, null);
                        var args = (SerialDataReceivedEventArgs)constructor.Invoke(new object[] { SerialData.Chars });
                        OnDataReceived(args);
                    }
                }
                catch
                {
                    return;
                }
            }
            while (IsOpen);
        }

        void OnDataReceived(SerialDataReceivedEventArgs args)
        {
            SerialDataReceivedEventHandler handler = (SerialDataReceivedEventHandler)Events[data_received];

            if (handler != null)
            {
                handler(this, args);
            }
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern bool poll_serial(int fd, out int error, int timeout);

        private bool Poll(Stream stream, int timeout)
        {
            CheckDisposed(stream);
            if (IsOpen == false)
            {
                throw new Exception("port is closed");
            }
            int error;

            bool poll_result = poll_serial(fd, out error, ReadTimeout);
            if (error == -1)
            {
                ThrowIOException();
            }
            return poll_result;
        }

        [DllImport("libc")]
        static extern IntPtr strerror(int errnum);

        static void ThrowIOException()
        {
            int errnum = Marshal.GetLastWin32Error();
            string error_message = Marshal.PtrToStringAnsi(strerror(errnum));

            throw new IOException(error_message);
        }

        void CheckDisposed(Stream stream)
        {
            bool disposed = (bool)disposedFieldInfo.GetValue(stream);
            if (disposed)
            {
                throw new ObjectDisposedException(stream.GetType().FullName);
            }
        }
    }
}

 
