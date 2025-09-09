
namespace CottageFinder.Models;

public class BearingResult
{
    /// <summary>
    /// Bearing angle in degrees (0-360°), where 0° is North, 90° is East, 180° is South, 270° is West
    /// </summary>
    public double BearingDegrees { get; set; }

    /// <summary>
    /// Bearing angle in radians
    /// </summary>
    public double BearingRadians { get; set; }

    /// <summary>
    /// Compass direction as a string (N, NE, E, SE, S, SW, W, NW)
    /// </summary>
    public string CompassDirection { get; set; } = string.Empty;

    /// <summary>
    /// Distance between the two points in meters
    /// </summary>
    public double DistanceMeters { get; set; }

    /// <summary>
    /// Distance between the two points in kilometers
    /// </summary>
    public double DistanceKilometers => DistanceMeters / 1000.0;

    /// <summary>
    /// Starting location coordinates
    /// </summary>
    public Location FromLocation { get; set; } = new Location();

    /// <summary>
    /// Target location coordinates
    /// </summary>
    public Location ToLocation { get; set; } = new Location();

    /// <summary>
    /// Timestamp when the bearing was calculated
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}
