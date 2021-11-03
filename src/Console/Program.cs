using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Console
{
    internal static class Program
    {
        private static void Main()
        {
            var watch = new Stopwatch();
            watch.Start();
            
            var windows = WindowFactory.GetWindows(window =>
                window.IsVisible && 
                window.IsWindow && 
                window.ClassName == "ApplicationFrameWindow" &&
                window.ProcessName.Contains("ApplicationFrameHost"));

            watch.Stop();

            WriteToConsole(windows.ToList());
            
            System.Console.Out.WriteLine($"Windows found: {windows.ToList().Count}. Search time: {watch.ElapsedMilliseconds / 1000}.{watch.ElapsedMilliseconds % 1000} seconds.");
        }
        
        private static void WriteToConsole(IEnumerable<Window> windows)
        {
            var options = new LoggerTableOptions
                {Columns = new List<string> {"Handle", "Class Name", "Title", "Process Name", "ProcessId", "PackageFilePath", "IconPath", "Commands"}};
            var tableLogger = new TableLogger(options);
            windows.ToList().ForEach(win =>
                tableLogger.AddRow(win.Handle.ToString(), win.ClassName.Truncate(70, ""), win.Title.Truncate(70, ""), 
                    win.ProcessName.Truncate(30, ""), win.ProcessId, win.PackageFilePath.Truncate(120),
                    win.IconFilePath, string.Join(',', win.Commands)));
                
            tableLogger.Write(Format.Minimal);
        }
    }
}
