using Assets.Scripts.Objects.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace SpectatorCamMod;

public static class GhostManager
{
    private static HashSet<ulong> _ghostedPlayers = new HashSet<ulong>();

    public static bool TogglePlayer(ulong clientId)
    {
        if (_ghostedPlayers.Contains(clientId))
        {
            _ghostedPlayers.Remove(clientId);
            SetPlayerGodMode(clientId, false);
            Plugin.Logger.LogInfo($"Player {clientId} is no longer a ghost");
            return false;
        }
        else
        {
            _ghostedPlayers.Add(clientId);
            SetPlayerGodMode(clientId, true);
            Plugin.Logger.LogInfo($"Player {clientId} is now a ghost");
            return true;
        }
    }

    public static bool IsGhosted(ulong clientId)
    {
        return _ghostedPlayers.Contains(clientId);
    }

    private static void SetPlayerGodMode(ulong clientId, bool godMode)
    {
        var human = Human.Find(clientId);
        if (human == null)
        {
            Plugin.Logger.LogWarning($"Could not find human for client {clientId}");
            return;
        }

        // Existing god mode code...
        human.Indestructable = godMode;
        human.UnlimitedGas = godMode;
    
        if (human.OrganLungs != null)
        {
            human.OrganLungs.Indestructable = godMode;
        }
    
        foreach (var slot in human.Slots)
        {
            if (slot.Occupant != null)
            {
                slot.Occupant.Indestructable = godMode;
            }
        }
        
        var renderers = human.gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            renderer.enabled = !godMode;
        }

        // NEW: Try layer-based invisibility
        if (godMode)
        {
            // Move to an invisible/ignore layer
            human.gameObject.layer = UnityEngine.LayerMask.NameToLayer("Ignore Raycast");
        
            // Also try disabling renderers directly
            if (human.SkinnedMeshes != null)
            {
                foreach (var skinnedMesh in human.SkinnedMeshes)
                {
                    if (skinnedMesh.Renderer != null)
                    {
                        skinnedMesh.Renderer.enabled = false;
                    }
                }
            }
        }
        else
        {
            // Restore visibility
            human.gameObject.layer = Human.LayerPlayer;
        
            if (human.SkinnedMeshes != null)
            {
                foreach (var skinnedMesh in human.SkinnedMeshes)
                {
                    if (skinnedMesh.Renderer != null)
                    {
                        skinnedMesh.Renderer.enabled = true;
                    }
                }
            }
        }

        Plugin.Logger.LogInfo($"Ghost mode {godMode} applied successfully to {human.DisplayName}");
    }
}