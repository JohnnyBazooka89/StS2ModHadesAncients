using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics.Types;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace HadesAncients.HadesAncientsCode.Hecate.Relics;

[Pool(typeof(EventRelicPool))]
public class Strength() : HadesAncientsRelic(HadesAncient.Hecate), IArcanaRelic
{
    private const string HpThresholdKey = "HpThreshold";
    private bool _strengthAndDexterityApplied;
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(HpThresholdKey, 50M),
        new PowerVar<StrengthPower>(2M),
        new PowerVar<DexterityPower>(1M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    private bool StrengthAndDexterityApplied
    {
        get => _strengthAndDexterityApplied;
        set
        {
            AssertMutable();
            _strengthAndDexterityApplied = value;
        }
    }

    public int GetArcanaRelicNumber()
    {
        return 23;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        await ModifyStrengthAndDexterityIfNecessary();
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        StrengthAndDexterityApplied = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, Decimal _)
    {
        if (!CombatManager.Instance.IsInProgress)
            return;
        await ModifyStrengthAndDexterityIfNecessary();
    }

    private async Task ModifyStrengthAndDexterityIfNecessary()
    {
        Creature creature = Owner.Creature;
        bool flag = creature.CurrentHp >
                    creature.MaxHp * (DynamicVars[HpThresholdKey].BaseValue / 100M);
        Status = flag ? RelicStatus.Normal : RelicStatus.Active;
        Decimal strengthBaseValue = DynamicVars.Strength.BaseValue;
        Decimal dexterityBaseValue = DynamicVars.Dexterity.BaseValue;
        if (flag && StrengthAndDexterityApplied)
        {
            Flash();
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), creature, -strengthBaseValue,
                creature, null);
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), creature, -dexterityBaseValue,
                creature, null);
            StrengthAndDexterityApplied = false;
        }
        else
        {
            if (flag || StrengthAndDexterityApplied)
                return;
            Flash();
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), creature, strengthBaseValue,
                creature, null);
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), creature, dexterityBaseValue,
                creature, null);
            StrengthAndDexterityApplied = true;
        }
    }
}