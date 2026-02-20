namespace HotelSearch.API.Helpers;
public readonly record struct GeoLocation(double Latitude, double Longitude);

public static class GeoLocationExtensions
{
    public static double GetDistance(GeoLocation a, GeoLocation b)
    {
        const double R = 6371.0;
        static double ToRad(double deg) => deg * (Math.PI / 180.0);

        var dLat = ToRad(b.Latitude - a.Latitude);
        var dLon = ToRad(b.Longitude - a.Longitude);

        var lat1 = ToRad(a.Latitude);
        var lat2 = ToRad(b.Latitude);

        var h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Asin(Math.Sqrt(h));
        return R * c;
    }
}