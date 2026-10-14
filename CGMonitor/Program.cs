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
            
            while (true) {monitor.Update();
                Thread.Sleep(1000);
                var snapshot = monitor.Snapshot;
                
                    
                    Console.WriteLine($"Температура CPU: {snapshot.CpuTemp:F0}°C");
                    Console.WriteLine($"Частота CPU: {snapshot.CpuClocks:F0}ГГц");
                    Console.WriteLine($"Загрузка CPU: {snapshot.CpuLoad:F0}%");
                    Console.WriteLine($"Потребление CPU: {snapshot.CpuPower:F0}W");

                    Console.WriteLine($"Температура GPU: {snapshot.GpuTemp}°C");
                    Console.WriteLine($"Потребление GPU: {(snapshot.GpuPower == null ? snapshot.GpuPowerMobile : null):F0}W");
                    Console.WriteLine($"FPS: {snapshot.GpuFPS}FPS");
                    Console.WriteLine($"Загрузка GPU: {snapshot.GpuLoad}%");


                    Console.WriteLine($"Загрузка RAM: {snapshot.RAMLoad}%");
                    Console.WriteLine($"Занято RAM: {snapshot.RAMUsed:F1}/{snapshot.RAMTotal:F1}Гб");

                    Console.WriteLine("");
                    //Console.WriteLine($"Частота RAM: {snapshot.RAMClocks}МГц");
            }
        }
    }
}