using Damona.Schemas.Common;

namespace Damona.Schemas.Responses;

public class ExecutedEmergencyDto
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required EmergencyDto Surgery { get; set; }
    public required string? OperatingTheatre { get; set; }
}