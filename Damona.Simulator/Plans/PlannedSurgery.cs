using Damona.Simulator.Domain;

namespace Damona.Simulator.Plans;

public class PlannedSurgery
{
    public DateTime Time { get; set; }

    public TimeSpan Duration { get; set; }

    public ElectiveSurgery ElectiveSurgery { get; set; }

    public PlannedSurgery(DateTime time, TimeSpan duration, ElectiveSurgery electiveSurgery)
    {
        Time = time;
        Duration = duration;
        ElectiveSurgery = electiveSurgery;
    }
}