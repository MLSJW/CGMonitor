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
                Thread.Sleep(1000);
                var snapshot = mon.Snapshot;

                if (snapshot.CpuTemp.HasValue)
                {
                    Console.WriteLine($"Температура CPU1: {snapshot.CpuTemp.Value:F0}°C");
                    Console.WriteLine($"Частота CPU1: {snapshot.CpuClocks.Value:F0}ГГц");
                    Console.WriteLine($"Загрузка CPU1: {snapshot.CpuLoad.Value:F0}%");
                }
                else
                {
                    Console.WriteLine("Температура CPU: Поиск датчика...");
                }

            }

        }
    }
}