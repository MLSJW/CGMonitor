using ClickableTransparentOverlay;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;


namespace CGMonitor.Overl
{
    
    public class OverlayApp : Overlay
    {
        private bool _isFontLoaded = false;
        private readonly Stopwatch _gpuStopwatch = Stopwatch.StartNew();
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly HardwareMonitor _hardwareMonitor;
        public OverlayApp(HardwareMonitor hardwareMonitor) : base("CGMonitor")
        {
            _hardwareMonitor = hardwareMonitor;
        }
        private void ReplaceFontArial()
        {
            
            if (!_isFontLoaded)
            {
               ImGuiIOPtr io = ImGui.GetIO();
               string fontPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts), "arial.ttf");
               if (File.Exists(fontPath))
               {
                    ReplaceFont(fontPath, 20, FontGlyphRangeType.English);
               }
               _isFontLoaded = true;
             }
        }
        
        private void SetFrame()
        {
            const long FrameTime = 33;

            long elapsed = _gpuStopwatch.ElapsedMilliseconds;
            long remaining = FrameTime - elapsed;
            if (remaining > 2)
            {
                Thread.Sleep((int)(remaining - 1)); 
            }

            while (_gpuStopwatch.ElapsedMilliseconds < FrameTime)
            {
                Thread.Yield(); 
            }

            _gpuStopwatch.Restart();
        }

        private void UpdateHardwareMonitor()
        {
            if (_stopwatch.ElapsedMilliseconds > 1000)
            {
                _hardwareMonitor.Update();
                _stopwatch.Restart();
            }
        }
        
        protected override void Render()
        {
            ImGui.Begin(
                "CGMonitor",
                //ImGuiWindowFlags.NoInputs |
                ImGuiWindowFlags.AlwaysAutoResize 
                );
            ReplaceFontArial();
            SetFrame();
            UpdateHardwareMonitor();

            SensorSnapshot snapshot = _hardwareMonitor.Snapshot;
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.133f, 0.847f, 1.0f, 1.0f), $"{snapshot.CpuName}");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Text("Temp CPU: ");
            ImGui.SameLine(0.0f, 0.0f); 
            Vector4 valueColor;
            if (snapshot.CpuTemp >= 75 && snapshot.CpuTemp < 90)
            {
                valueColor = new Vector4(1.0f, 0.4f, 0.0f, 1.0f); 
            }
            else if (snapshot.CpuTemp >= 90)
            {
                valueColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); 
            }
            else
            {
                valueColor = new Vector4(0.0f, 1.0f, 1.0f, 1.0f); 
            }
            ImGui.TextColored(valueColor, $"{snapshot.CpuTemp:F0}°C");

            ImGui.Text($"Freq CPU: {snapshot.CpuClocks:F0}MHz");
            ImGui.Text($"Load CPU: {snapshot.CpuLoad:F0}%%");
            ImGui.Text($"Power CPU: {snapshot.CpuPower:F0} W");
            ImGui.Separator();
            float? gpuTempToShow = snapshot.GpuTemp ?? snapshot.GpuTempMobile;
            float? gpuPowerToShow = snapshot.GpuPower ?? snapshot.GpuPowerMobile;
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.11f, 1.0f, 0.153f, 1.0f), $"{snapshot.GpuName}");
            ImGui.SetWindowFontScale(1.0f);
            if (gpuTempToShow >= 75 && gpuTempToShow < 90)
            {
                valueColor = new Vector4(1.0f, 0.4f, 0.0f, 1.0f);
            }
            else if (gpuTempToShow >= 90)
            {
                valueColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                valueColor = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);
            }
            ImGui.Text("Temp GPU: ");
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.TextColored(valueColor, $"{gpuTempToShow:F0}°C");
            ImGui.Text($"Power GPU: {gpuPowerToShow ?? 0:F0} W");
            ImGui.Text($"FPS: {snapshot.GpuFPS:F0} FPS");
            ImGui.Text($"Load GPU: {snapshot.GpuLoad:F0}%%");
            ImGui.Separator();
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(0.953f, 0.525f, 0.153f, 1.0f), "RAM");
            ImGui.SetWindowFontScale(1.0f);
            ImGui.Text($"Load RAM: {snapshot.RAMLoad:F0}%%");
            ImGui.Text($"Used RAM: {snapshot.RAMUsed:F1}/{snapshot.RAMTotal:F1} Gb");
            ImGui.Separator();
          
            if (snapshot.FrameTimeArray != null && snapshot.FrameTimeArray.Length > 0)
            {
                ImGui.Text($"Frametime: {snapshot.CurrentFrameTime:F1} ms");

                System.Numerics.Vector2 graphPos = ImGui.GetCursorScreenPos();
                System.Numerics.Vector2 graphSize = new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 75); 
                ImGui.Dummy(graphSize);

                float minScale = 0.0f;
                float maxScale = 50.0f;

                var drawList = ImGui.GetWindowDrawList();

                float y60fps = (graphPos.Y + graphSize.Y) - (graphSize.Y * (16.6f / maxScale));
                uint gridColor = ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(1f, 1f, 1f, 0.12f));

                int pointsCount = snapshot.FrameTimeArray.Length;
                float xStep = graphSize.X / (pointsCount - 1);

                System.Numerics.Vector2? previousPoint = null;

                for (int i = 0; i < pointsCount; i++)
                {
                    float value = Math.Clamp(snapshot.FrameTimeArray[i], minScale, maxScale);

                    float normalizedY = (value - minScale) / (maxScale - minScale);
                    float pointX = graphPos.X + (i * xStep);
                    float pointY = (graphPos.Y + graphSize.Y) - (graphSize.Y * normalizedY);

                    System.Numerics.Vector2 currentPoint = new System.Numerics.Vector2(pointX, pointY);

                    if (previousPoint.HasValue)
                    {
                        uint pointColor = snapshot.FrameTimeArray[i] > 30.0f
                            ? ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(1.0f, 0.2f, 0.2f, 0.9f))  
                            : ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.0f, 1.0f, 0.0f, 0.8f)); 

                        System.Numerics.Vector2 cornerPoint = new System.Numerics.Vector2(currentPoint.X, previousPoint.Value.Y);
                        drawList.AddLine(previousPoint.Value, cornerPoint, pointColor, 1.5f);
                        drawList.AddLine(cornerPoint, currentPoint, pointColor, 1.5f);
                    }
                    previousPoint = currentPoint;
                }
                uint maxTextColor = ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(1f, 0.4f, 0.4f, 0.6f));
                drawList.AddText(new System.Numerics.Vector2(graphPos.X + graphSize.X - 100, graphPos.Y + 3), maxTextColor, $"Max: {snapshot.MaxFrameTime:F1} ms");
            }
        }
    }
}
