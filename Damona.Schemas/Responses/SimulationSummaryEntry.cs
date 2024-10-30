namespace Damona.Schemas.Responses;

public class SimulationSummaryEntry
{
    public int Initial { get; set; }
    public int New { get; set; }
    public int Completed { get; set; }
    public int Final => Initial + New - Completed;

    public List<SimulationPeriodEntry> Periods { get; } = new();
}