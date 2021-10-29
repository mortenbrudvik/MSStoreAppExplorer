using System;
using System.Runtime.InteropServices;

namespace Console.NativeMethods
{
    public class DwmApiExt
    {
        public const int DWMWA_CLOAKED = 14;

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, int dwAttribute, out int pvAttribute, int cbAttribute);
    }
}
