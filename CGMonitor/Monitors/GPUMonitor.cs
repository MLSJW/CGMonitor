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

        public void Initialize(IHardware hardware)
        {
            _temperature = SensorSelector.Find(hardware, SensorType.Temperature, "GPU Core", "GPU Memory", "GPU VR VDDC", "GPU VR MVDD", "GPU VR SoC", "GPU Liquid", "GPU PLX", "GPU Hot Spot");
           
             
        }
        public void UpdateData(SensorSnapshot snapshot)
        {
            snapshot.GpuTemp = _temperature?.Value;
        }
    }
}
