using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HadesAncients.HadesAncientsCode.Hecate.Relics;
using HadesAncients.HadesAncientsCode.Shared.Enums;
using HadesAncients.HadesAncientsCode.Shared.Extensions;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;

namespace HadesAncients.HadesAncientsCode.Hecate.Ancients;

[Pool(typeof(AncientEventModel))]
public class HecateAncient : CustomAncientModel
{
    public override string CustomScenePath => "hecate.tscn".AncientImagePath(HadesAncient.Hecate);
    public override string CustomMapIconPath => "map_icon.png".AncientImagePath(HadesAncient.Hecate);
    public override string CustomMapIconOutlinePath => "map_icon_outline.png".AncientImagePath(HadesAncient.Hecate);
    public override string CustomRunHistoryIconPath => "run_history_icon.png".AncientImagePath(HadesAncient.Hecate);
    public override string CustomRunHistoryIconOutlinePath =>
        "run_history_icon_outline.png".AncientImagePath(HadesAncient.Hecate);

    protected override OptionPools MakeOptionPools
    {
        get
        {
            var relics = GetAllHecateAncientOptions();

            return new OptionPools(
                MakePool(relics.ToArray())
            );
        }
    }

    public static List<AncientOption> GetAllHecateAncientOptions()
    {
        return
        [
            AncientOption<Death>(),
            AncientOption<Divinity>(),
            AncientOption<Eternity>(),
            AncientOption<Excellence>(),
            AncientOption<Judgement>(),
            AncientOption<Night>(),
            AncientOption<Origination>(),
            AncientOption<Persistence>(),
            AncientOption<Strength>(),
            AncientOption<TheArtificer>(),
            AncientOption<TheBoatman>(),
            AncientOption<TheCentaur>(),
            AncientOption<TheChampions>(),
            AncientOption<TheEnchantress>(),
            AncientOption<TheFates>(),
            AncientOption<TheFuries>(),
            AncientOption<TheHuntress>(),
            AncientOption<TheLovers>(),
            AncientOption<TheMessenger>(),
            AncientOption<TheMoon>(),
            AncientOption<TheQueen>(),
            AncientOption<TheSorceress>(),
            AncientOption<TheSwiftRunner>(),
            AncientOption<TheUnseen>(),
            AncientOption<TheWaywardSon>()
        ];
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 1 && !HadesAncientsModConfig.DisableHecate;
    }

    protected override AncientDialogueSet DefineDialogues()
    {
        AncientDialogueSet baseSet = base.DefineDialogues();

        string prefix = $"{Id.Entry}.talk.firstVisitEver.0";

        AncientDialogue firstVisit = new(
            AncientDialogueUtil.SfxPath($"{prefix}-0.ancient"),
            AncientDialogueUtil.SfxPath($"{prefix}-1.char"),
            AncientDialogueUtil.SfxPath($"{prefix}-2.ancient"),
            AncientDialogueUtil.SfxPath($"{prefix}-3.char")
        );

        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = firstVisit,
            CharacterDialogues = baseSet.CharacterDialogues,
            AgnosticDialogues = baseSet.AgnosticDialogues
        };
    }
}