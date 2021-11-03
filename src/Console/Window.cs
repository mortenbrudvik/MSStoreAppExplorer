using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Console.NativeMethods;
using LanguageExt;
using PInvoke;
using static PInvoke.User32;

using static LanguageExt.Prelude;

namespace Console
{
    public class Window
    {
        private readonly Lazy<string> _title;
        private readonly Lazy<string> _className;
        private readonly Lazy<string> _filePath;
        private readonly Lazy<int> _processId;
        private readonly Lazy<string> _processName;
        private readonly Lazy<string> _packageXmlPath;
        private readonly Lazy<Option<XElement>> _packageXml;

        public Window(IntPtr windowHandle)
        {
            Handle = windowHandle;
            _title = new Lazy<string>(GetWindowText(Handle));
            _filePath = new Lazy<string>(ExtractFilePath);
            _className = new Lazy<string>(GetClassName(Handle));
            _processName = new Lazy<string>(GetProcessName);
            _processId = new Lazy<int>(GetProcessId());
            _packageXmlPath = new Lazy<string>(GetPackageXmlPath);
            _packageXml = new Lazy<Option<XElement>>(GetPackageXml);
        }

        private Option<XElement> GetPackageXml()
        {
            if (PackageXmlPath.IsNullOrWhiteSpace())
                return None;
            
            return XElement.Load(PackageXmlPath);
        }

        public string PackageXmlPath => _packageXmlPath.Value;

        private string GetPackageXmlPath()
        {
            var dirPath = Path.GetDirectoryName(PackageFilePath);
            var xmlPath = Path.Join(dirPath, "AppxManifest.xml");
            
            if( File.Exists(xmlPath))
                return xmlPath;

            return "";
        }

        public string IconFilePath => _packageXml.Value.Match(
            xmlDoc => xmlDoc.Descendants().SingleOrDefault(x => x.Name.LocalName == "Logo")?.Value,
            () => "");


        public IEnumerable<string> Commands => _packageXml.Value.Match(
            xmlDoc => xmlDoc.Descendants()
                .Where(x => x.Name.LocalName == "Protocol")
                .Select(x => x.Attribute("Name").Value),
            () => new List<string>());
            
        public IntPtr Handle { get; }

        public string Title => _title.Value;
        public string ClassName => _className.Value;
        public string ProcessName => _processName.Value;
        public int ProcessId => _processId.Value;
        public string PackageFilePath => _filePath.Value;

        public bool IsVisible => IsWindowVisible(Handle);
        public bool IsWindow => User32.IsWindow(Handle);

        private string GetProcessName()
        {
            try
            {
                var processHandle = Kernel32Ext.OpenProcess(ProcessAccessFlags.AllAccess, false, ProcessId);
                var processName = new StringBuilder(1000);

                if(PsapiExt.GetProcessImageFileName(processHandle, processName, 1000) != 0)
                    return processName.ToString().Split('\\').Reverse().ToArray()[0];

                return "";

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }

        private int GetProcessId()
        {
            try
            {
                GetWindowThreadProcessId(Handle, out var processId);
            
                return processId;

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }

        private string ExtractFilePath()
        {
            return UwpUtils.GetFilePath(Handle);
        }
    }
}