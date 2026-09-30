using LibreHardwareMonitor.Hardware;
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

        }
    }
}
