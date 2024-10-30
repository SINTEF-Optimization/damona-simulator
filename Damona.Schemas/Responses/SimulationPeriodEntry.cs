namespace Damona.Schemas.Responses;

public class SimulationPeriodEntry
{
    public DateOnly Month { get; set; }
    public int Initial { get; set; }
    public int New { get; set; }
    public int Completed { get; set; }
    public int Final => Initial + New - Completed;
}