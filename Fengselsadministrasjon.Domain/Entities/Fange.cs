using Fengselsadministrasjon.Domain.Shared;

namespace Fengselsadministrasjon.Domain.Entities;

public class Fange
{
    public Fange(string navn,
                 int alder,
                 Kjonn kjonn,
                 DateTime fengslingsDatoFra,
                 DateTime fengslingsDatoTil)
    {
        Id = Guid.NewGuid();
        Navn = navn;
        Alder = alder;
        Kjonn = kjonn;
        FengslingsDatoFra = fengslingsDatoFra;
        FengslingsDatoTil = fengslingsDatoTil;
    }

    public Guid Id { get; set; }
    public string Navn { get; }
    public int Alder { get; }
    public Kjonn Kjonn { get; }
    public DateTime FengslingsDatoFra { get; }
    public DateTime FengslingsDatoTil { get; private set; }

    public override string ToString()
    {
        return Navn;
    }

}
