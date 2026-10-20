using CGMonitor.Monitors;
using CGMonitor.Overl;
using System;
using System.Runtime.InteropServices;
using Up;
namespace CGMonitor
{
    internal class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProcessDPIAware();
        static async Task Main(string[] args)
        {
            
            if(Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
            HardwareMonitor monitor = new HardwareMonitor();
            var overlay = new OverlayApp(monitor);
            await overlay.Run();
        }
    }
}