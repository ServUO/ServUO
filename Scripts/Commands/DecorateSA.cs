using System;
using Server.Commands;

namespace Server
{
    public static class StygianAbyss
    { 
        public static void Initialize()
        {
            CommandSystem.Register("DecorateSA", AccessLevel.Administrator, new CommandEventHandler(DecorateSA_OnCommand));
        }

        [Usage("DecorateSA")]
        [Description("Generates Stygian Abyss world decoration.")]
        private static void DecorateSA_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Generating Stygian Abyss world decoration, please wait.");
			
            Decorate.Generate("Data/Decoration/Stygian Abyss/Ter Mur", Map.TerMur);
            Decorate.Generate("Data/Decoration/Stygian Abyss/Trammel", Map.Trammel);
            Decorate.Generate("Data/Decoration/Stygian Abyss/Felucca", Map.Felucca);
			
            e.Mobile.SendMessage("Stygian Abyss world generation complete.");
        }
    }
}