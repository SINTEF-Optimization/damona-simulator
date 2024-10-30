namespace Damona.Schemas.Distributions;

public class SurgeryDistributions
{
    /// <summary>
    ///     Distribution of emergencies throughout the day
    /// </summary>
    public IEnumerable<EmergencyFrequencies>? Emergencies { get; set; }

    /// <summary>
    ///     Surgery time distribution for elective surgeries
    /// </summary>
    public PlannedSurgeryTimeFrequencies? ElectiveSurgeryTime { get; set; }

    /// <summary>
    ///     Surgery time distribution for emergencies
    /// </summary>
    public IEnumerable<PlannedSurgeryTime>? EmergencySurgeryTime { get; set; }
}