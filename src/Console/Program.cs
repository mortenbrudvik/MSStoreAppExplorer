namespace Console
{
    internal static class Program
    {
        private static void Main()
        {
            var windows = WindowFactory.GetWindows(window => window.IsVisible && window.IsWindow);

            foreach (var window in windows)
            {
                System.Console.Out.WriteLine($"{window.Title}  | {window.ClassName} | {window.ProcessName} |  {window.FilePath}");
            }

            System.Console.Out.WriteLine("");
        }
        
        private static bool IsHiddenWindowStoreApp(Window window)
            => window.ClassName is "Windows.UI.Core.CoreWindow" && 
               window.IsCloaked;
    }
}
