using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace ElevatorPlugin.Commands
{
    public class CommandCheckPos : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "checkpos";
        public string Help => "Shows your current position (X, Y, Z)";
        public string Syntax => string.Empty;
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "checkpos" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            Vector3 pos = player.Position;

            UnturnedChat.Say(player, $"Your position: X: {pos.x:F2}, Y: {pos.y:F2}, Z: {pos.z:F2}", Color.green);
            Logger.Log($"{player.DisplayName} checked position: X: {pos.x:F2}, Y: {pos.y:F2}, Z: {pos.z:F2}");
        }
    }
}
