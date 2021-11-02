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
                window.ProcessName.Contains("ApplicationFrameHost") 
                );

            foreach (var window in windows)
            {
                var filePath = window.FilePath;
                var dirPath = Path.GetDirectoryName(filePath);
                var xmlPath = Path.Join(dirPath, "AppxManifest.xml");
                if( !File.Exists(xmlPath))
                    continue;
                
                var doc = XElement.Load(xmlPath);

                var logo = doc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Logo")?.Value;
                var commands = doc.Descendants()
                    .Where(x => x.Name.LocalName == "Protocol")
                    .Select(x => x.Attribute("Name").Value);

                System.Console.Out.WriteLine($"{window.Title}  | " +
                                             $"{logo} |  " +
                                             $"{string.Join(',', commands)} |  " +
                                             $"{window.FilePath}");
            }
        }
    }
}
