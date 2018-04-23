using System;
using Server;
using Server.Items;
using Server.Commands;

namespace Server.Scripts.Commands
{
    //[pick command: uses lockpick from backpack, or withdraws from keys if not found
    public class Pick
    {
        public static void Initialize()
        {
            CommandSystem.Register("Pick",AccessLevel.GameMaster,new CommandEventHandler(Pick_Callback));
        }

        [Usage("Pick")]
        [Description("this will use lockpicks found within a player's pack (or withdraw from keys if needed)")]
        public static void Pick_Callback(CommandEventArgs e)
        {
            Lockpick pick = (Lockpick)BaseStoreKey.FindItemByType(e.Mobile.Backpack,typeof(Lockpick));

            if (pick == null)
            {
                e.Mobile.SendMessage("You need lockpicks");
                return;
            }

            pick.OnDoubleClick(e.Mobile);
        }
    }
}