using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace HadesAncients.HadesAncientsCode.Shared.Hooks;

public interface IAfterArtifactPowerModifiedPowerAmountReceived
{
    Task AfterArtifactPowerModifiedPowerAmountReceived(
        ArtifactPower artifactPower,
        PowerModel blockedPower
    );
}