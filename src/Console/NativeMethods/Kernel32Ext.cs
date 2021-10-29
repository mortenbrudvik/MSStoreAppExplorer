using System;
using System.Runtime.InteropServices;

namespace Console.NativeMethods
{
    public class Kernel32Ext
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);
    }
}