using Damona.Schemas.Common;

namespace Damona.Schemas.Responses;

public class ExecutedElectiveSurgeryDto
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required ElectiveSurgeryDto Surgery { get; set; }
    public required string? OperatingTheatre { get; set; }
}