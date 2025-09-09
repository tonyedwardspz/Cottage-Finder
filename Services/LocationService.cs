using CottageFinder.Interfaces;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Authentication;

namespace CottageFinder.Services;

internal class LocationService : ILocationService, IDisposable
{
    public event EventHandler<LocationEventArgs>? LocationChanged;
    public event EventHandler<bool>? LocationStatusChanged;

    public bool IsLocationSupported => true; // Location is generally supported on all MAUI platforms
    public bool IsLocationMonitoring { get; private set; }

    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed = false;

    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    return null;
                }
            }

            var request = new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            };

            var location = await Geolocation.Default.GetLocationAsync(request);
            return location;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get location: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> StartLocationUpdatesAsync()
    {
        if (IsLocationMonitoring)
            return true;

        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    return false;
                }
            }

            _cancellationTokenSource = new CancellationTokenSource();
            IsLocationMonitoring = true;
            LocationStatusChanged?.Invoke(this, true);

            // Start location monitoring in background task
            _ = Task.Run(async () => await MonitorLocationAsync(_cancellationTokenSource.Token));

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to start location updates: {ex.Message}");
            IsLocationMonitoring = false;
            return false;
        }
    }

    public async Task StopLocationUpdatesAsync()
    {
        if (!IsLocationMonitoring)
            return;

        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            
            IsLocationMonitoring = false;
            LocationStatusChanged?.Invoke(this, false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to stop location updates: {ex.Message}");
        }
    }

    public async Task<bool> ToggleLocationUpdatesAsync()
    {
        if (IsLocationMonitoring)
        {
            await StopLocationUpdatesAsync();
            return false;
        }
        else
        {
            return await StartLocationUpdatesAsync();
        }
    }

    private async Task MonitorLocationAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var location = await GetCurrentLocationAsync();
                if (location != null)
                {
                    var args = new LocationEventArgs(location);
                    LocationChanged?.Invoke(this, args);
                }

                // Wait for 2 seconds before next update
                await Task.Delay(2000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in location monitoring: {ex.Message}");
                await Task.Delay(5000, cancellationToken); // Wait longer on error
            }
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (IsLocationMonitoring)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
            }
            _disposed = true;
        }
    }
}