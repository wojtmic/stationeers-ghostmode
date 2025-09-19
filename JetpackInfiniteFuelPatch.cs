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
                Plugin.Logger.LogInfo($"Blocking fuel consumption for ghost player {__instance.ParentHuman.DisplayName}");
                return false; // Skip the original fuel consumption method
            }
            
            return true; // Allow normal fuel consumption for non-ghost players
        }
    }
}