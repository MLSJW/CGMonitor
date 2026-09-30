using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Interop.PowerMonitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGMonitor
{
    public class SensorSelector
    {
        public static ISensor? Find(IHardware hardware, SensorType type, params string[] preferredNameParts) 
        {
            if (preferredNameParts == null || hardware?.Sensors == null) { return null; }

            foreach (var param in preferredNameParts)
            {
                foreach (var sensor in hardware.Sensors) { 
                    if (sensor.SensorType == type && sensor.Value.HasValue && sensor.Name.Contains(param))
                    {
                        return sensor;
                    } 
                }
            }
            return null;
        }
    }
}
