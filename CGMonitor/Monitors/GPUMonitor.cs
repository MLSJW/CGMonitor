using BlackSharp.Core.Extensions;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Gpu;
using LibreHardwareMonitor.PawnIo;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGMonitor.Sensors
{
    internal class GPUMonitor
    {
        private ISensor? _temperature;
        private ISensor? _clock;
        private ISensor? _load;
        private ISensor? _power;
        private ISensor? _fps;
        private float? _smoothedLoad = 0;
        private const float SmoothingFactor = 0.15f;

        public void Initialize(IHardware hardware)
        {
            //_temperature = SensorSelector.Find(hardware, SensorType.Temperature, "GPU Core", "GPU Memory", "GPU VR VDDC", "GPU VR MVDD", "GPU VR SoC", "GPU Liquid", "GPU PLX", "GPU Hot Spot");
            // _power = SensorSelector.Find(hardware, SensorType.Power, "GPU Core", "GPU Package");
            _fps = SensorSelector.Find(hardware, SensorType.Factor, "Fullscreen FPS");
            _load = SensorSelector.Find(hardware, SensorType.Load, "Gpu Load", "D3D 3D");
        }
        public void UpdateData(SensorSnapshot snapshot)
        {

            float? currentLoad = _load?.Value;
            _smoothedLoad = (_smoothedLoad * (1f - SmoothingFactor)) + (currentLoad * SmoothingFactor);
            snapshot.GpuFPS = _fps?.Value;
            snapshot.GpuLoad = _smoothedLoad;
            //snapshot.GpuTemp = _temperature?.Value;
           // snapshot.GpuPower = _power?.Value;
        }
    }
}
