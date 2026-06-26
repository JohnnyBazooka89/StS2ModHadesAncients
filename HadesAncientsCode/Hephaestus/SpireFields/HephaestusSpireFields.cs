using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Hephaestus.SpireFields;

public class HephaestusSpireFields
{
    public static readonly SpireField<CardModel, Boolean> UncannyFortitudeUpgradedOrEnchantedCards = new(() => false);
}