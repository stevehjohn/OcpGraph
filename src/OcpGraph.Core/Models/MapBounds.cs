namespace OcpGraph.Core.Models;

public readonly record struct MapBounds(double MinLatitude, double MaxLatitude, double MinLongitude, double MaxLongitude)
{
    public static MapBounds FromCentre(double centreLatitude, double centreLongitude, double widthMetres, double heightMetres)
    {
        const double metresPerDegreeLatitude = 111_320d;

        var halfLatitudeDelta = heightMetres / 2d / metresPerDegreeLatitude;

        var metresPerDegreeLongitude = metresPerDegreeLatitude * Math.Cos(centreLatitude * Math.PI / 180d);

        var halfLongitudeDelta = widthMetres / 2d / metresPerDegreeLongitude;

        return new MapBounds(centreLatitude - halfLatitudeDelta, centreLatitude + halfLatitudeDelta, centreLongitude - halfLongitudeDelta, centreLongitude + halfLongitudeDelta);
    }
}