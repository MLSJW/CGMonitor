using System;
using CGMonitor.Overl;
using Up;
namespace CGMonitor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var overlay = new OverlayApp();
            await overlay.Run();
            HardwareMonitor mon = new HardwareMonitor();
            while (true) {mon.Update();
 }
            
        }
    }
}