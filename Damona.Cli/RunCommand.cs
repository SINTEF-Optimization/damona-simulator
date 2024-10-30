using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AutoMapper;
using Damona.Examples;
using Damona.Schemas;
using Damona.Schemas.AutoMapper;
using Damona.Schemas.Responses;
using Damona.Simulator;
using Spectre.Console.Cli;

namespace Damona.Cli;

public sealed class RunCommand(SpecialtyMap specialtyMap, IMapper mapper)
    : Command<RunCommand.Settings>
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;
    private readonly IMapper _mapper = mapper;

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
        };

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<scenario>")]
        [Description("Path to the scenario file to use for the simulation")]
        public required string ScenarioFilePath { get; set; }

        [CommandOption("-o|--out")]
        [Description("Output path for the simulation summary")]
        public string? SummaryFilePath { get; init; }

        [CommandOption("-h|--history")]
        [Description("Output path for the simulation history")]
        public string? HistoryFilePath { get; set; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var scenario = ReadScenario(settings);
        _specialtyMap.SetEmergencyName(scenario.Configuration.EmergencyName);

        var builder = new FromScenarioSimulationBuilder(_specialtyMap, scenario);

        var sim = builder.Build();

        sim.Run();

        SaveSimulationSummary(settings, sim);
        SaveSimulationHistory(settings, sim);

        return 0;
    }

    private Scenario ReadScenario(Settings settings)
    {
        using var streamReader = File.OpenRead(settings.ScenarioFilePath);
        return JsonSerializer.Deserialize<Scenario>(streamReader, _jsonSerializerOptions)
               ?? throw new(
                   $"Failed to parse {typeof(Scenario)} from file {settings.SummaryFilePath}"
               );
    }

    private void SaveSimulationSummary(Settings settings, HospitalSimulation simulation)
    {
        var report = new SimulationReport
        {
            ElectiveSurgerySummary = _mapper.Map<Dictionary<string, SimulationSummaryEntry>>(
                simulation.ElectiveSurgeryMonitor!.Report()
            ),
            EmergencySummary = _mapper.Map<Dictionary<string, SimulationSummaryEntry>>(
                simulation.EmergencyMonitor!.Report()
            ),
        };

        var filename = settings.SummaryFilePath ?? "summary.json";
        var result = JsonSerializer.Serialize(
            report,
            new JsonSerializerOptions { WriteIndented = true }
        );
        Console.WriteLine(result);
        File.WriteAllText(filename, result);
    }

    private void SaveSimulationHistory(Settings settings, HospitalSimulation simulation)
    {
        var history = new SimulationHistory
        {
            ExecutedElectiveSurgeries = _mapper.Map<List<ExecutedElectiveSurgeryDto>>(
                simulation.ElectiveSurgeryMonitor!.Completed
            ),
            ExecutedEmergencies = _mapper.Map<List<ExecutedEmergencyDto>>(
                simulation.EmergencyMonitor!.Completed
            ),
        };

        var filename = settings.HistoryFilePath ?? "history.json";
        var result = JsonSerializer.Serialize(
            history,
            new JsonSerializerOptions { WriteIndented = true }
        );
        File.WriteAllText(filename, result);
    }
}