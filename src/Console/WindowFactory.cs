using System;
using System.Collections.Generic;
using PInvoke;

namespace Console
{
    /// <summary>
    /// Get a filtered list of OS windows. 
    ///
    /// Performance: 
    /// * ProcessName - Slow
    /// * ClassName - Very fast
    ///
    /// NB! Be careful on what properties that you filter against and in what order. There can be a huge difference in performance.
    /// 
    /// Pending: Convert to a polling service that life time will be as long as there is any applications handled. (pollingIntervalInMs: 20)
    /// </summary>
    internal static class WindowFactory
    {
        public static IEnumerable<Window> GetWindows(Predicate<Window> match)
        {
            var windows = new List<Window>();
            User32.EnumWindows((handle, _) =>
            {
                var window = new Window(handle);
                if (match(window))
                    windows.Add(window);

                return true;
            }, IntPtr.Zero);

            return windows;
        }
    }
}