namespace Damona.Simulator.Domain;

public class Surgery
{
    public required Specialty Specialty { get; set; }

    // public string? SurgeryCode { get; set; }

    public required TimeSpan EstimatedDuration { get; set; }
}