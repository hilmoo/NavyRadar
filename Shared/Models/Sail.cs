using System.ComponentModel.DataAnnotations;
using Shared.Util;

namespace Shared.Models;

public class Sail
{
    public int Id { get; set; }

    public int ShipId { get; set; }
    public int CaptainId { get; set; }
    public int OriginPortId { get; set; }
    public int DestinationPortId { get; set; }

    [Required] public required SailStatus Status { get; set; }

    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public decimal? TotalDistanceNm { get; set; }
    public decimal? AverageSpeedKnots { get; set; }
    public decimal? MaxSpeedKnots { get; set; }
}