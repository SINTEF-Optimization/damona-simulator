namespace Damona.Schemas.Common;

public class ElectiveSurgeryDto
{
    public required string Specialty { get; set; }
    public required TimeSpan EstimatedDuration { get; set; }
    public DateTime? ReferralDate { get; set; }
    public DateTime? Deadline { get; set; }
}