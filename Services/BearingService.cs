using CottageFinder.Interfaces;
using Microsoft.Maui.Devices.Sensors;
using CottageFinder.Models;

namespace CottageFinder.Services;

internal class BearingService : IBearingService
{
    private readonly ILocationService _locationService;
    private const double EarthRadiusKm = 6371.0;

    public BearingService(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<BearingResult?> CalculateBearingToTargetAsync(double targetLatitude, double targetLongitude)
    {
        var currentLocation = await _locationService.GetCurrentLocationAsync();
        if (currentLocation == null)
            return null;

        return CalculateBearing(currentLocation.Latitude, currentLocation.Longitude, targetLatitude, targetLongitude);
    }

    public async Task<BearingResult?> CalculateBearingToTargetAsync(Location targetLocation)
    {
        var currentLocation = await _locationService.GetCurrentLocationAsync();
        if (currentLocation == null)
            return null;

        return CalculateBearing(currentLocation, targetLocation);
    }

    public BearingResult CalculateBearing(Location fromLocation, Location toLocation)
    {
        return CalculateBearing(fromLocation.Latitude, fromLocation.Longitude, toLocation.Latitude, toLocation.Longitude);
    }

    public BearingResult CalculateBearing(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
    {
        // Convert degrees to radians
        var lat1Rad = DegreesToRadians(fromLatitude);
        var lat2Rad = DegreesToRadians(toLatitude);
        var deltaLonRad = DegreesToRadians(toLongitude - fromLongitude);

        // Calculate bearing using the forward azimuth formula
        var y = Math.Sin(deltaLonRad) * Math.Cos(lat2Rad);
        var x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) - Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(deltaLonRad);
        
        var bearingRad = Math.Atan2(y, x);
        var bearingDeg = RadiansToDegrees(bearingRad);
        
        // Normalize to 0-360 degrees
        bearingDeg = (bearingDeg + 360) % 360;

        // Calculate distance using Haversine formula
        var distanceMeters = CalculateDistance(fromLatitude, fromLongitude, toLatitude, toLongitude);

        return new BearingResult
        {
            BearingDegrees = bearingDeg,
            BearingRadians = bearingRad,
            CompassDirection = GetCompassDirection(bearingDeg),
            DistanceMeters = distanceMeters,
            FromLocation = new Location(fromLatitude, fromLongitude),
            ToLocation = new Location(toLatitude, toLongitude),
            CalculatedAt = DateTime.Now
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula for calculating distance between two points on Earth
        var lat1Rad = DegreesToRadians(lat1);
        var lat2Rad = DegreesToRadians(lat2);
        var deltaLatRad = DegreesToRadians(lat2 - lat1);
        var deltaLonRad = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return EarthRadiusKm * c * 1000; // Convert km to meters
    }

    private string GetCompassDirection(double bearingDegrees)
    {
        var directions = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
        var index = (int)Math.Round(bearingDegrees / 22.5) % 16;
        return directions[index];
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static double RadiansToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }
}