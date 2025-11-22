using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElevatorPlugin.Commands
{
    public class CommandElevator : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "elevator";
        public string Help => "Opens the UI for the nearest elevator.";
        public string Syntax => "/elevator";
        public List<string> Aliases => new List<string> { "ele" };
        public List<string> Permissions => new List<string> { "elevator.command" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;
            var config = ElevatorPlugin.Instance?.Configuration?.Instance;

            // Check if the plugin or its configuration is loaded and if there are any elevators
            if (config?.Elevators == null || !config.Elevators.Any())
            {
                UnturnedChat.Say(player, "No elevators are configured on this server.", Color.red);
                return;
            }

            Elevator closestElevator = null;
            float closestDistance = float.MaxValue;

            // Loop through all elevators to find the one closest to the player
            foreach (var elevator in config.Elevators)
            {
                float distance = Vector3.Distance(player.Position, elevator.Position.ToVector3());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestElevator = elevator;
                }
            }

            // Define the maximum distance a player can be from an elevator to use the command
            const float maxActivationDistance = 10f;

            // Check if an elevator was found and if it's within the allowed range
            if (closestElevator != null && closestDistance <= maxActivationDistance)
            {
                // Success: Open the UI for the nearby elevator
                ElevatorPlugin.Instance.OpenElevatorUI(player, closestElevator);
            }
            else
            {
                // Failure: Player is not close enough to any elevator
                UnturnedChat.Say(player, "You are not near an elevator.", Color.yellow);
            }
        }
    }
}