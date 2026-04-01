using UnityEngine;
using UnityEngine.Rendering;

namespace SpectatorCamMod
{
    /// <summary>
    /// Attached to a ghost player's gameObject. Overrides RenderSettings every
    /// Update() frame to keep ambient light at maximum. Can be toggled at runtime
    /// via SetActive(); original settings are restored when inactive or on Cleanup().
    ///
    /// Note: runs on the host, so fullbright only affects the host's view.
    /// </summary>
    public class FullbrightController : MonoBehaviour
    {
        private Color _savedAmbientLight;
        private float _savedAmbientIntensity;
        private AmbientMode _savedAmbientMode;
        private bool _active = true;

        private void Awake()
        {
            SaveSettings();
            Plugin.Logger.LogInfo("Fullbright activated: ambient settings saved");
        }

        private void Update()
        {
            if (!_active) return;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.white;
            RenderSettings.ambientIntensity = 8f;
        }

        /// <summary>Toggle fullbright on or off without removing the component.</summary>
        public void SetActive(bool active)
        {
            if (_active == active) return;
            _active = active;

            if (!_active)
            {
                RestoreSettings();
                Plugin.Logger.LogInfo("Fullbright toggled off");
            }
            else
            {
                // Re-snapshot current settings before taking over again.
                SaveSettings();
                Plugin.Logger.LogInfo("Fullbright toggled on");
            }
        }

        /// <summary>Restore original ambient settings and remove this component.</summary>
        public void Cleanup()
        {
            RestoreSettings();
            Plugin.Logger.LogInfo("Fullbright deactivated: ambient settings restored");
            Destroy(this);
        }

        private void SaveSettings()
        {
            _savedAmbientLight = RenderSettings.ambientLight;
            _savedAmbientIntensity = RenderSettings.ambientIntensity;
            _savedAmbientMode = RenderSettings.ambientMode;
        }

        private void RestoreSettings()
        {
            RenderSettings.ambientMode = _savedAmbientMode;
            RenderSettings.ambientLight = _savedAmbientLight;
            RenderSettings.ambientIntensity = _savedAmbientIntensity;
        }
    }
}
