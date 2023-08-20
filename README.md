## Innledning
Ved gjennomførelse av denne casen har jeg skrevet en console app i Dotnet 6. Jeg har gått inn for å løse det på en måte som ligner noe på hvordan man ville gjort det i et ordentlig prosjekt mtp. oppdeling av prosjekter osv.

## Antakelser
Jeg har gjort noen antakelser:
- Hver celle har maks plass til to fanger.
- Hvert nummer/etasje har to celler.
- Kvinner og menn kan ikke dele celle.
- Fanger fra API med til-dato som er i fortiden blir ikke lagt til.
- Ved løslatelse av fangene fjernes de helt fra systemet.

## Annet
- Brukernavn og passord til API er per nå satt i appsettings.json. Vanligvis ville jeg satt dette i octopus.
- For å ivareta domenebegreper osv. har jeg kodet delvis på norsk.
- Det er ikke skrevet tester for alt da det virket noe unødvendig for en case, men det er lagt inn eksempler på tester som demonstrerer hvordan jeg ville gjort det, samt er det samlet veldig mye verifisering i én og samme test for å spare arbeid. Vanligvis ville jeg delt denne "TestAlt"-metoden opp i mer spesifikke tester der man tester enkelt-features.

## Hvordan kjøre applikasjonen

#### Alternativ 1:
Åpne Fengsel.sln i Visual Studio, sett Fengselsadministrasjon.ConsoleApp som startup project og kjør applikasjonen.

#### Alternativ 2:
Åpne en terminal og gå til root av koden der Fengsel.sln ligger og kjør `dotnet publish -c Release -r win10-x64`.

Åpne en file explorer og gå til `Fengselsadministrasjon.ConsoleApp\bin\Release\net6.0\win10-x64` og kjør "Fengselsadministrasjon.ConsoleApp.exe".
