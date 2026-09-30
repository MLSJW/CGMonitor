using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;


namespace CGMonitor.Sensors
{
    internal class CPUMonitor
    {
        private ISensor? _temperature;
        private ISensor? _clock;
        private ISensor? _load;

        public void Initialize(IHardware cpuhardware)
        {
            // _temperature = SensorSelector.Find(cpuhardware, SensorType.Temperature, "Package", "Cores", "Total", "Core");
            _temperature = SensorSelector.Find(cpuhardware, SensorType.Temperature, "Core (Tctl/Tdie)", "total");
        }
        public void UpdateData(SensorSnapshot snapshot)
        {
            snapshot.CpuTemp = _temperature?.Value;
        }
    }
}
