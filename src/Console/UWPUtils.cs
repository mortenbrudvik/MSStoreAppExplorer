using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Epsis.EnifyEngine.Infrastructure.Utils
{
    internal struct WINDOWINFO
    {
        public uint ownerpid;
        public uint childpid;
    }

    /// <summary>
    /// https://stackoverflow.com/questions/32001621/how-to-get-the-application-name-from-hwnd-for-windows-10-store-apps-e-g-edge
    ///
    /// TODO: Refactor
    /// </summary>
    public static class UwpUtils
    {
        public static int GetChildProcessId(IntPtr windowHandle, uint processId)
        {
            var windowInfo = new WINDOWINFO();
            windowInfo.ownerpid = processId;
            windowInfo.childpid = windowInfo.ownerpid;

            var pWindowinfo = Marshal.AllocHGlobal(Marshal.SizeOf(windowInfo));

            Marshal.StructureToPtr(windowInfo, pWindowinfo, false);

            var lpEnumFunc = new EnumWindowProc(EnumChildWindowsCallback);
            EnumChildWindows(windowHandle, lpEnumFunc, pWindowinfo);

            windowInfo = (WINDOWINFO)Marshal.PtrToStructure(pWindowinfo, typeof(WINDOWINFO));

            Marshal.FreeHGlobal(pWindowinfo);

            return (int) windowInfo.childpid;
        }
        
        public static string GetFilePath(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                return null;

            GetWindowThreadProcessId(windowHandle, out var processId);

            IntPtr proc;
            if ((proc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, (int)processId)) == IntPtr.Zero)
                return null;

            var capacity = 2000;
            var sb = new StringBuilder(capacity);
            QueryFullProcessImageName(proc, 0, sb, ref capacity);

            var processName = sb.ToString(0, capacity);

            // UWP apps are wrapped in another app called, if this has focus then try and find the child UWP process
            if (Path.GetFileName(processName).Equals("ApplicationFrameHost.exe"))
                processName = UWP_AppName(windowHandle, processId);

            return processName;
        }
        
        public static string GetFilePath2(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                return null;

            GetWindowThreadProcessId(windowHandle, out var processId);
            return UWP_AppName(windowHandle, processId);
            
            IntPtr proc;
            if ((proc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, (int)processId)) == IntPtr.Zero)
                return null;

            var capacity = 2000;
            var sb = new StringBuilder(capacity);
            QueryFullProcessImageName(proc, 0, sb, ref capacity);

            var processName = sb.ToString(0, capacity);

            // UWP apps are wrapped in another app called, if this has focus then try and find the child UWP process
            if (Path.GetFileName(processName).Equals("ApplicationFrameHost.exe"))
                processName = UWP_AppName(windowHandle, processId);

            return processName;
        }

        
        #region User32
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        
        /// <summary>
        /// Delegate for the EnumChildWindows method
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
        /// <returns>True to continue enumerating, false to bail.</returns>
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowProc lpEnumFunc, IntPtr lParam);
        #endregion

        #region Kernel32
        public const UInt32 PROCESS_QUERY_INFORMATION = 0x400;
        public const UInt32 PROCESS_VM_READ = 0x010;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryFullProcessImageName([In]IntPtr hProcess, [In]int dwFlags, [Out]StringBuilder lpExeName, ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            UInt32 dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)]
            Boolean bInheritHandle,
            Int32 dwProcessId
        );
        #endregion

   

        #region Get UWP Application Name

        /// <summary>
        /// Find child process for uwp apps, edge, mail, etc.
        /// </summary>
        /// <param name="windowHandle">hWnd</param>
        /// <param name="processId">pID</param>
        /// <returns>The application name of the UWP.</returns>
        private static string UWP_AppName(IntPtr windowHandle, uint processId)
        {
            var windowInfo = new WINDOWINFO();
            windowInfo.ownerpid = processId;
            windowInfo.childpid = windowInfo.ownerpid;

            var pWindowÏnfo = Marshal.AllocHGlobal(Marshal.SizeOf(windowInfo));

            Marshal.StructureToPtr(windowInfo, pWindowÏnfo, false);

            var lpEnumFunc = new EnumWindowProc(EnumChildWindowsCallback);
            EnumChildWindows(windowHandle, lpEnumFunc, pWindowÏnfo);

            windowInfo = (WINDOWINFO)Marshal.PtrToStructure(pWindowÏnfo, typeof(WINDOWINFO));

            IntPtr proc;
            if ((proc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, (int)windowInfo.childpid)) == IntPtr.Zero)
                return null;

            var capacity = 2000;
            var sb = new StringBuilder(capacity);
            QueryFullProcessImageName(proc, 0, sb, ref capacity);

            Marshal.FreeHGlobal(pWindowÏnfo);

            return sb.ToString(0, capacity);
        }

        /// <summary>
        /// Callback for enumerating the child windows.
        /// </summary>
        /// <param name="hWnd">hWnd</param>
        /// <param name="lParam">lParam</param>
        /// <returns>always <c>true</c>.</returns>
        private static bool EnumChildWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            WINDOWINFO info = (WINDOWINFO)Marshal.PtrToStructure(lParam, typeof(WINDOWINFO));

            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            if (processId != info.ownerpid)
                info.childpid = processId;

            Marshal.StructureToPtr(info, lParam, true);

            return true;
        }
        #endregion
    }
}