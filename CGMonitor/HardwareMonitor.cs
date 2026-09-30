using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using Up;
using static CGMonitor.SensorSnapshot;

namespace CGMonitor
{
    public class HardwareMonitor
    {
        public SensorSnapshot Snapshot { get => _current; }
        private SensorSnapshot _current { get; set; }
        private Computer _computer { get; set; }
        public HardwareMonitor() {
            _computer = new Computer
            { IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = false,
                IsNetworkEnabled = false,
                IsStorageEnabled = false,
                IsPowerMonitorEnabled = true, };
            _computer.Open();
            _current = new SensorSnapshot();
            }
        public void Update()
        {

            _computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in _computer.Hardware)
            {
                Console.WriteLine("Hardware: {0}", hardware.Name);

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    Console.WriteLine("\tSubhardware: {0}", subhardware.Name);

                    foreach (ISensor sensor in subhardware.Sensors)
                        //Console.WriteLine("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                           
                            string valueStr = sensor.Value.HasValue ? $"{sensor.Value.Value}°C" : "NULL";
                            Console.WriteLine($"\t[ТЕМПЕРАТУРА] {sensor.Name}: {valueStr}");
                        }
                }

                foreach (ISensor sensor in hardware.Sensors)
                    Console.WriteLine("\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
            }
            
            
        }
        public void Dispose()
        {
            _computer.Close();
        }
    }
}
