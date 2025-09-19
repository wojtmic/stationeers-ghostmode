using Assets.Scripts;
using BepInEx.Logging;

namespace SpectatorCamMod;
using Util.Commands;

public class GhostCommand : CommandBase
{
    public override string HelpText => "Makes a player invisible, immortal and makes the jetpack infinite";

    public override string[] Arguments => new string[1] { "<clientId>" };

    public override bool IsLaunchCmd => false;

    public override string Execute(string[] args)
    {
        if (CommandBase.CannotAsClient("save"))
        {
            return "You need to be the host to run this command"; // Can't save means we are not the host
        }

        if (args.Length < 1)
        {
            return "Client ID required, Usage: ghost <clientId>";
        }

        if (!ulong.TryParse(args[0], out ulong clientId))
        {
            return "Invalid Client ID";
        }
        
        Client targetClient = Client.Find(clientId);
        if (targetClient == null)
        {
            return "Client not found";
        }

        bool isNowGhost = GhostManager.TogglePlayer(clientId);
        
        return isNowGhost ? "Made the player a Ghost!" : "Made the player not a Ghost!";
    }
}