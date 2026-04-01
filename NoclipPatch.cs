using Assets.Scripts.Objects.Entities;
using UnityEngine;

namespace SpectatorCamMod
{
    /// <summary>
    /// Attached to a ghost player's gameObject. Disables all colliders and freezes
    /// the Rigidbody while the jetpack is actively ticking (signalled by
    /// JetpackInfiniteFuelPatch), re-enables everything after a short grace period
    /// once ticking stops.
    /// </summary>
    public class NoclipController : MonoBehaviour
    {
        private Human _human;
        private Collider[] _colliders;
        private bool[] _originalEnabledStates;
        private Rigidbody _rigidbody;
        private bool _originalUseGravity;
        private bool _originalIsKinematic;
        private bool _noclipActive;

        // OnAtmosphericTick may run slower than Update(), so use a generous window.
        private float _jetpackActiveUntil;
        private const float GracePeriod = 2f;

        public void Initialize(Human human)
        {
            _human = human;

            _colliders = human.gameObject.GetComponentsInChildren<Collider>(true);
            _originalEnabledStates = new bool[_colliders.Length];
            for (int i = 0; i < _colliders.Length; i++)
                _originalEnabledStates[i] = _colliders[i].enabled;

            _rigidbody = human.gameObject.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                _originalUseGravity = _rigidbody.useGravity;
                _originalIsKinematic = _rigidbody.isKinematic;
            }

            Plugin.Logger.LogInfo($"NoclipController initialised for {human.DisplayName} " +
                                  $"({_colliders.Length} collider(s), rigidbody={_rigidbody != null})");
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
            // Toggle colliders
            for (int i = 0; i < _colliders.Length; i++)
                _colliders[i].enabled = enabled ? false : _originalEnabledStates[i];

            // Freeze / unfreeze the Rigidbody so gravity doesn't pull the player
            // through the floor while colliders are disabled.
            if (_rigidbody != null)
            {
                if (enabled)
                {
                    _rigidbody.useGravity = false;
                    _rigidbody.isKinematic = true;
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }
                else
                {
                    _rigidbody.useGravity = _originalUseGravity;
                    _rigidbody.isKinematic = _originalIsKinematic;
                }
            }

            Plugin.Logger.LogInfo($"Noclip {(enabled ? "enabled" : "disabled")} for {_human?.DisplayName}");
        }

        /// <summary>Restore colliders/rigidbody and remove this component.</summary>
        public void Cleanup()
        {
            if (_colliders != null)
                ApplyNoclip(false);

            Destroy(this);
        }
    }
}
