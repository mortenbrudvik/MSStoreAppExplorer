﻿using System;
using System.Linq;
using System.Text;
using Console.NativeMethods;
using PInvoke;
using static PInvoke.User32;

namespace Console
{
    public class Window
    {
        private readonly Lazy<string> _title;
        private readonly Lazy<string> _className;
        private readonly Lazy<string> _filePath;
        private readonly Lazy<int> _processId;
        private readonly Lazy<string> _processName;

        public Window(IntPtr windowHandle)
        {
            Handle = windowHandle;
            _title = new Lazy<string>(GetWindowText(Handle));
            _filePath = new Lazy<string>(ExtractFilePath);
            _className = new Lazy<string>(GetClassName(Handle));
            _processName = new Lazy<string>(GetProcessName);
            _processId = new Lazy<int>(GetProcessId());
        }

        private IntPtr Handle { get; }

        public string Title => _title.Value;
        public string ClassName => _className.Value;
        public string ProcessName => _processName.Value;
        public int ProcessId => _processId.Value;
        public string FilePath => _filePath.Value;

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