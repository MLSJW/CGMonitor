using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;


namespace CGMonitor.Sensors
{
    internal class CPUMonitor
    {
        private ISensor? _temperature;
        private ISensor? _power;
        private ISensor? _load;
        private IHardware hardware;
        private float _currentFreq;

        public void Initialize(IHardware cpuhardware)
        {
            hardware = cpuhardware;
            _temperature = SensorSelector.Find(cpuhardware, SensorType.Temperature, "Core (Tctl/Tdie)");
            _load = SensorSelector.Find(cpuhardware, SensorType.Load, "CPU Total");
            _power = SensorSelector.Find(cpuhardware, SensorType.Power, "Package");
            
        }
        public void UpdateData(SensorSnapshot snapshot)
        {
            snapshot.CpuTemp = _temperature?.Value;
            snapshot.CpuLoad = _load?.Value;
            snapshot.CpuPower = _power?.Value;

            float maxFreq = 0;
            foreach(var sensor in hardware.Sensors)
            {
                if(sensor.SensorType==SensorType.Clock 
                    && sensor.Name.Contains("Core") 
                    && !sensor.Name.Contains("SMU")
                    && !sensor.Name.Contains("Effective")  
                    && !sensor.Name.Contains("Average")
                    && !sensor.Name.Contains("Max"))
                {
                    float coreFreq = sensor.Value ?? 0;
                    if (coreFreq > maxFreq)
                    {
                        maxFreq = coreFreq;
                    }
                }
            }
            _currentFreq = maxFreq;
            snapshot.CpuClocks = _currentFreq;

            float totalCoresPower = 0;
            foreach(var sensor in hardware.Sensors)
            {
                if (sensor.Name.Contains("Core #") && sensor.SensorType == SensorType.Power)
                {
                    totalCoresPower += sensor?.Value ?? 0;
                }
            }
            float calculatedGpuPower = (_power?.Value ?? 0) - totalCoresPower;
            if (calculatedGpuPower < 0) { calculatedGpuPower = 0; }
            snapshot.GpuPowerMobile = calculatedGpuPower;
        }
    }
}
