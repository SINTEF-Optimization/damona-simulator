namespace Damona.Schemas.Distributions;

public class PlannedSurgeryTime
{
    public required int Urgency { get; set; }

    public required PlannedSurgeryTimeFrequencies Frequencies { get; set; }
}