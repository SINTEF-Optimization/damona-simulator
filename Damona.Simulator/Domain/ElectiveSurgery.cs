namespace Damona.Simulator.Domain;

public class ElectiveSurgery : Surgery
{
    public DateTime? ReferralDate { get; set; }
    public DateTime? Deadline { get; set; }
}