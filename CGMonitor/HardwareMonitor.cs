using CGMonitor.Sensors;
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
        private readonly CPUMonitor _cpu = new CPUMonitor();
        private readonly UpdateVisitor _updateVisitor = new UpdateVisitor();
        public SensorSnapshot Snapshot { get => _current; }
        private SensorSnapshot _current { get; set; }
        private Computer _computer { get; set; }

        public HardwareMonitor() {
                                   _computer = new Computer
                                   {   IsCpuEnabled = true,
                                       IsGpuEnabled = true,
                                       IsMemoryEnabled = true,
                                       IsMotherboardEnabled = true,
                                       IsControllerEnabled = false,
                                       IsNetworkEnabled = false,
                                       IsStorageEnabled = false,
                                       IsPowerMonitorEnabled = true, 
                                   };
                                    _computer.Open();
                                    _current = new SensorSnapshot();
                                    InitializeMonitors();
                                   }
       

        private void InitializeMonitors()
        {
            _computer.Accept(_updateVisitor);

            foreach (IHardware hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    _cpu.Initialize(hardware); 
                }
            }
            foreach(IHardware hardware in _computer.Hardware)
            {
                if(hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuIntel|| hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    //_gpu.Initialize(hardware)
                }
            }
        }

        public void Update()
        {
            _computer.Accept(_updateVisitor);
            _cpu.UpdateData(_current);
        }

        public void Dispose()
        {
            _computer.Close();
        }
    }
}
