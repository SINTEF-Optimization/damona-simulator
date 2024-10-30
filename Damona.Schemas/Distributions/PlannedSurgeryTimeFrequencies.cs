using System.Text.Json;
using System.Text.Json.Serialization;

namespace Damona.Schemas.Distributions;

public class PlannedSurgeryTimeFrequencies
    : Dictionary<string, PlannedSurgeryTimeFrequencies.SingleSpecialtyFrequencies>
{
    public static PlannedSurgeryTimeFrequencies FromFile(string filename)
    {
        var path = Directory.GetCurrentDirectory();
        path = Path.Combine(path, "Damona.Cli");
        path = Path.Combine(path, "Distributions");
        path = Path.Combine(path, filename);

        using var streamReader = File.OpenRead(path);
        return JsonSerializer.Deserialize<PlannedSurgeryTimeFrequencies>(streamReader) ??
               throw new($"Failed to parse {typeof(PlannedSurgeryTimeFrequencies)} from file {filename}");
    }

    public class SingleSpecialtyFrequencies
    {
        [JsonPropertyName("counts")]
        public required double[] Counts { get; set; }

        [JsonPropertyName("values")]
        public required double[] Values { get; set; }
    }
}