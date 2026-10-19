using CGMonitor;
using LibreHardwareMonitor.Hardware;
using System;
using System.Runtime.InteropServices;

public class AmdApuMonitor : IDisposable
{
    private const string AdlDll = "atiadlxx.dll";
    private const int AMD_ODN_TEMPERATURE_EDGE = 1;

    public delegate IntPtr ADL_Main_Memory_Alloc(int iSize);

    [DllImport(AdlDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int iEnumConnectedAdapters, out IntPtr context);

    [DllImport(AdlDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ADL2_Main_Control_Destroy(IntPtr context);

    [DllImport(AdlDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ADL2_OverdriveN_Temperature_Get(
        IntPtr context,
        int iAdapterIndex,
        int iTemperatureType,
        out int lpTemperature
    );

    private static IntPtr AllocMemory(int iSize) => Marshal.AllocHGlobal(iSize);

    private IntPtr _context = IntPtr.Zero;
    private bool _isInitialized = false;
    private bool _dllMissing = false; 

    public bool Initialize()
    {
        if (_dllMissing) return false;
        if (_isInitialized) return true;

        try
        {
            return TryCreateControl();
        }
        catch (DllNotFoundException)
        {
            _dllMissing = true;
            _isInitialized = false;
            return false;
        }
        catch (Exception)
        {
            _isInitialized = false;
            return false;
        }
    }

    private bool TryCreateControl()
    {
        int initResult = ADL2_Main_Control_Create(AllocMemory, 1, out _context);
        if (initResult == 0)
        {
            _isInitialized = true;
            return true;
        }
        return false;
    }

    public float? ReadTemperature()
    {
        if (_dllMissing) return null;

        if (!_isInitialized || _context == IntPtr.Zero)
        {
            if (!Initialize()) return null;
        }

        try
        {
            return TryGetTemperature();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private float? TryGetTemperature()
    {
        for (int adapterIndex = 0; adapterIndex < 4; adapterIndex++)
        {
            int rawTemperature = 0;
            int res = ADL2_OverdriveN_Temperature_Get(
                _context,
                adapterIndex,
                AMD_ODN_TEMPERATURE_EDGE,
                out rawTemperature
            );

            if (res == 0 && rawTemperature > 0)
            {
                return rawTemperature / 1000f;
            }
        }
        return null;
    }

    private ISensor? _power;

    public void Initialize(IHardware hardware)
    {
        _power = SensorSelector.Find(hardware, SensorType.Power, "GPU Core", "GPU Package");
    }

    public void UpdateData(SensorSnapshot snapshot)
    {
        try
        {
            snapshot.GpuTempMobile = ReadTemperature();
        }
        catch (Exception)
        {
            snapshot.GpuTempMobile = null; 
        }

        snapshot.GpuPower = _power?.Value;
    }

    public void Dispose()
    {
        if (_context != IntPtr.Zero)
        {
            try
            {
                ADL2_Main_Control_Destroy(_context);
            }
            catch { }
            _context = IntPtr.Zero;
            _isInitialized = false;
        }
    }
}
