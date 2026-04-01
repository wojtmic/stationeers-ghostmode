using UnityEngine;
using UnityEngine.Rendering;

namespace SpectatorCamMod
{
    /// <summary>
    /// Attached to a ghost player's gameObject. Overrides RenderSettings every
    /// Update() frame to keep ambient light at maximum so the ghost player sees a
    /// fully-lit world. Original settings are restored on Cleanup().
    ///
    /// Note: because this runs on the host, it only affects the host's view.
    /// If the ghosted player is not the host, a client-side version of this mod
    /// would be required for the fullbright to take effect on their screen.
    /// </summary>
    public class FullbrightController : MonoBehaviour
    {
        private Color _savedAmbientLight;
        private float _savedAmbientIntensity;
        private AmbientMode _savedAmbientMode;

        private void Awake()
        {
            _savedAmbientLight = RenderSettings.ambientLight;
            _savedAmbientIntensity = RenderSettings.ambientIntensity;
            _savedAmbientMode = RenderSettings.ambientMode;

            Plugin.Logger.LogInfo("Fullbright activated: ambient settings saved");
        }

        private void Update()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.white;
            RenderSettings.ambientIntensity = 8f;
        }

        /// <summary>Restore original ambient settings and remove this component.</summary>
        public void Cleanup()
        {
            RenderSettings.ambientMode = _savedAmbientMode;
            RenderSettings.ambientLight = _savedAmbientLight;
            RenderSettings.ambientIntensity = _savedAmbientIntensity;

            Plugin.Logger.LogInfo("Fullbright deactivated: ambient settings restored");
            Destroy(this);
        }
    }
}
