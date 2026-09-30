using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;


namespace CGMonitor.Sensors
{
    internal class CPUMonitor
    {
        private ISensor? _temperature;
        private float _clock;
        private ISensor? _load;
        private IHardware hardware;
        private float _currentFreq;

        public void Initialize(IHardware cpuhardware)
        {
            hardware = cpuhardware;
            // _temperature = SensorSelector.Find(cpuhardware, SensorType.Temperature, "Package", "Cores", "Total", "Core");
            _temperature = SensorSelector.Find(cpuhardware, SensorType.Temperature, "Core (Tctl/Tdie)");
            _load= SensorSelector.Find(cpuhardware, SensorType.Load, "CPU Total");
            //_clock = _currentFreq;
        }
        public void UpdateData(SensorSnapshot snapshot)
        {
            snapshot.CpuTemp = _temperature?.Value;
            snapshot.CpuLoad = _load?.Value;
            

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
        }
        public void CalcFreq()
        {
            
        }
    }
}
