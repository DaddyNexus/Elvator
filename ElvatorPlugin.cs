using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Rocket.Unturned.Events.UnturnedPlayerEvents;
using Logger = Rocket.Core.Logging.Logger;

namespace ElevatorPlugin
{
    public class ElevatorPlugin : RocketPlugin<ElevatorPluginConfiguration>
    {
        public static ElevatorPlugin Instance { get; private set; }
        private Dictionary<CSteamID, Elevator> PlayerElevatorContext;
        private Dictionary<CSteamID, Elevator> PlayerInZone;
        private Dictionary<CSteamID, DateTime> PunchCooldowns;

        protected override void Load()
        {
            Instance = this;
            PlayerElevatorContext = new Dictionary<CSteamID, Elevator>();
            PlayerInZone = new Dictionary<CSteamID, Elevator>();
            PunchCooldowns = new Dictionary<CSteamID, DateTime>();
            Logger.Log("Elevator Plugin loaded successfully!");
            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnPlayerUpdateGesture;
            Provider.onServerDisconnected += OnPlayerDisconnected;
        }

        protected override void Unload()
        {
            EffectManager.onEffectButtonClicked -= OnEffectButtonClicked;
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= OnPlayerUpdateGesture;
            Provider.onServerDisconnected -= OnPlayerDisconnected;
            PlayerElevatorContext.Clear();
            PlayerInZone.Clear();
            PunchCooldowns.Clear();
            Logger.Log("Elevator Plugin unloaded.");
            Instance = null;
        }

        private void FixedUpdate()
        {
            if (Provider.clients.Count == 0 || Configuration.Instance.Elevators == null) return;
            var zoneElevators = Configuration.Instance.Elevators.Where(e => e.UseZoneTrigger).ToList();
            if (zoneElevators.Count == 0) return;

            foreach (var steamPlayer in Provider.clients)
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                if (player == null) continue;
                Elevator elevatorPlayerIsIn = null;

                foreach (var elevator in zoneElevators)
                {
                    if (Vector3.Distance(player.Position, elevator.Position.ToVector3()) <= elevator.Radius)
                    {
                        elevatorPlayerIsIn = elevator;
                        break;
                    }
                }

                PlayerInZone.TryGetValue(player.CSteamID, out var previouslyTrackedZone);

                if (elevatorPlayerIsIn != null && previouslyTrackedZone != elevatorPlayerIsIn)
                {
                    PlayerInZone[player.CSteamID] = elevatorPlayerIsIn;
                    OpenElevatorUI(player, elevatorPlayerIsIn);
                }
                else if (elevatorPlayerIsIn == null && previouslyTrackedZone != null)
                {
                    PlayerInZone.Remove(player.CSteamID);
                    if (PlayerElevatorContext.TryGetValue(player.CSteamID, out var openUI) && openUI == previouslyTrackedZone)
                    {
                        CloseElevatorUI(player);
                    }
                }
            }
        }

        private void OnPlayerDisconnected(CSteamID steamID)
        {
            PlayerElevatorContext.Remove(steamID);
            PlayerInZone.Remove(steamID);
            PunchCooldowns.Remove(steamID);
        }

        private void OnPlayerUpdateGesture(UnturnedPlayer player, PlayerGesture gesture)
        {
            if (gesture != PlayerGesture.PunchLeft && gesture != PlayerGesture.PunchRight)
                return;

            if (player == null) return;

            // Cooldown Check
            if (PunchCooldowns.ContainsKey(player.CSteamID) && (DateTime.Now - PunchCooldowns[player.CSteamID]).TotalSeconds < 2)
            {
                return;
            }

            // Raycast for both barricades and structures
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, 3f, RayMasks.BARRICADE | RayMasks.STRUCTURE))
            {
                ushort itemID = 0;

                // Try to identify the punched object's ID
                BarricadeDrop barricadeDrop = BarricadeManager.FindBarricadeByRootTransform(hit.transform);
                if (barricadeDrop != null)
                {
                    itemID = barricadeDrop.asset.id;
                }
                else
                {
                    StructureDrop structureDrop = StructureManager.FindStructureByRootTransform(hit.transform);
                    if (structureDrop != null)
                    {
                        itemID = structureDrop.asset.id;
                    }
                }

                // If we successfully got an item ID
                if (itemID != 0)
                {
                    // Find the FIRST elevator that uses this item ID as a punch trigger.
                    var targetElevator = Configuration.Instance.Elevators
                        .FirstOrDefault(e => !e.UseZoneTrigger && e.TriggerItemID == itemID);

                    // If we found a matching elevator in the config
                    if (targetElevator != null)
                    {
                        Logger.Log($"{player.DisplayName} punched elevator '{targetElevator.Name}'!");
                        OpenElevatorUI(player, targetElevator);
                        PunchCooldowns[player.CSteamID] = DateTime.Now;
                    }
                }
            }
        }


        private void OnEffectButtonClicked(Player player, string buttonName)
        {
            UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromPlayer(player);
            if (unturnedPlayer == null) return;

            if (!PlayerElevatorContext.TryGetValue(unturnedPlayer.CSteamID, out Elevator elevator))
            {
                return;
            }

            if (buttonName.Equals(Configuration.Instance.CloseButtonName, StringComparison.OrdinalIgnoreCase))
            {
                CloseElevatorUI(unturnedPlayer);
                return;
            }

            var floor = elevator.Floors.FirstOrDefault(f => f.ButtonName.Equals(buttonName, StringComparison.OrdinalIgnoreCase));

            if (floor != null)
            {
                var destination = floor.Destination.ToVector3();
                unturnedPlayer.Teleport(destination, unturnedPlayer.Rotation);
                CloseElevatorUI(unturnedPlayer);
                Logger.Log($"Teleported {unturnedPlayer.DisplayName} via '{elevator.Name}' to floor '{floor.DisplayName}'.");
            }
        }

        public void OpenElevatorUI(UnturnedPlayer player, Elevator elevator)
        {
            PlayerElevatorContext[player.CSteamID] = elevator;
            short uiKey = (short)Configuration.Instance.UIEffectID;
            EffectManager.sendUIEffect(Configuration.Instance.UIEffectID, uiKey, player.CSteamID, true);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

            for (int i = 1; i <= 20; i++)
            {
                EffectManager.sendUIEffectVisibility(uiKey, player.CSteamID, true, $"Floor_{i}", false);
            }

            if (elevator.Floors == null)
            {
                Logger.LogError($"Elevator '{elevator.Name}' has no floors configured!");
                return;
            }

            for (int i = 0; i < elevator.Floors.Count; i++)
            {
                var floor = elevator.Floors[i];
                string buttonName = floor.ButtonName;
                string floorDisplayName = floor.DisplayName;
                string textObjectName = $"FloorText_{i + 1}";
                EffectManager.sendUIEffectVisibility(uiKey, player.CSteamID, true, buttonName, true);
                EffectManager.sendUIEffectText(uiKey, player.CSteamID, true, textObjectName, floorDisplayName);
            }
        }

        public void CloseElevatorUI(UnturnedPlayer player)
        {
            EffectManager.askEffectClearByID(Configuration.Instance.UIEffectID, player.CSteamID);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            PlayerElevatorContext.Remove(player.CSteamID);
        }
    }
}