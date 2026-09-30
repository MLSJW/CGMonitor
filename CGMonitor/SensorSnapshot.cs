using System;

namespace CGMonitor
{
    public class SensorSnapshot
    {
       
        public float? CpuLoad { get; set; }
        public float? CpuTemp { get; set; }
        public float? CpuClocks { get; set; }

   
        public float? GpuLoad { get; set; }
        public float? GpuTemp { get; set; }
        public float? GpuMemLoad { get; set; } 
        public float? GpuMemAll { get; set; }  
        public float? GpuClocks { get; set; }

      
        public float? PhysMemLoad { get; set; }   
        public float? PhysMemAll { get; set; }    
        public float? PhysMemClocks { get; set; } 
    }
}
