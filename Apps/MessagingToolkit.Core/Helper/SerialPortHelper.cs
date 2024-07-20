using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Reflection;
using System.Threading;

namespace MessagingToolkit.Core.Helper
{
    /// <summary>
    /// Serial port helper class.
    /// </summary>
    public sealed class SerialPortHelper : IDisposable
    {
        public static void Execute(string portName)
        {
            using (new SerialPortHelper(portName))
            {
            }
        }

        /// <summary>
        /// Safely closes a serial port and its internal stream even if
        /// a USB serial interface was physically removed from the system
        /// in a reliable manner.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="internalSerialStream"></param>
        /// <remarks>
        /// The <see cref="SerialPort"/> class has 3 different problems in disposal
        /// in case of a USB serial device that is physically removed:
        ///
        /// 1. The eventLoopRunner is asked to stop and <see cref="SerialPort.IsOpen"/>
        /// returns false. Upon disposal this property is checked and closing
        /// the internal serial stream is skipped, thus keeping the original
        /// handle open indefinitely (until the finalizer runs which leads to the next problem)
        ///
        /// The solution for this one is to manually close the internal serial stream.
        /// We can get its reference by <see cref="SerialPort.BaseStream" />
        /// before the exception has happened or by reflection and getting the
        /// "internalSerialStream" field.
        ///
        /// 2. Closing the internal serial stream throws an exception and closes
        /// the internal handle without waiting for its eventLoopRunner thread to finish,
        /// causing an uncatchable ObjectDisposedException from it later on when the finalizer
        /// runs (which oddly avoids throwing the exception but still fails to wait for
        /// the eventLoopRunner).
        ///
        /// The solution is to manually ask the event loop runner thread to shutdown
        /// (via reflection) and waiting for it before closing the internal serial stream.
        ///
        /// 3. Since Dispose throws exceptions, the finalizer is not suppressed.
        ///
        /// The solution is to suppress their finalizers at the beginning.
        /// </remarks>
        public static void SafeDisconnect(SerialPort port, Stream internalSerialStream)
        {
            try
            {
                if (port != null)
                {
                    GC.SuppressFinalize(port);
                }

                if (internalSerialStream != null)
                {
                    GC.SuppressFinalize(internalSerialStream);
                    ShutdownEventLoopHandler(internalSerialStream);
                }
            }
            catch (Exception ex)
            {
            }

            try
            {

                internalSerialStream.Close();
            }
            catch (Exception ex)
            {
            }

            try
            {
                port.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private static void ShutdownEventLoopHandler(Stream internalSerialStream)
        {
            try
            {

                FieldInfo eventRunnerField = internalSerialStream.GetType()
                    .GetField("eventRunner", BindingFlags.NonPublic | BindingFlags.Instance);

                if (eventRunnerField == null)
                {
                }
                else
                {
                    object eventRunner = eventRunnerField.GetValue(internalSerialStream);
                    Type eventRunnerType = eventRunner.GetType();

                    FieldInfo endEventLoopFieldInfo = eventRunnerType.GetField(
                        "endEventLoop", BindingFlags.Instance | BindingFlags.NonPublic);

                    FieldInfo eventLoopEndedSignalFieldInfo = eventRunnerType.GetField(
                        "eventLoopEndedSignal", BindingFlags.Instance | BindingFlags.NonPublic);

                    FieldInfo waitCommEventWaitHandleFieldInfo = eventRunnerType.GetField(
                        "waitCommEventWaitHandle", BindingFlags.Instance | BindingFlags.NonPublic);

                    if (endEventLoopFieldInfo == null
                        || eventLoopEndedSignalFieldInfo == null
                        || waitCommEventWaitHandleFieldInfo == null)
                    {
                    }
                    else
                    {
                        var eventLoopEndedWaitHandle =
                            (WaitHandle)eventLoopEndedSignalFieldInfo.GetValue(eventRunner);
                        var waitCommEventWaitHandle =
                            (ManualResetEvent)waitCommEventWaitHandleFieldInfo.GetValue(eventRunner);

                        endEventLoopFieldInfo.SetValue(eventRunner, true);

                        // Sometimes the event loop handler resets the wait handle
                        // before exiting the loop and hangs (in case of USB disconnect)
                        // In case it takes too long, brute-force it out of its wait by
                        // setting the handle again.
                        do
                        {
                            waitCommEventWaitHandle.Set();
                        } while (!eventLoopEndedWaitHandle.WaitOne(2000));

                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (m_Handle != null)
            {
                m_Handle.Close();
                m_Handle = null;
            }
        }

        #endregion

        #region Implementation

        private const int DcbFlagAbortOnError = 14;
        private const int CommStateRetries = 10;
        private SafeFileHandle m_Handle;

        private SerialPortHelper(string portName)
        {
            const int dwFlagsAndAttributes = 0x40000000;
            const int dwAccess = unchecked((int)0xC0000000);

            if ((portName == null) || !portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Serial Port", "portName");
            }
            SafeFileHandle hFile = CreateFile(@"\\.\" + portName, dwAccess, 0, IntPtr.Zero, 3, dwFlagsAndAttributes,
                                              IntPtr.Zero);
            if (hFile.IsInvalid)
            {
                WinIoError();
            }
            try
            {
                int fileType = GetFileType(hFile);
                if ((fileType != 2) && (fileType != 0))
                {
                    throw new ArgumentException("Invalid Serial Port", "portName");
                }
                m_Handle = hFile;
                InitializeDcb();
            }
            catch
            {
                hFile.Close();
                m_Handle = null;
                throw;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId,
                                                StringBuilder lpBuffer, int nSize, IntPtr arguments);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ClearCommError(SafeFileHandle hFile, ref int lpErrors, ref Comstat lpStat);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
                                                        IntPtr securityAttrs, int dwCreationDisposition,
                                                        int dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetFileType(SafeFileHandle hFile);

        private void InitializeDcb()
        {
            Dcb dcb = new Dcb();
            GetCommStateNative(ref dcb);
            dcb.Flags &= ~(1u << DcbFlagAbortOnError);
            SetCommStateNative(ref dcb);
        }

        private static string GetMessage(int errorCode)
        {
            StringBuilder lpBuffer = new StringBuilder(0x200);
            if (
                FormatMessage(0x3200, new HandleRef(null, IntPtr.Zero), errorCode, 0, lpBuffer, lpBuffer.Capacity,
                              IntPtr.Zero) != 0)
            {
                return lpBuffer.ToString();
            }
            return "Unknown Error";
        }

        private static int MakeHrFromErrorCode(int errorCode)
        {
            return (int)(0x80070000 | (uint)errorCode);
        }

        private static void WinIoError()
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new IOException(GetMessage(errorCode), MakeHrFromErrorCode(errorCode));
        }

        private void GetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat();

            for (int i = 0; i < CommStateRetries; i++)
            {
                if (!ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (GetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }

        private void SetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat();

            for (int i = 0; i < CommStateRetries; i++)
            {
                if (!ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (SetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }

        #region Nested type: COMSTAT

        [StructLayout(LayoutKind.Sequential)]
        private struct Comstat
        {
            public readonly uint Flags;
            public readonly uint cbInQue;
            public readonly uint cbOutQue;
        }

        #endregion

        #region Nested type: DCB

        [StructLayout(LayoutKind.Sequential)]
        private struct Dcb
        {
            public readonly uint DCBlength;
            public readonly uint BaudRate;
            public uint Flags;
            public readonly ushort wReserved;
            public readonly ushort XonLim;
            public readonly ushort XoffLim;
            public readonly byte ByteSize;
            public readonly byte Parity;
            public readonly byte StopBits;
            public readonly byte XonChar;
            public readonly byte XoffChar;
            public readonly byte ErrorChar;
            public readonly byte EofChar;
            public readonly byte EvtChar;
            public readonly ushort wReserved1;
        }

        #endregion

        #endregion
    }

    /*
    internal class Program
    {
        private static void Main(string[] args)
        {
            SerialPortHelper.Execute("COM1");
            using (SerialPort port = new SerialPort("COM1"))
            {
                port.Write("test");
            }
        }
    }
    */
}
