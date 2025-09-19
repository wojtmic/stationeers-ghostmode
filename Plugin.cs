using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Util.Commands;
using Assets.Scripts.Objects; // Add this
using System.Reflection;

namespace SpectatorCamMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo("Plugin awakening...");
        
        // Debug: Check if EntityDamageState exists
        try 
        {
            var entityDamageStateType = typeof(EntityDamageState);
            Logger.LogInfo($"Found EntityDamageState type: {entityDamageStateType}");
            
            var methods = entityDamageStateType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Logger.LogInfo($"EntityDamageState has {methods.Length} methods");
            
            foreach (var method in methods)
            {
                if (method.Name.Contains("Damage"))
                {
                    Logger.LogInfo($"Found method: {method.Name} - {method}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"Error inspecting EntityDamageState: {ex}");
        }
        
        try
        {
            var harmony = new Harmony("dev.wojtmic.spectatorcammod");
            Logger.LogInfo("Harmony instance created");
            
            harmony.PatchAll();
            Logger.LogInfo("PatchAll called");
            
            // Check what got patched
            var patchedMethods = harmony.GetPatchedMethods();
            Logger.LogInfo($"Patched {patchedMethods.Count()} methods total");
            
            foreach (var method in patchedMethods)
            {
                Logger.LogInfo($"Patched method: {method.DeclaringType?.Name}.{method.Name}");
            }
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"Harmony patching failed: {ex}");
        }
                
        CommandLine.AddCommand("ghost", new GhostCommand());
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}