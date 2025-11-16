using System.ComponentModel.DataAnnotations;
using NavyRadar.Shared.Entities;

namespace NavyRadar.Shared.Domain.Sailing;

public class UpdateSailStatusRequest
{
    [Required]
    public SailStatus Status { get; set; }
}