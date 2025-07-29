namespace CottageFinder.Interfaces;

public interface ISensorsService
{
    event EventHandler<CompassReadingEventArgs>? CompassReadingChanged;
    event EventHandler<bool>? CompassStatusChanged;
    
    bool IsCompassSupported { get; }
    bool IsCompassMonitoring { get; }
    
    Task<bool> StartCompassAsync();
    Task StopCompassAsync();
    Task<bool> ToggleCompassAsync();
}

public class CompassReadingEventArgs : EventArgs
{
    public double HeadingMagneticNorth { get; }
    public string FormattedReading { get; }
    
    public CompassReadingEventArgs(double headingMagneticNorth)
    {
        HeadingMagneticNorth = headingMagneticNorth;
        FormattedReading = $"Compass: {headingMagneticNorth:F1}°";
    }
}