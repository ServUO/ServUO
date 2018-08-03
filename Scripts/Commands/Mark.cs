using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Commands
{
    public class Mark
    {
        public static void Initialize()
        {
            CommandSystem.Register("Mark", AccessLevel.GameMaster, new CommandEventHandler(Mark_OnCommand));
        }

        [Usage("Mark [name]")]
        [Description("Creates a marked rune at your location.")]
        private static void Mark_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length <= 0)
            {
                e.Mobile.SendMessage("Usage: mark [RuneName]");
                return;
            }

            var runeName = e.Arguments[0];

            var rune = new RecallRune();
            rune.Mark(e.Mobile);
            rune.Name = rune.Description = runeName;

            e.Mobile.AddToBackpack(rune);
            e.Mobile.SendMessage(string.Format("Rune {0} added to your backpack.", runeName));
        }
    }
}
