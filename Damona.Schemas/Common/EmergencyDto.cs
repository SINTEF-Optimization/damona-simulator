namespace Damona.Schemas.Common;

public class EmergencyDto
{
    public required string Specialty { get; set; }
    public required TimeSpan EstimatedDuration { get; set; }
    public DateTime? EmergencyDate { get; set; }
    public required int Urgency { get; set; }
}