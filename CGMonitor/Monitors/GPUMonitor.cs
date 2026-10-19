using BlackSharp.Core.Extensions;
using CGMonitor.Monitors;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Gpu;
using LibreHardwareMonitor.PawnIo;
using PresentMonFps;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGMonitor.Sensors
{
    internal class GPUMonitor
    {
        private ISensor? _temperature;
        private ISensor? _load;
        private ISensor? _power;
        private ISensor? _fps;
        private float? _smoothedLoad = 0;
        private const float SmoothingFactor = 0.15f;
        private float _currentFps = 0f;
        private string? _currentTrackedProcess;
        private CancellationTokenSource? _fpsCts;
        private Task? _fpsTask;

        public void Initialize(IHardware hardware)
        {
            _temperature = SensorSelector.Find(hardware, SensorType.Temperature, "GPU Core", "GPU Package", "Core (Tctl/Tdie)", "Core", "GPU Hot Spot");
            _load = SensorSelector.Find(hardware, SensorType.Load, "Gpu Load", "D3D 3D");
            _power = SensorSelector.Find(hardware, SensorType.Power, "GPU Package", "GPU Core", "GPU Total", "GPU ASIC", "Total Power", "Power");
        }

        public void UpdateData(SensorSnapshot snapshot)
        {
            float? currentLoad = _load?.Value;
            _smoothedLoad = (_smoothedLoad * (1f - SmoothingFactor)) + (currentLoad * SmoothingFactor);
            UpdateFpsTracking();
            snapshot.GpuFPS = _currentFps;
            snapshot.GpuLoad = _smoothedLoad;
            snapshot.GpuTemp = _temperature?.Value;
            snapshot.GpuPower = _power?.Value;
        }

        public void UpdateFpsTracking()
        {
            string? activeProcess = FPSHelper.GetActiveProcessName();
            if (string.IsNullOrEmpty(activeProcess)) return;
            if (activeProcess == _currentTrackedProcess) return; 

            _fpsCts?.Cancel();
            _fpsTask?.Wait(300);

            _currentTrackedProcess = activeProcess;
            _fpsCts = new CancellationTokenSource();
            _fpsTask = Task.Run(async () =>
            {
                try
                {
                    uint pid = await FpsInspector.GetProcessIdByNameAsync(activeProcess);
                    await FpsInspector.StartForeverAsync(
                        new FpsRequest(pid),
                        result => _currentFps = (float)result.Fps,
                        _fpsCts.Token
                    );
                }
                catch
                {
                    _currentFps = 0;
                }
            });
        }
        
    }
}
