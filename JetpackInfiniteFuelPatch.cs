using Assets.Scripts.Objects.Items;
using HarmonyLib;

namespace SpectatorCamMod
{
    [HarmonyPatch(typeof(Jetpack), "OnAtmosphericTick")]
    public class JetpackInfiniteFuelPatch
    {
        [HarmonyPrefix]
        public static bool PreventFuelConsumption(Jetpack __instance)
        {
            // Check if this jetpack belongs to a ghost player
            if (__instance.ParentHuman != null &&
                GhostManager.IsGhosted(__instance.ParentHuman.OwnerClientId))
            {
                // Signal NoclipController that the jetpack is actively ticking so it
                // can enable/extend the noclip window for this player.
                __instance.ParentHuman.gameObject
                    .GetComponent<NoclipController>()
                    ?.SignalJetpackActive();

                return false; // Skip the original fuel consumption method
            }

            return true; // Allow normal fuel consumption for non-ghost players
        }
    }
}