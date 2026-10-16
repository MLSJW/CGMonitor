using ClickableTransparentOverlay;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Vortice.DXGI;

namespace CGMonitor.Overl
{
    public class OverlayApp : Overlay
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly HardwareMonitor _hardwareMonitor;
        public OverlayApp(HardwareMonitor hardwareMonitor)
        {
            _hardwareMonitor = hardwareMonitor;
        }
        public void CreateSnapshot()
        {

        }

        protected override void Render()
        {
            if (_stopwatch.ElapsedMilliseconds > 1000)
            {
                _hardwareMonitor.Update();
                _stopwatch.Restart();
            }
            SensorSnapshot snapshot = _hardwareMonitor.Snapshot;
            
                ImGui.Text($"Temp CPU: {snapshot.CpuTemp:F0}°C");
                ImGui.Text($"Freq CPU: {snapshot.CpuClocks:F0}MHz");
                ImGui.Text($"Load CPU: {snapshot.CpuLoad:F0}%%");
                ImGui.Text($"Power CPU: {snapshot.CpuPower:F0} W");

                ImGui.Text($"Temp GPU: {snapshot.GpuTemp}°C");
                ImGui.Text($"Power GPU: {(snapshot.GpuPower == null ? snapshot.GpuPowerMobile : null):F0} W");
                ImGui.Text($"FPS: {snapshot.GpuFPS:F0} FPS");
                ImGui.Text($"Load GPU: {snapshot.GpuLoad:F0}%%");

                ImGui.Text($"Load RAM: {snapshot.RAMLoad:F0}%%");
                ImGui.Text($"Used RAM: {snapshot.RAMUsed:F1}/{snapshot.RAMTotal:F1} Gb");
        }

    }    
}
