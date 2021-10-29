using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Console.NativeMethods
{
    public class PsapiExt
    {
        /// <summary>
        /// Retrieves the name of the executable file for the specified process.
        ///
        /// https://docs.microsoft.com/en-us/windows/win32/api/psapi/nf-psapi-getprocessimagefilenamea
        /// </summary>
        /// <param name="hProcess">A handle to the process. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right. For more information, see Process Security and Access Rights.</param>
        /// <param name="lpImageFileName">A pointer to a buffer that receives the full path to the executable file.</param>
        /// <param name="nSize">The size of the lpImageFileName buffer, in characters.</param>
        /// <returns></returns>
        [DllImport("psapi.dll", BestFitMapping = false)]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, [Out] StringBuilder lpImageFileName, [In][MarshalAs(UnmanagedType.U4)] int nSize);
    }
}