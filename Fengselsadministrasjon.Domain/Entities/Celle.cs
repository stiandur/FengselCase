using Fengselsadministrasjon.Domain.Shared;

namespace Fengselsadministrasjon.Domain.Entities;

public class Celle
{
    private int Makskapasitet { get; }

    public string Cellenummer { get; }
    public List<Fange> Fanger { get; }

    public Celle(string cellenummer, int makskapasitet)
    {
        Cellenummer = cellenummer;
        Makskapasitet = makskapasitet;
        Fanger = new List<Fange>();
    }

    public int TilgjengeligKapasitet => Fanger.Count;
    public bool HarLedigKapasitet => TilgjengeligKapasitet < Makskapasitet;
    public bool HarFange(Guid id) => Fanger.Exists(x => x.Id == id);
    public bool HarFange(string? navn) => Fanger.Exists(x => x.Navn == navn);
    public Fange? HentFange(Guid id) => Fanger.Find(x => x.Id == id);
    public Fange? HentFange(string? navn) => Fanger.Find(x => x.Navn == navn);
    public bool KanIkkeBenytteCellePgaKjonn(Kjonn kjonn) => Fanger.Exists(x => x.Kjonn != kjonn);

    public void LeggTilFange(Fange fange)
    {
        if (HarFange(fange.Navn))
        {
            throw new ArgumentException($"Fange '{fange}' er allerede plassert i denne cellen.");
        }

        if (!HarLedigKapasitet)
        {
            throw new ArgumentException($"Celle {Cellenummer} har ikke plass til flere fanger.");
        }

        if(KanIkkeBenytteCellePgaKjonn(fange.Kjonn))
        {
            throw new ArgumentException($"Fange '{fange}' kan ikke plasseres i denne cellen da den allerede har en fange av motsatt kjønn.");
        }

        Fanger.Add(fange);
    }

    public Fange RemoveFange(Guid id)
    {
        var fange = HentFange(id);

        if (fange is null)
        {
            throw new ArgumentException($"Fange ved navn '{fange}' er ikke registrert på denne cellen.");
        }

        Fanger.Remove(fange);
        return fange;
    }

    public override string ToString()
    {
        var fanger = string.Join(", ", Fanger);
        return $"Cellnummer: {Cellenummer}  -  Kapasitet: ({TilgjengeligKapasitet}/{Makskapasitet})  - {fanger}";
    }

}
