using System;
using System.Collections.Generic;
using System.Text;


namespace CGMonitor
{
    public class SensorSnapshot
    {
        public int CpuLoad { get; init; }
        public int CpuTemp { get; init; }
        public int CpuClocks { get; init; }
        public int GpuLoad { get; init; }
        public int GpuTemp { get; init; }
        public int GpuMemLoad { get; init; }
        public int GpuMemAll { get; init; }
        public int GpuClocks { get; init; }
        public int PhysMemLoad { get; init; }
        public int PhysMemAll { get; init; }
        public int PhysMemClocks { get; init; }


    }
}
