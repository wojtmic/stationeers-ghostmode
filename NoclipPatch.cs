using Assets.Scripts.Objects.Entities;
using UnityEngine;

namespace SpectatorCamMod
{
    /// <summary>
    /// Attached to a ghost player's gameObject. Disables all colliders while the
    /// jetpack is actively ticking (signalled by JetpackInfiniteFuelPatch) and
    /// re-enables them after a short grace period once ticking stops.
    /// </summary>
    public class NoclipController : MonoBehaviour
    {
        private Human _human;
        private Collider[] _colliders;
        private bool[] _originalEnabledStates;
        private bool _noclipActive;

        // Time.time value after which we consider the jetpack no longer active.
        // Extended to 2 s because OnAtmosphericTick may run slower than Update().
        private float _jetpackActiveUntil;
        private const float GracePeriod = 2f;

        public void Initialize(Human human)
        {
            _human = human;
            _colliders = human.gameObject.GetComponentsInChildren<Collider>(true);
            _originalEnabledStates = new bool[_colliders.Length];
            for (int i = 0; i < _colliders.Length; i++)
                _originalEnabledStates[i] = _colliders[i].enabled;

            Plugin.Logger.LogInfo($"NoclipController initialised for {human.DisplayName} with {_colliders.Length} collider(s)");
        }

        /// <summary>
        /// Called by JetpackInfiniteFuelPatch each time OnAtmosphericTick fires for
        /// this ghost player, keeping the noclip window open.
        /// </summary>
        public void SignalJetpackActive()
        {
            _jetpackActiveUntil = Time.time + GracePeriod;
        }

        private void Update()
        {
            if (_human == null || _colliders == null) return;

            bool shouldNoclip = Time.time < _jetpackActiveUntil;
            if (shouldNoclip == _noclipActive) return;

            _noclipActive = shouldNoclip;
            ApplyNoclip(_noclipActive);
        }

        private void ApplyNoclip(bool enabled)
        {
            for (int i = 0; i < _colliders.Length; i++)
                _colliders[i].enabled = enabled ? false : _originalEnabledStates[i];

            Plugin.Logger.LogInfo($"Noclip {(enabled ? "enabled" : "disabled")} for {_human?.DisplayName}");
        }

        /// <summary>Restore colliders and remove this component.</summary>
        public void Cleanup()
        {
            if (_colliders != null)
                ApplyNoclip(false);

            Destroy(this);
        }
    }
}
