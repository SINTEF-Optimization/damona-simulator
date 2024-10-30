namespace Damona.Simulator.Domain;

public enum AnesthesiaType
{
    Local,
    Spinal,
    Narcosis,
    NarcosisAndSpinal,
    None
}

public static class AnesthesiaTypeExtensions
{
    public static TimeSpan GetDuration(this AnesthesiaType anesthesiaType)
    {
        return anesthesiaType switch
        {
            AnesthesiaType.Local => TimeSpan.FromMinutes(15),
            AnesthesiaType.Spinal => TimeSpan.FromMinutes(60),
            AnesthesiaType.Narcosis => TimeSpan.FromMinutes(50),
            AnesthesiaType.NarcosisAndSpinal => TimeSpan.FromMinutes(85),
            AnesthesiaType.None => TimeSpan.Zero,
            _ => TimeSpan.Zero
        };
    }
}