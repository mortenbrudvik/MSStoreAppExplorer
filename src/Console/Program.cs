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
                window.ClassName == "ApplicationFrameWindow");

            foreach (var window in windows)
            {
                var filePath = window.FilePath;
                var dirPath = Path.GetDirectoryName(filePath);
                var xmlPath = Path.Join(dirPath, "AppxManifest.xml");
                if(!File.Exists(xmlPath))
                    continue;
                
                //var xml = File.ReadAllText(xmlPath);
                var doc = XElement.Load(xmlPath);

                var logo = doc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Logo")?.Value;
                
                var protocols = doc.Descendants().Where(x => x.Name.LocalName == "Protocol");
                var aliases = protocols.Select(x => x.Attribute("Name").Value);
                
                
                    
                

                
                
                
                
                System.Console.Out.WriteLine($"{window.Title}  | " +
                                             $"{window.ClassName} | " +
                                             $"{window.ProcessName} |  " +
                                             $"{window.ProcessId} |  " +
                                             $"{window.FilePath}");
            }

            System.Console.Out.WriteLine("");
        }
        
        private static bool IsHiddenWindowStoreApp(Window window)
            => window.ClassName is "Windows.UI.Core.CoreWindow" && 
               window.IsCloaked;
    }
}
