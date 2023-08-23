using Fengselsadministrasjon.Domain.Entities;
using Fengselsadministrasjon.Domain.Integrations;
using Fengselsadministrasjon.Domain.Shared;
using System.Globalization;

namespace Fengselsadministrasjon.ConsoleApp.Services
{
    public class FengselsadministrasjonService
    {
        const string seCelleoverSikt = "se celleoversikt";
        const string seCellekapasitet = "se cellekapasitet";
        const string nyFange = "ny fange";
        const string flyttFange = "flytt fange";
        const string loslatFange = "løslat fange";
        const string avslutt = "avslutt";

        private readonly IFangedataGateway _fangedataGateway;

        public FengselsadministrasjonService(IFangedataGateway fangedataGateway)
        {
            _fangedataGateway = fangedataGateway;
        }

        public async Task Run()
        {
            PrintWelcome();

            var fengsel = new Fengsel("Oslo Fengsel", "Oslo fengsel ligger på Grønland, og er en enhet med høyt sikkerhetsnivå.\r\nFengselet er et av Norges minste med plass til 14 kvinnelige og 14 mannlige innsatte.");
            fengsel.GenererCeller(antallEtasjer: 7, antallCellerPerEtasje: 2, makskapasitetPerCelle: 2);

            var fangedataResponse = await _fangedataGateway.GetFanger();

            PopulateFanger(fengsel, fangedataResponse);

            PrintIntro(fengsel);

            var gyldigeKommandoer = new List<string> { seCelleoverSikt, seCellekapasitet, nyFange, flyttFange, loslatFange, avslutt };
            var kommando = Console.ReadLine();

            do
            {
                Console.WriteLine();
                ShortWait();

                if (!gyldigeKommandoer.Exists(x => x == kommando))
                {
                    Console.WriteLine("Oppgitt kommando er ikke en gyldig kommando, vennligst prøv på nytt.");
                    Console.WriteLine();
                    Console.WriteLine("Kommando:");
                    kommando = Console.ReadLine();
                    break;
                }

                if (kommando == seCelleoverSikt)
                {
                    fengsel.Celler.ForEach(celle => Console.WriteLine(celle));
                }

                if(kommando == seCellekapasitet)
                {
                    Console.WriteLine("Hvilken celle ønsker du å se kapasitet for?");
                    var celle = GetCellenummer(fengsel);

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine(celle);
                }

                if (kommando == nyFange)
                {
                    Console.WriteLine("For å kunne legge til en ny fange trenger vi informasjon om fangen. Vennligst oppgi følgende:");

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine("Fullt navn på fangen:");
                    var navn = GetNavn();

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine("Fangens alder:");
                    var alder = GetAlder();

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine("Fangens kjønn:");
                    var kjonn = GetKjonn();

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine("Til hvilken dato skal fangen være fengslet? Eksempel på gyldig datoformat: 22-01-2000");
                    var fengslingsDatoTil = GetFengslingsDato();

                    var fange = new Fange(
                        navn: navn,
                        alder: alder,
                        kjonn: Kjonn.FromString(kjonn),
                        fengslingsDatoFra: DateTime.Now,
                        fengslingsDatoTil: fengslingsDatoTil
                    );

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine("Her er en liste over tilgjengelige celler:");
                    Console.WriteLine();

                    var tilgjengeligeCeller = fengsel.GetTilgjengeligeCeller(fange.Kjonn);
                    tilgjengeligeCeller.ForEach(celle => Console.WriteLine(celle));

                    Console.WriteLine();
                    Console.WriteLine("Hvilken celle ønsker du å plassere fangen i?");
                    var celle = GetCellenummer(fengsel);

                    try
                    {
                        fengsel.AddFange(fange, celle.Cellenummer);

                        Console.WriteLine();
                        Console.WriteLine($"Fange ved navn {fange.Navn} er nå plassert i celle med nummer {celle.Cellenummer}");
                    }
                    catch (Exception e)
                    {
                        PrintFeilmelding(e);
                    }
                }

                if (kommando == loslatFange)
                {
                    Console.WriteLine();
                    fengsel.GetAlleFanger().ForEach(fange => Console.WriteLine(fange));
                    Console.WriteLine();
                    Console.WriteLine("Vennligst oppgi fullt navn på personen du ønsker å løslate:");

                    var fange = HentFange(fengsel);
                    try
                    {
                        fengsel.LoslatFange(fange.Id);
                        Console.WriteLine();
                        ShortWait();
                        Console.WriteLine($"Fange ved navn {fange.Navn} har blitt løslatt!");
                    }
                    catch (Exception e)
                    {
                        PrintFeilmelding(e);
                    }
                }

                if (kommando == flyttFange)
                {
                    Console.WriteLine("Vennligst oppgi fullt navn på personen du ønsker å flytte:");
                    Console.WriteLine();
                    fengsel.GetAlleFanger().ForEach(fange => Console.WriteLine(fange));
                    Console.WriteLine();
                    Console.WriteLine("Kommando:");

                    var fange = HentFange(fengsel);

                    ShortWait();
                    Console.WriteLine();
                    Console.WriteLine("Hvilken celle ønsker du å plassere fangen i?");
                    Console.WriteLine();
                    Console.WriteLine("Kommando:");

                    var tilgjengeligeCeller = fengsel.GetTilgjengeligeCeller(fange.Kjonn);
                    tilgjengeligeCeller.ForEach(celle => Console.WriteLine(celle));
                    Console.WriteLine();

                    var celle = GetCellenummer(fengsel);

                    try
                    {
                        fengsel.FlyttFange(fange.Id, celle.Cellenummer);
                        Console.WriteLine($"{fange.Navn} har blitt flyttet til celle {celle.Cellenummer}.");
                    }
                    catch (Exception e)
                    {
                        PrintFeilmelding(e);
                    }
                }

                ShortWait();
                Console.WriteLine();
                Console.WriteLine("Ønsker du å gjøre noe mer?");
                ShortWait();
                PrintKommandoer();

                kommando = Console.ReadLine();

            } while (kommando != avslutt);

        }

        private static void PrintFeilmelding(Exception e)
        {
            Console.WriteLine($"Noe gikk galt: {e.Message}");
            Console.WriteLine();
            Console.WriteLine("Operasjonen ble avbrutt.");
        }

        private static void PopulateFanger(Fengsel fengsel, List<(string cellenummer, Fange fange)> fangedataResponse)
        {
            foreach (var fangedata in fangedataResponse)
            {
                var cellenummer = fangedata.cellenummer;
                var fange = fangedata.fange;

                if (fange.FengslingsDatoTil > DateTime.Now)
                {
                    fengsel.AddFange(fange, cellenummer);
                }
            }
        }

        private static void PrintWelcome()
        {
            Console.WriteLine("Velkommen til fengselsadministratoren!");
            Console.WriteLine();
            LongWait();
        }

        private static void PrintIntro(Fengsel fengsel)
        {
            Console.WriteLine($"Du er logget inn i systemet til {fengsel.Navn}");
            LongWait();
            Console.WriteLine(fengsel.Beskrivelse);
            Console.WriteLine();

            LongWait();
            Console.WriteLine("Hva ønsker du å gjøre? Her er en liste over mulige kommandoer:");
            LongWait();
            PrintKommandoer();
        }

        private static string GetNavn()
        {
            var verdi = Console.ReadLine();
            if (string.IsNullOrEmpty(verdi))
            {
                Console.WriteLine();
                Console.WriteLine("Oppgitt navn er ikke gyldig, vennligst oppgi navn på nytt:");
                return GetNavn();
            }

            return verdi;
        }

        private static Fange HentFange(Fengsel fengsel)
        {
            var verdi = Console.ReadLine();

            var fange = fengsel.GetFange(verdi);

            if (fange is null)
            {
                Console.WriteLine();
                Console.WriteLine("Oppgitt fange er ikke en person tilhørende dette fengselet, vennligst oppgi navn på nytt:");
                return HentFange(fengsel);
            }

            return fange;
        }

        private static int GetAlder()
        {
            var verdi = Console.ReadLine();
            var isValid = int.TryParse(verdi, out int alder);

            if (!isValid)
            {
                Console.WriteLine();
                Console.WriteLine("Oppgitt alder er ikke et gyldig tall, vennligst oppgi et tall:");
                return GetAlder();
            }

            return alder;
        }

        private static string GetKjonn()
        {
            var verdi = Console.ReadLine();
            if (string.IsNullOrEmpty(verdi) || !Kjonn.IsValid(verdi))
            {
                Console.WriteLine();
                Console.WriteLine($"Oppgitt verdi er ikke et gyldig kjønn, vennligst oppgi en av følgende verdier: {string.Join(", ", Kjonn.ListValues())}");
                return GetKjonn();
            }

            return verdi;
        }

        private static DateTime GetFengslingsDato()
        {
            var verdi = Console.ReadLine();

            var isValid = DateTime.TryParseExact(verdi, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

            if (!isValid)
            {
                Console.WriteLine();
                Console.WriteLine($"Oppgitt verdi er ikke en gyldig dato, vennligst prøv på nytt:");
                return GetFengslingsDato();
            }

            if (parsedDate < DateTime.Now)
            {
                Console.WriteLine();
                Console.WriteLine($"Oppgitt dato er bakover i tid, vennligst oppgi en dato som er i fremtiden:");
                return GetFengslingsDato();
            }

            return parsedDate;
        }

        private static Celle GetCellenummer(Fengsel fengsel)
        {
            var verdi = Console.ReadLine();

            var celle = fengsel.GetCelle(verdi);

            if (celle is null)
            {
                Console.WriteLine();
                Console.WriteLine($"Oppgitt cellenummer '{verdi}' er ikke en tilgjenglig celle. Vennligst prøv på nytt:");
                return GetCellenummer(fengsel);
            }

            return celle;
        }

        private static void ShortWait() => Thread.Sleep(1000);
        private static void LongWait() => Thread.Sleep(3000);

        private static void PrintKommandoer()
        {
            Console.WriteLine();
            Console.WriteLine($"Se celleoversikt: ----------------------- '{seCelleoverSikt}'");
            Console.WriteLine($"Se cellekapasitet for en enkelt celle: -- '{seCellekapasitet}'");
            Console.WriteLine($"Legg til ny fange: ---------------------- '{nyFange}'");
            Console.WriteLine($"Flytt fange mellom celler: -------------- '{flyttFange}'");
            Console.WriteLine($"Løslat fange: --------------------------- '{loslatFange}'");
            Console.WriteLine($"Avslutt: -------------------------------- '{avslutt}'");
            Console.WriteLine();
            Console.WriteLine("Kommando:");
        }
    }
}
