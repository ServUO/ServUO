using System;
using Server;
using Server.Mobiles;
using Server.Commands;

public class PrintSkillsCommand
{
    public static void Initialize()
    {
        CommandSystem.Register("PrintSkills", AccessLevel.Player, new CommandEventHandler(PrintSkills_OnCommand));
        CommandSystem.Register("MaxSkills", AccessLevel.Player, new CommandEventHandler(MaxSkills_OnCommand));
    }

    [Usage("PrintSkills")]
    [Description("Prints all your skills and their values to your chat.")]
    public static void PrintSkills_OnCommand(CommandEventArgs e)
    {
        Mobile from = e.Mobile; // The player executing the command

        if (from is PlayerMobile player)
        {
            foreach (Skill skill in player.Skills)
            {
                player.SendMessage($"Skill: {skill.Name}, Value: {skill.Base:F1}");
            }
        }
        else
        {
            e.Mobile.SendMessage("This command is only available to players.");
        }
    }

    [Usage("MaxSkills")]
    [Description("Sets all your skills to 100.")]
    public static void MaxSkills_OnCommand(CommandEventArgs e)
    {
        Mobile from = e.Mobile; // The player executing the command

        if (from is PlayerMobile player)
        {
            foreach (Skill skill in player.Skills)
            {
                skill.Base = 100.0; // Sets the base skill value to 100
            }

            player.SendMessage("All your skills have been set to 100.");
        }
        else
        {
            e.Mobile.SendMessage("This command is only available to players.");
        }
    }
}
