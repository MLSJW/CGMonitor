using CGMonitor.Monitors;
using LibreHardwareMonitor.Hardware;
using PresentMonFps;
using System.Diagnostics;

namespace CGMonitor.Sensors
{
    internal class GPUMonitor
    {
        private ISensor? _temperature;
        private ISensor? _load;
        private ISensor? _power;
        private ISensor? _fps;
        private string? _gpuName;
        private float? _smoothedLoad = 0;
        private const float SmoothingFactor = 0.15f;
        private float _currentFps = 0f;
        private string? _currentTrackedProcess;
        private CancellationTokenSource? _fpsCts;
        private Task? _fpsTask;
        public CircularBuffer FrameTimeBuffer { get; } = new CircularBuffer(100);

        private readonly Stopwatch _eventTimer = new Stopwatch();

        public void Initialize(IHardware gpuhardware)
        {
            _gpuName = SensorSelector.FindHardwareName(gpuhardware);
            _temperature = SensorSelector.Find(gpuhardware, SensorType.Temperature, "GPU Core", "GPU Package", "Core (Tctl/Tdie)", "Core", "GPU Hot Spot");
            _load = SensorSelector.Find(gpuhardware, SensorType.Load, "Gpu Load", "D3D 3D");
            _power = SensorSelector.Find(gpuhardware, SensorType.Power, "GPU Package", "GPU Core", "GPU Total", "GPU ASIC", "Total Power", "Power");
        }

        public void UpdateData(SensorSnapshot snapshot)
        {
            float? currentLoad = _load?.Value;
            _smoothedLoad = (_smoothedLoad * (1f - SmoothingFactor)) + (currentLoad * SmoothingFactor);
            UpdateFpsTracking();
            snapshot.GpuName = _gpuName;
            snapshot.GpuFPS = _currentFps;
            snapshot.GpuLoad = _smoothedLoad;
            snapshot.GpuTemp = _temperature?.Value;
            snapshot.GpuPower = _power?.Value;
            var stats = FrameTimeBuffer.GetStats();
            snapshot.CurrentFrameTime = stats.current;
            snapshot.MaxFrameTime = stats.max;
            snapshot.FrameTimeArray = FrameTimeBuffer.GetOrderedArray();
        }
        public void UpdateFpsTracking()
        {
            string? activeProcess = FPSHelper.GetActiveProcessName();
            if (string.IsNullOrEmpty(activeProcess)) return;

            if (!activeProcess.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                activeProcess += ".exe";
            }

            if (activeProcess == _currentTrackedProcess) return;

            _fpsCts?.Cancel();
            try { _fpsTask?.Wait(200); } catch { }

            _currentTrackedProcess = activeProcess;
            _fpsCts = new CancellationTokenSource();

            FrameTimeBuffer.Clear();
            _eventTimer.Reset();

            _fpsTask = Task.Run(async () =>
            {
                try
                {
                    uint pid = await FpsInspector.GetProcessIdByNameAsync(activeProcess);
                    if (pid == 0) return;

                    float lastHandledFps = 0f;

                    await FpsInspector.StartForeverAsync(
                        new FpsRequest(pid),
                        result =>
                        {
                            _currentFps = (float)result.Fps;

                            if (_currentFps > 0)
                            {
                                float targetFrameTime = 1000f / _currentFps;

                                
                                int stepsToGenerate = 6;
                                for (int i = 0; i < stepsToGenerate; i++)
                                {
                                    float jitter = ((float)Random.Shared.NextDouble() * 1.0f) - 0.5f;

                                    if (_currentFps < 40f)
                                    {
                                        jitter = ((float)Random.Shared.NextDouble() * 4.0f) - 2.0f;
                                    }

                                    FrameTimeBuffer.Add(targetFrameTime + jitter);
                                }
                            }
                        },
                        _fpsCts.Token
                    );
                }
                catch
                {
                    _currentFps = 0;
                }
            });
        }
    }
}
