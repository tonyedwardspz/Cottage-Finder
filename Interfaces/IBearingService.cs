using Microsoft.Maui.Devices.Sensors;
using CottageFinder.Models;

namespace CottageFinder.Interfaces;

public interface IBearingService
{
    /// <summary>
    /// Calculates the bearing from the current location to the target coordinates
    /// </summary>
    /// <param name="targetLatitude">Target latitude in decimal degrees</param>
    /// <param name="targetLongitude">Target longitude in decimal degrees</param>
    /// <returns>Bearing result containing bearing angle and additional information, or null if current location is unavailable</returns>
    Task<BearingResult?> CalculateBearingToTargetAsync(double targetLatitude, double targetLongitude);

    /// <summary>
    /// Calculates the bearing between two specific coordinates
    /// </summary>
    /// <param name="fromLatitude">Starting latitude in decimal degrees</param>
    /// <param name="fromLongitude">Starting longitude in decimal degrees</param>
    /// <param name="toLatitude">Target latitude in decimal degrees</param>
    /// <param name="toLongitude">Target longitude in decimal degrees</param>
    /// <returns>Bearing result containing bearing angle and additional information</returns>
    BearingResult CalculateBearing(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude);

    /// <summary>
    /// Calculates the bearing from the current location to the target location
    /// </summary>
    /// <param name="targetLocation">Target location</param>
    /// <returns>Bearing result containing bearing angle and additional information, or null if current location is unavailable</returns>
    Task<BearingResult?> CalculateBearingToTargetAsync(Location targetLocation);

    /// <summary>
    /// Calculates the bearing between two locations
    /// </summary>
    /// <param name="fromLocation">Starting location</param>
    /// <param name="toLocation">Target location</param>
    /// <returns>Bearing result containing bearing angle and additional information</returns>
    BearingResult CalculateBearing(Location fromLocation, Location toLocation);
}