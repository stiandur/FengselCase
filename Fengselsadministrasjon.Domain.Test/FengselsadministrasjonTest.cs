using Fengselsadministrasjon.Domain.Entities;
using Fengselsadministrasjon.Domain.Shared;
using Shouldly;

namespace Fengselsadministrasjon.Domain.Test
{
    public class Tests
    {
        private Fengsel fengsel;

        [SetUp]
        public void Setup()
        {
            fengsel = new Fengsel("Oslo Fengsel", "Oslo fengsel ligger på Grønland og er er oslos største fengsel.");
            fengsel.GenererCeller(7, 2, 2);
        }

        [Test]
        public void SkalKasteException_NårManProverALeggeTilFangeICelleSomIkkeEksisterer()
        {
            // Setup
            var fange = StandardFange();

            // Run
            void action() => fengsel.AddFange(fange, "801");

            // Assert
            Assert.Throws<ArgumentException>(action, "Fant ingen celle med cellenummer 801");
        }

        [Test]
        public void SkalKasteException_NårManProverALeggeTilFangeISammeCelleToGanger()
        {
            // Setup
            var fange = StandardFange();

            // Run
            fengsel.AddFange(fange, "701");
            void action() => fengsel.AddFange(fange, "701");

            // Assert
            Assert.Throws<ArgumentException>(action, "Fange 'Test Testesen' er allerede plassert i denne cellen");
        }

        [Test]
        public void SkalKasteException_NårManProverALeggeTilFangeIEnCelleSomErFull()
        {
            // Setup
            var jens = StandardFange("Jens Pikenes");
            var ola = StandardFange("Ola Normann");
            var kari = StandardFange("Kari Normann", Kjonn.Kvinne);

            // Run
            fengsel.AddFange(jens, "701");
            fengsel.AddFange(ola, "701");
            void action() => fengsel.AddFange(kari, "701");

            // Assert
            Assert.Throws<ArgumentException>(action, "Celle 701 har ikke plass til flere fanger");
        }

        [Test]
        public void SkalKasteException_NårManProverALeggeTilFangeIEnCelleSomInneholderAnnenFangeAvMotsattKjonn()
        {
            // Setup
            var jens = StandardFange("Jens Pikenes");
            var kari = StandardFange("Kari Normann", Kjonn.Kvinne);

            // Run
            fengsel.AddFange(jens, "701");
            void action() => fengsel.AddFange(kari, "701");

            // Assert
            Assert.Throws<ArgumentException>(action, "Fange 'Kari Normann' kan ikke plasseres i denne cellen da den allerede har en fange av motsatt kjønn");
        }

        [Test]
        public void TestAlt()
        {
            // Setup
            var jens = StandardFange("Jens Pikenes");
            var ola = StandardFange("Ola Normann");
            var harald = StandardFange("Harald Rex");
            var olav = StandardFange("Olav Hellig");

            var kari = StandardFange("Kari Normann", Kjonn.Kvinne);
            var berit = StandardFange("Berit Bergen", Kjonn.Kvinne);
            var trine = StandardFange("Trine Trondheim", Kjonn.Kvinne);
            var laila = StandardFange("Laila Lillestrøm", Kjonn.Kvinne);

            fengsel.AddFange(jens, "701");
            fengsel.AddFange(ola, "101");
            fengsel.AddFange(harald, "101");
            fengsel.AddFange(olav, "202");

            fengsel.AddFange(kari, "401");
            fengsel.AddFange(berit, "501");
            fengsel.AddFange(trine, "601");
            fengsel.AddFange(laila, "601");

            // Assert
            var alleFanger = fengsel.GetAlleFanger();
            alleFanger.Count.ShouldBe(8);
            alleFanger.ShouldContain(x => x.Id == jens.Id);

            fengsel.Celler.Count.ShouldBe(14);

            var tilgjengeligeCellerMenn = fengsel.GetTilgjengeligeCeller(Kjonn.Mann);
            tilgjengeligeCellerMenn.Count.ShouldBe(10);
            tilgjengeligeCellerMenn.ShouldNotContain(x => x.Cellenummer == "101");
            tilgjengeligeCellerMenn.ShouldNotContain(x => x.Cellenummer == "401");
            tilgjengeligeCellerMenn.ShouldContain(x => x.Cellenummer == "701");
            tilgjengeligeCellerMenn.ShouldContain(x => x.Cellenummer == "202");

            var tilgjengeligeCellerKvinner = fengsel.GetTilgjengeligeCeller(Kjonn.Kvinne);
            tilgjengeligeCellerKvinner.Count.ShouldBe(10);
            tilgjengeligeCellerKvinner.ShouldNotContain(x => x.Cellenummer == "601");
            tilgjengeligeCellerKvinner.ShouldNotContain(x => x.Cellenummer == "701");
            tilgjengeligeCellerKvinner.ShouldContain(x => x.Cellenummer == "501");
            tilgjengeligeCellerKvinner.ShouldContain(x => x.Cellenummer == "401");

            fengsel.LoslatFange(ola.Id);
            alleFanger = fengsel.GetAlleFanger();
            alleFanger.Count.ShouldBe(7);
            alleFanger.ShouldNotContain(x => x.Id == ola.Id);

            tilgjengeligeCellerMenn = fengsel.GetTilgjengeligeCeller(Kjonn.Mann);
            tilgjengeligeCellerMenn.Count.ShouldBe(11);

            fengsel.FlyttFange(jens.Id, "101");

            var celle101 = fengsel.GetCelle("101");
            celle101.HarFange(jens.Id).ShouldBeTrue();

            var celle701 = fengsel.GetCelle("701");
            celle701.HarFange(jens.Id).ShouldBeFalse();
        }

        private static Fange StandardFange(string navn = "Test Testesen", Kjonn? kjonn = null) => new(navn,
                                                       42,
                                                       kjonn ?? Kjonn.Mann,
                                                       fengslingsDatoFra: DateTime.Now,
                                                       fengslingsDatoTil: DateTime.Now.AddYears(2));
    }
}