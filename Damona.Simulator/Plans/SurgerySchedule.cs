using Damona.Simulator.Domain;

namespace Damona.Simulator.Plans;

public class SurgerySchedule
{
    public List<OperatingTheatreSchedule> TheatreSchedules { get; } = new();

    public DateTime FindNextTimeSlot(ElectiveSurgery electiveSurgery)
    {
        throw new NotImplementedException();
    }
}