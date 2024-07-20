using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace MessagingToolkit.Barcode.Helper
{
    internal static class NativeMethods
    {
        internal static unsafe void CopyUnmanagedMemory(byte* srcPtr, int srcOffset, byte* dstPtr, int dstOffset, int count)
        {
            srcPtr += srcOffset;
            dstPtr += dstOffset;

            memcpy(dstPtr, srcPtr, count);
        }

        internal static void SetUnmanagedMemory(IntPtr dst, int filler, int count)
        {
            memset(dst, filler, count);
        }

        // Win32 memory copy function
        //[DllImport("ntdll.dll")]
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern unsafe byte* memcpy(
            byte* dst,
            byte* src,
            int count);

        // Win32 memory set function
        //[DllImport("ntdll.dll")]
        //[DllImport("coredll.dll", EntryPoint = "memset", SetLastError = false)]
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void memset(
            IntPtr dst,
            int filler,
            int count);


        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        /*
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        */
    }
}
