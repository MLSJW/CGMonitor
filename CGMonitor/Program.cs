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
                var snapshot = mon.Snapshot;

                if (snapshot.CpuTemp.HasValue)
                {
                    Console.WriteLine($"Температура CPU: {snapshot.CpuTemp.Value:F0}°C");
                }
                else
                {
                    Console.WriteLine("Температура CPU: Поиск датчика...");
                }

            }

        }
    }
}