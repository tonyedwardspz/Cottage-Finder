using Microsoft.Maui.Devices.Sensors;

namespace CottageFinder.Interfaces;

public interface ILocationService
{
    event EventHandler<LocationEventArgs>? LocationChanged;
    event EventHandler<bool>? LocationStatusChanged;
    
    bool IsLocationSupported { get; }
    bool IsLocationMonitoring { get; }
    
    Task<Location?> GetCurrentLocationAsync();
    Task<bool> StartLocationUpdatesAsync();
    Task StopLocationUpdatesAsync();
    Task<bool> ToggleLocationUpdatesAsync();
}

public class LocationEventArgs : EventArgs
{
    public Location Location { get; }
    public DateTime Timestamp { get; }

    public LocationEventArgs(Location location)
    {
        Location = location;
        Timestamp = DateTime.Now;
    }
}