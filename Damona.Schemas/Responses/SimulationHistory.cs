using System.Text.Json.Serialization;

namespace Damona.Schemas.Responses;

public class SimulationHistory
{
    [JsonPropertyName("electiveSurgeries")]
    public IEnumerable<ExecutedElectiveSurgeryDto>? ExecutedElectiveSurgeries { get; set; }

    [JsonPropertyName("emergencies")]
    public IEnumerable<ExecutedEmergencyDto>? ExecutedEmergencies { get; set; }
}