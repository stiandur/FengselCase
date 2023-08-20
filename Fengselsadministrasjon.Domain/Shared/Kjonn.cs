namespace Fengselsadministrasjon.Domain.Shared;

public record struct Kjonn
{
    public static readonly Kjonn Mann = new("Mann");
    public static readonly Kjonn Kvinne = new("Kvinne");

    string Navn;

    private Kjonn(string value)
    {
        Navn = value;
    }

    public static List<string> ListValues()
    {
        return new List<string>
        {
            Mann.Navn,
            Kvinne.Navn
        };
    }

    public static bool IsValid(string value) => value.ToLowerInvariant() switch
    {
        "mann" => true,
        "kvinne" => true,
        _ => false
    };

    public static Kjonn FromString(string value) => value.ToLowerInvariant() switch
    {
        "mann" => Mann,
        "kvinne" => Kvinne,
        _ => throw new ArgumentOutOfRangeException($"Klarte ikke å mappe følgende verdi for kjønn: {value}"),
    };
}
