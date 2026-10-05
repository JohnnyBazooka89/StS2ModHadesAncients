using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace HadesAncients.HadesAncientsCode.Shared.Patches;

[HarmonyPatch]
public static class HadesAncients_CombatManager_IsEnding_Patch
{
    // Counter instead of bool so nested usages are safe.
    private static readonly AsyncLocal<int> _bypassCount = new();

    private static bool IsBypassed => _bypassCount.Value > 0;

    public static IDisposable Bypass()
    {
        _bypassCount.Value++;
        return new BypassScope();
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(CombatManager),
        nameof(CombatManager.IsEnding),
        MethodType.Getter
    )]
    private static bool IsEndingPrefix(ref bool __result)
    {
        if (!IsBypassed)
            return true; // Run original getter.

        __result = false;
        return false; // Skip original getter.
    }

    private sealed class BypassScope : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _bypassCount.Value--;
        }
    }
}