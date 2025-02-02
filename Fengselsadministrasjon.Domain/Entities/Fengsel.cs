﻿using Fengselsadministrasjon.Domain.Shared;

namespace Fengselsadministrasjon.Domain.Entities;

public class Fengsel
{
    public string Navn { get; }
    public string Beskrivelse { get; }
    public List<Celle> Celler { get; }

    public Fengsel(string navn, string beskrivelse)
    {
        Navn = navn;
        Beskrivelse = beskrivelse;
        Celler = new List<Celle>();
    }

    public void AddCelle(Celle celle)
    {
        Celler.Add(celle);
    }

    public void AddFange(Fange fange, string cellenummer)
    {
        var celle = GetCelle(cellenummer);
        celle.LeggTilFange(fange);
    }

    public void FlyttFange(Guid id, string cellenummer)
    {
        var fange = RemoveFangeFraCelle(id);
        AddFange(fange, cellenummer);
    }

    public Celle GetCelle(string? cellenummer)
    {
        var celle = Celler.Find(x => x.Cellenummer == cellenummer);

        if (celle is null)
        {
            throw new ArgumentException($"Fant ingen celle med cellenummer {cellenummer}");
        }

        return celle;
    }

    public Fange? GetFange(string? navn)
    {
        return Celler.Find(x => x.HarFange(navn))?.HentFange(navn);
    }

    public void GenererCeller(int antallEtasjer, int antallCellerPerEtasje, int makskapasitetPerCelle)
    {
        for (int etasjeNummer = 1; etasjeNummer <= antallEtasjer; etasjeNummer++)
        {
            for (int celleNummer = 1; celleNummer <= antallCellerPerEtasje; celleNummer++)
            {
                var celle = new Celle($"{etasjeNummer}0{celleNummer}", makskapasitetPerCelle);
                AddCelle(celle);
            }
        }
    }

    public List<Celle> GetTilgjengeligeCeller(Kjonn kjonn)
    {
        return Celler
            .Where(x => x.HarLedigKapasitet)
            .Where(x => !x.KanIkkeBenytteCellePgaKjonn(kjonn))
            .ToList();
    }

    public List<Fange> GetAlleFanger()
    {
        return Celler.SelectMany(x => x.Fanger).ToList();
    }

    public void LoslatFange(Guid id)
    {
        RemoveFangeFraCelle(id);
    }

    private Fange RemoveFangeFraCelle(Guid id)
    {
        var celle = Celler.Find(x => x.HarFange(id));

        if (celle is null)
        {
            throw new ArgumentException($"Fant ingen fange med id {id}");
        }

        var fange = celle.RemoveFange(id);
        return fange;
    }
}
