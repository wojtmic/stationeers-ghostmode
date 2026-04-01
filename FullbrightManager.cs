using Assets.Scripts.Objects.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace SpectatorCamMod
{
    /// <summary>
    /// Added to the Plugin GameObject on every game instance (host and client).
    /// Watches Human.LocalHuman each frame: if the local player is in ghost mode
    /// it forces RenderSettings to max ambient light. Restores original settings
    /// as soon as the condition no longer holds.
    ///
    /// Detection uses two independent checks so it works on both host and client:
    ///   1. GhostManager.IsGhosted — authoritative on the host (command runs there).
    ///   2. Layer == "Ignore Raycast" — detects synced state on connected clients.
    /// </summary>
    public class ClientFullbrightWatcher : MonoBehaviour
    {
        private static readonly int IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

        private Color _savedAmbientLight;
        private float _savedAmbientIntensity;
        private AmbientMode _savedAmbientMode;
        private bool _fullbrightApplied;

        private void Update()
        {
            var local = Human.LocalHuman;
            bool isGhosted = local != null &&
                             (GhostManager.IsGhosted(local.OwnerClientId) ||
                              local.gameObject.layer == IgnoreRaycastLayer);
            bool shouldApply = isGhosted && GhostManager.FullbrightEnabled;

            if (shouldApply && !_fullbrightApplied)
            {
                // Snapshot natural settings the moment we transition on, so we
                // restore correctly even if the game changes them over time.
                _savedAmbientLight = RenderSettings.ambientLight;
                _savedAmbientIntensity = RenderSettings.ambientIntensity;
                _savedAmbientMode = RenderSettings.ambientMode;
                _fullbrightApplied = true;
                Plugin.Logger.LogInfo("Fullbright on (local player is ghost)");
            }
            else if (!shouldApply && _fullbrightApplied)
            {
                RenderSettings.ambientMode = _savedAmbientMode;
                RenderSettings.ambientLight = _savedAmbientLight;
                RenderSettings.ambientIntensity = _savedAmbientIntensity;
                _fullbrightApplied = false;
                Plugin.Logger.LogInfo("Fullbright off");
                return;
            }

            if (_fullbrightApplied)
            {
                RenderSettings.ambientMode = AmbientMode.Flat;
                RenderSettings.ambientLight = Color.white;
                RenderSettings.ambientIntensity = 8f;
            }
        }
    }
}
