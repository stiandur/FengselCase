using Fengselsadministrasjon.Domain.Entities;
using Fengselsadministrasjon.Domain.Shared;
using Fengselsadministrasjon.Infrastructure.DTOs;
using System.Globalization;

namespace Fengselsadministrasjon.Infrastructure.Mappers;

internal static class FangeMapper
{
    internal static Fange Map(FangeDto fange) =>
        new(navn: fange.Navn,
            alder: fange.Alder,
            kjonn: Kjonn.FromString(fange.Kjonn),
            fengslingsDatoFra: DateMapper(fange.FengslingsDatoFra),
            fengslingsDatoTil: DateMapper(fange.FengslingsDatoTil)
        );

    internal static DateTime DateMapper(string dateTimeValue)
    {
        if (DateTime.TryParseExact(dateTimeValue, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            return dateTime;
        }

        throw new FormatException($"Parsing av følgende verdi til DateTime feilet: {dateTimeValue}");
    }
}
