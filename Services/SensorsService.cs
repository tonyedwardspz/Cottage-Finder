using CottageFinder.Interfaces;

namespace CottageFinder.Services;

internal class SensorsService : ISensorsService, IDisposable
{
    public event EventHandler<CompassReadingEventArgs>? CompassReadingChanged;
    public event EventHandler<bool>? CompassStatusChanged;

    public bool IsCompassSupported => Compass.Default.IsSupported;
    public bool IsCompassMonitoring => Compass.Default.IsMonitoring;

    private bool _disposed = false;

    public async Task<bool> StartCompassAsync()
    {
        if (!IsCompassSupported)
            return false;

        if (IsCompassMonitoring)
            return true;

        try
        {
            Compass.Default.ReadingChanged += OnCompassReadingChanged;
            Compass.Default.Start(SensorSpeed.UI);
            
            CompassStatusChanged?.Invoke(this, true);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to start compass: {ex.Message}");
            return false;
        }
    }

    public async Task StopCompassAsync()
    {
        if (!IsCompassMonitoring)
            return;

        try
        {
            Compass.Default.Stop();
            Compass.Default.ReadingChanged -= OnCompassReadingChanged;
            
            CompassStatusChanged?.Invoke(this, false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to stop compass: {ex.Message}");
        }
    }

    public async Task<bool> ToggleCompassAsync()
    {
        if (IsCompassMonitoring)
        {
            await StopCompassAsync();
            return false;
        }
        else
        {
            return await StartCompassAsync();
        }
    }

    private void OnCompassReadingChanged(object? sender, CompassChangedEventArgs e)
    {
        var args = new CompassReadingEventArgs(e.Reading.HeadingMagneticNorth);
        CompassReadingChanged?.Invoke(this, args);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (IsCompassMonitoring)
            {
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= OnCompassReadingChanged;
            }
            _disposed = true;
        }
    }
}
