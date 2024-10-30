using System.Text.Json;
using Damona.Schemas.Common;
using Damona.Schemas.Resources;

namespace Damona.Schemas.Distributions;

public class EmergencyFrequencies
{
    public required int Urgency { get; set; }
    public required WeekdayFrequencies Frequencies { get; set; }

    // public static EmergencyFrequencies FromFile(string filename)
    // {
    //     var path = Directory.GetCurrentDirectory();
    //     path = Path.Combine(path, "Damona.Cli");
    //     path = Path.Combine(path, "Distributions");
    //     path = Path.Combine(path, filename);
    //
    //     using var streamReader = File.OpenRead(path);
    //     return JsonSerializer.Deserialize<EmergencyFrequencies>(streamReader) ??
    //            throw new($"Failed to parse {typeof(EmergencyFrequencies)} from file {filename}");
    // }

    public class WeekdayFrequencies
    {
        public Dictionary<string, int[]> Weekday { get; set; }
        public Dictionary<string, int[]> Weekend { get; set; }
    }
}