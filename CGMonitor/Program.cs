using CGMonitor.Monitors;
using CGMonitor.Overl;
using System;
using Up;
namespace CGMonitor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HardwareMonitor monitor = new HardwareMonitor();
            var overlay = new OverlayApp(monitor);
            await overlay.Run();
        }
    }
}