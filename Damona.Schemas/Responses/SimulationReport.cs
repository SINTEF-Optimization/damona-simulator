using System.Text.Json.Serialization;
using Damona.Schemas.Common;

namespace Damona.Schemas.Responses;

public class SimulationReport
{
    [JsonPropertyName("electiveSurgeries")]
    public Dictionary<string, SimulationSummaryEntry>? ElectiveSurgerySummary { get; set; }

    [JsonPropertyName("emergencies")]
    public Dictionary<string, SimulationSummaryEntry>? EmergencySummary { get; set; }
}