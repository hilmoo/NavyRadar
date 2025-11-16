namespace NavyRadar.Shared.Domain.Sailing;

public record AddPositionRequest(
    double Latitude,
    double Longitude,
    double? SpeedKnots,
    int? HeadingDegrees
);