using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Runs;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Parsing;

namespace HadesAncients.HadesAncientsCode.Shared.Formatters;

public class GenderedFormatter : IFormatter
{
    public string Name
    {
        get => "gendered";
        set => throw new NotSupportedException("Setting the 'Names' property is not supported.");
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        IList<Format> formatList = formattingInfo.Format?.Split('|') ?? [];
        if (formatList is not { Count: 3 })
        {
            throw new LocException(
                $"Format expression must contain 3 options. num_of_options={formatList.Count} format={formattingInfo.Format}.");
        }

        Format maleFormat = formatList[0];
        Format femaleFormat = formatList[1];
        Format neutralFormat = formatList[2];

        Player? player = LocalContext.GetMe(RunManager.Instance?.DebugOnlyGetState());

        if (player == null)
        {
            formattingInfo.FormatAsChild(neutralFormat, formattingInfo.CurrentValue);
            return true;
        }

        switch (player.Character.Gender)
        {
            case CharacterGender.Masculine:
                formattingInfo.FormatAsChild(maleFormat, formattingInfo.CurrentValue);
                break;
            case CharacterGender.Feminine:
                formattingInfo.FormatAsChild(femaleFormat, formattingInfo.CurrentValue);
                break;
            case CharacterGender.Neutral:
                formattingInfo.FormatAsChild(neutralFormat, formattingInfo.CurrentValue);
                break;
        }

        return true;
    }
}