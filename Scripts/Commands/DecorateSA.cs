using System;
using Server.Commands;
using Server.Items;

namespace Server
{
    public static class StygianAbyss
    { 
        public static void Initialize()
        {
			CommandSystem.Register("DecorateSA", AccessLevel.Administrator, new CommandEventHandler(DecorateSA_OnCommand));
			CommandSystem.Register("DecorateSADelete", AccessLevel.Administrator, new CommandEventHandler(DecorateSADelete_OnCommand));
		}
        [Usage("DecorateSADelete")]
        [Description("Deletes Stygian Abyss world decoration.")]
		private static void DecorateSADelete_OnCommand(CommandEventArgs e)
		{
			WeakEntityCollection.Delete("sa");
		}

        [Usage("DecorateSA")]
        [Description("Generates Stygian Abyss world decoration.")]
        private static void DecorateSA_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Generating Stygian Abyss world decoration, please wait.");
			
            Decorate.Generate("sa", "Data/Decoration/Stygian Abyss/Ter Mur", Map.TerMur);
			Decorate.Generate("sa", "Data/Decoration/Stygian Abyss/Trammel", Map.Trammel);
			Decorate.Generate("sa", "Data/Decoration/Stygian Abyss/Felucca", Map.Felucca);

            NavreysController.GenNavery(e.Mobile);
            Server.Engines.CannedEvil.PrimevalLichPuzzle.GenLichPuzzle(e.Mobile);

            e.Mobile.SendMessage("Stygian Abyss world generation complete.");
        }
    }
}