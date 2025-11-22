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

            if (config?.Elevators == null || !config.Elevators.Any())
            {
                UnturnedChat.Say(player, "No elevators are configured on this server.", Color.red);
                return;
            }

            Elevator closestElevator = null;
            float closestDistance = float.MaxValue;

            foreach (var elevator in config.Elevators)
            {
                float distance = Vector3.Distance(player.Position, elevator.Position.ToVector3());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestElevator = elevator;
                }
            }

            const float maxActivationDistance = 10f;

            if (closestElevator != null && closestDistance <= maxActivationDistance)
            {
                ElevatorPlugin.Instance.OpenElevatorUI(player, closestElevator);
            }
            else
            {
                UnturnedChat.Say(player, "You are not near an elevator.", Color.yellow);
            }
        }
    }
}