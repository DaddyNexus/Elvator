using Rocket.API;
using Rocket.Core.Logging; // Add this using directive for the Logger
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace YourPluginNamespace
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

            // Send a private message to the player in-game
            UnturnedChat.Say(player, $"📍 Your position: X: {pos.x:F2}, Y: {pos.y:F2}, Z: {pos.z:F2}", Color.green);

            // NEW: Log the player's name and position to the server console
            Logger.Log($"{player.DisplayName} checked position: X: {pos.x:F2}, Y: {pos.y:F2}, Z: {pos.z:F2}");
        }
    }
}