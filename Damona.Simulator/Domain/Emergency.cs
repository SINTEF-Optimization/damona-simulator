namespace Damona.Simulator.Domain;

public class Emergency : Surgery
{
    public required DateTime EmergencyDate { get; set; }

    public required Urgency Urgency { get; set; }
}