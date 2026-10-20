namespace CGMonitor
{
    public class SensorSnapshot
    {
        public float[]? FrameTimeArray { get; set; } 
        public float CurrentFrameTime { get; set; }
        public float MaxFrameTime { get; set; }

        public string? CpuName { get; set; }
        public float? CpuLoad { get; set; }
        public float? CpuTemp { get; set; }
        public float? CpuClocks { get; set; }
        public float? CpuPower { get; set; }

        public string? GpuName { get; set; }
        public float? GpuLoad { get; set; }
        public float? GpuTemp { get; set; }
        public float? GpuTempMobile { get; set; }
        public float? GpuMemLoad { get; set; } 
        public float? GpuMemAll { get; set; }  
        public float? GpuClocks { get; set; }
        public float? GpuPower { get; set; }
        public float? GpuPowerMobile { get; set; }
        public float? GpuFPS { get; set; }

        public float? RAMLoad { get; set; }   
        public float? RAMTotal { get; set; }    
        public float? RAMClocks { get; set; }
        public float? RAMUsed { get; set; }
    }
}
