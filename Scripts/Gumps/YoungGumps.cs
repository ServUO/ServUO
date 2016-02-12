using System;
using Server.Accounting;
using Server.Network;

namespace Server.Gumps
{
    public class YoungDungeonWarning : Gump
    {
        public override int TypeID { get { return 0x82; } }

        public YoungDungeonWarning()
            : base(150, 200)
        {
            this.AddBackground(0, 0, 250, 170, 0xA28);

            this.AddHtmlLocalized(20, 43, 215, 70, 1018030, true, true); // Warning: monsters may attack you on site down here in the dungeons!

            this.AddButton(70, 123, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(105, 125, 100, 35, 1011036, false, false); // OKAY
        }
    }

    public class YoungDeathNotice : Gump
    {
        public override int TypeID { get { return 0x28D; } }

        public YoungDeathNotice()
            : base(100, 15)
        {
            this.Closable = false;

            this.AddBackground(25, 10, 425, 444, 0x13BE);

            this.AddImageTiled(33, 20, 407, 425, 0xA40);
            this.AddAlphaRegion(33, 20, 407, 425);

            this.AddHtmlLocalized(190, 24, 120, 20, 1046287, 0x7D00, false, false); // You have died.

            // As a ghost you cannot interact with the world. You cannot touch items nor can you use them.
            this.AddHtmlLocalized(50, 50, 380, 40, 1046288, 0xFFFFFF, false, false);
            // You can pass through doors as though they do not exist.  However, you cannot pass through walls.
            this.AddHtmlLocalized(50, 100, 380, 45, 1046289, 0xFFFFFF, false, false);
            // Since you are a new player, any items you had on your person at the time of your death will be in your backpack upon resurrection.
            this.AddHtmlLocalized(50, 140, 380, 60, 1046291, 0xFFFFFF, false, false);
            // To be resurrected you must find a healer in town or wandering in the wilderness.  Some powerful players may also be able to resurrect you.
            this.AddHtmlLocalized(50, 204, 380, 65, 1046292, 0xFFFFFF, false, false);
            // While you are still in young status, you will be transported to the nearest healer (along with your items) at the time of your death.
            this.AddHtmlLocalized(50, 269, 380, 65, 1046293, 0xFFFFFF, false, false);
            // To rejoin the world of the living simply walk near one of the NPC healers, and they will resurrect you as long as you are not marked as a criminal.
            this.AddHtmlLocalized(50, 334, 380, 70, 1046294, 0xFFFFFF, false, false);

            this.AddButton(195, 410, 0xF8, 0xF9, 0, GumpButtonType.Reply, 0);
        }
    }

    public class RenounceYoungGump : Gump
    {
        public override int TypeID { get { return 0x5B9C; } }

        public RenounceYoungGump()
            : base(150, 50)
        {
            this.AddBackground(0, 0, 450, 400, 0xA28);

            this.AddHtmlLocalized(0, 30, 450, 35, 1013004, false, false); // <center> Renouncing 'Young Player' Status</center>

            /* As a 'Young' player, you are currently under a system of protection that prevents
            * you from being attacked by other players and certain monsters.<br><br>
            * 
            * If you choose to renounce your status as a 'Young' player, you will lose this protection.
            * You will become vulnerable to other players, and many monsters that had only glared
            * at you menacingly before will now attack you on sight!<br><br>
            * 
            * Select OKAY now if you wish to renounce your status as a 'Young' player, otherwise
            * press CANCEL.
            */
            this.AddHtmlLocalized(30, 70, 390, 210, 1013005, true, true);

            this.AddButton(45, 298, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(78, 300, 100, 35, 1011036, false, false); // OKAY

            this.AddButton(178, 298, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(211, 300, 100, 35, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 1)
            {
                Account acc = from.Account as Account;

                if (acc != null)
                {
                    acc.RemoveYoungStatus(502085); // You have chosen to renounce your `Young' player status.
                }
            }
            else
            {
                from.SendLocalizedMessage(502086); // You have chosen not to renounce your `Young' player status.
            }
        }
    }
}