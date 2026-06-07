using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

namespace HadesAncients.HadesAncientsCode.Poseidon.SpireFields;

public class PoseidonSpireFields
{
    public static readonly SpireField<CardReward, List<CardModel>> SeaStarOriginalCards = new(() => []);
}