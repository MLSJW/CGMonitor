using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGMonitor.Sensors
{
    internal class RAMMonitor
    {
        private ISensor? _temperature;
        private ISensor? _clock;
        private ISensor? _load;
        private ISensor? _used;
        private ISensor? _available;

        public void Initialize(IHardware hardware)
        {
            _clock = SensorSelector.Find(hardware, SensorType.Clock);
            _load = SensorSelector.Find(hardware, SensorType.Load, "Memory");
            _available = SensorSelector.Find(hardware, SensorType.Data, "Memory Available");
            _used = SensorSelector.Find(hardware, SensorType.Data, "Memory Used");
        }
        
        public void UpdateData(SensorSnapshot snapshot)
        {
            snapshot.RAMClocks = _clock?.Value;
            snapshot.RAMLoad = _load?.Value;
            snapshot.RAMTotal = _used?.Value + _available?.Value;
            snapshot.RAMUsed = _used?.Value;
        }
    }
}
