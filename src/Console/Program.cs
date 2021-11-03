using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Console
{
    internal static class Program
    {
        private static void Main()
        {
            var windows = WindowFactory.GetWindows(window =>
                window.IsVisible && 
                window.IsWindow && 
                window.ClassName == "ApplicationFrameWindow" &&
                window.ProcessName.Contains("ApplicationFrameHost"));

            WriteToConsole(windows.ToList());
        }
        
        private static void WriteToConsole(IReadOnlyCollection<Window> windows)
        {
            var watch = new Stopwatch();
            watch.Start();

            var options = new LoggerTableOptions
                {Columns = new List<string> {"Handle", "Class Name", "Title", "Process Name", "ProcessId", "PackageFilePath", "IconPath", "Commands"}};
            var tableLogger = new TableLogger(options);
            windows.ToList().ForEach(win =>
                tableLogger.AddRow(win.Handle.ToString(), win.ClassName.Truncate(70, ""), win.Title.Truncate(70, ""), 
                    win.ProcessName.Truncate(30, ""), win.ProcessId, win.PackageFilePath.Truncate(120),
                    win.IconFilePath, string.Join(',', win.Commands)));
                
            tableLogger.Write(Format.Minimal);
            watch.Stop();
            System.Console.Out.WriteLine($"Windows found: {windows.Count}. Search time: {watch.ElapsedMilliseconds / 1000}.{watch.ElapsedMilliseconds % 1000} seconds.");
        }
    }
}
