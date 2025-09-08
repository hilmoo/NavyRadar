namespace SharedModels.Models.TrackingModel;

using SharedModels.Models.ShipModel;

public abstract class ShipHistoryService
{
    private readonly Dictionary<string, List<Position>> _shipHistories = new();

    public void AddPositionToHistory(string shipId, Position position)
    {
        if (!_shipHistories.TryGetValue(shipId, out var value))
        {
            value = [];
            _shipHistories[shipId] = value;
        }

        value.Add(position);
    }

    private List<Position> GetShipHistory(string shipId)
    {
        return _shipHistories.TryGetValue(shipId, out var history) ? history : [];
    }

    public TrackingData GenerateTrackingData(string shipId, DateTime startTime, DateTime endTime)
    {
        var positions = GetShipHistory(shipId)
            .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
            .OrderBy(p => p.Timestamp)
            .ToList();

        if (positions.Count == 0)
        {
            return new TrackingData
            {
                ShipId = shipId,
                PositionHistory = new List<Position>(),
                StartTime = startTime,
                EndTime = endTime,
                TotalDistance = 0,
                AverageSpeed = 0,
                MaxSpeed = 0
            };
        }

        var totalDistance = CalculateTotalDistance(positions);
        var averageSpeed = positions.Where(p => p.SpeedOverGround.HasValue).Average(p => p.SpeedOverGround ?? 0);
        var maxSpeed = positions.Where(p => p.SpeedOverGround.HasValue).Max(p => p.SpeedOverGround ?? 0);

        return new TrackingData
        {
            ShipId = shipId,
            PositionHistory = positions,
            StartTime = startTime,
            EndTime = endTime,
            TotalDistance = totalDistance,
            AverageSpeed = averageSpeed,
            MaxSpeed = maxSpeed
        };
    }

    private static double CalculateTotalDistance(List<Position> positions)
    {
        double totalDistance = 0;
        for (var i = 1; i < positions.Count; i++)
        {
            totalDistance += CalculateDistance(positions[i - 1], positions[i]);
        }
        return totalDistance;
    }

    private static double CalculateDistance(Position pos1, Position pos2)
    {
        const double r = 3440.065;

        var lat1Rad = pos1.Latitude * Math.PI / 180;
        var lat2Rad = pos2.Latitude * Math.PI / 180;
        var deltaLatRad = (pos2.Latitude - pos1.Latitude) * Math.PI / 180;
        var deltaLonRad = (pos2.Longitude - pos1.Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return r * c;
    }
}
