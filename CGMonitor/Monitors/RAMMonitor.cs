using LibreHardwareMonitor.Hardware;

namespace CGMonitor.Sensors
{
    internal class RAMMonitor
    {
        private ISensor? _temperature;
        private ISensor? _clock;
        private ISensor? _load;
        private ISensor? _used;
        private ISensor? _available;

        public void Initialize(IHardware ramhardware)
        {
            _clock = SensorSelector.Find(ramhardware, SensorType.Clock);
            _load = SensorSelector.Find(ramhardware, SensorType.Load, "Memory");
            _available = SensorSelector.Find(ramhardware, SensorType.Data, "Memory Available");
            _used = SensorSelector.Find(ramhardware, SensorType.Data, "Memory Used");
        }
        
        public void UpdateData(SensorSnapshot snapshot)
        {
            snapshot.RAMClocks = _clock?.Value;
            snapshot.RAMLoad = _load?.Value;
            snapshot.RAMTotal = _used?.Value + _available?.Value;
            snapshot.RAMUsed = _used?.Value;
        }
    }
}
