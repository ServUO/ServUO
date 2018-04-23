//contributed by datguy AKA Morpheus of BES Oasis shard

using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Network;
using Server.Targeting;
using Server.Commands;
using Server.ContextMenus;
using Solaris.Extras;

namespace Server.Commands
{
    public class KeyGuardCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("KeyGuard",AccessLevel.Player,new CommandEventHandler(KeyGuard_OnCommand));
        }

        [Usage("KeyGuard")]
        [Description("Toggles whether an item can be pulled into storage keys")]
        public static void KeyGuard_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Which item do you wish to guard against storage key entry?");
            e.Mobile.Target = new KeyGuardTarget();
        }

        public class KeyGuardTarget : Target
        {
            public KeyGuardTarget() : base(10,false,TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from,object targeted)
            {
                if (targeted == null)
                {
                    return;
                }
                if (targeted is Mobile || targeted is Container || targeted is Corpse || targeted is LandTarget || targeted is Static)
                {
                    from.SendLocalizedMessage(502816); // You feel that such an action would be inappropriate
                    return;
                }

                if (targeted is Item)
                {
                    Item theitem = targeted as Item;
                    if (theitem.IsChildOf(from.Backpack))
                    {
                        //Note: is this necessary?
                        if (!from.InLOS(theitem) || !theitem.IsAccessibleTo(from) || !theitem.Movable)
                        {
                            from.SendLocalizedMessage(502816); // You feel that such an action would be inappropriat
                            return;
                        }

                        //if it's in your backpack already, it shouldn't be also worn
                        if (theitem.Parent is Mobile)
                        {
                            from.SendMessage(33,"You cannot target Mobiles or something on a Mobile. Try again.");
                            return;
                        }

                        //pass over to the extras dealie
                        ExecuteExtras.ToggleKeyGuard(from,theitem);
                    }
                    else
                    {
                        from.SendMessage(33,"Item needs to be in your backpack");
                    }
                }
            }
        }
    }
}