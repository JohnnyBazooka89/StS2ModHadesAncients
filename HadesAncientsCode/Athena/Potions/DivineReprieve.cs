using BaseLib.Utils;
using Godot;
using HadesAncients.HadesAncientsCode.Athena.Powers;
using HadesAncients.HadesAncientsCode.Shared.Abstracts;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace HadesAncients.HadesAncientsCode.Athena.Potions;

[Pool(typeof(SharedPotionPool))]
public class DivineReprieve() : HadesAncientsPotion(HadesAncient.Athena)
{
    public override PotionRarity Rarity => PotionRarity.Event;
    public override PotionUsage Usage => PotionUsage.AnyTime;
    public override TargetType TargetType => TargetType.AnyPlayer;

    public override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(3M),
        new PowerVar<DexterityPower>(3M)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
    ];

    public override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        NCombatRoom.Instance?.PlaySplashVfx(target, Colors.Red);
        await CreatureCmd.Heal(target, target.MaxHp);
        if (!CombatManager.Instance.IsInProgress)
            return;
        await PowerCmd.Apply<DivineReprievePower>(choiceContext, target, 1M, Owner.Creature, null);
        await PowerCmd.Apply<StrengthPower>(choiceContext, target, DynamicVars[nameof(StrengthPower)].BaseValue,
            Owner.Creature, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, target, DynamicVars[nameof(DexterityPower)].BaseValue,
            Owner.Creature, null);
    }
}