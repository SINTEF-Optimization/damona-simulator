using Damona.Schemas.Distributions;
using Damona.Schemas.Resources;

namespace Damona.Schemas;

/// <summary>
///     The simulation scenario to run.
/// </summary>
public class Scenario
{
    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public Configuration Configuration { get; set; } = new();

    /// <summary>
    ///     Resource plan for the hospital.
    /// </summary>
    public HospitalResourcePlanDto? ResourcePlan { get; set; }

    /// <summary>
    ///     Initial waiting list for elective surgeries
    /// </summary>
    public IDictionary<string, int>? InitialWaitingList { get; set; }

    public SurgeryDistributions? Distributions { get; set; }
}