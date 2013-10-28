using System;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class SQGump : Gump
    {
        public SQGump(Mobile owner)
            : base(50, 50)
        {
            //----------------------------------------------------------------------------------------------------
            this.AddPage(0);
            this.AddImageTiled(54, 33, 369, 400, 2624);
            this.AddAlphaRegion(54, 33, 369, 400);

            this.AddImageTiled(416, 39, 44, 389, 203);
            //--------------------------------------Window size bar--------------------------------------------

            this.AddImage(97, 49, 9005);
            this.AddImageTiled(58, 39, 29, 390, 10460);
            this.AddImageTiled(412, 37, 31, 389, 10460);
            this.AddLabel(140, 60, 1153, "Quest Offer");
            this.AddTextEntry(155, 110, 200, 20, 1163, 0, @"La Insep Ohm");
            this.AddTextEntry(107, 130, 200, 20, 1163, 0, @"Description");
            //AddLabel(175, 125, 200, 20, 1163, 0,"La Insep Ohm");
            //AddLabel(85, 135, 200, 20, 1163, 0, "Description");

            this.AddHtml(107, 155, 300, 230, "<BODY>" +
                                             //----------------------/----------------------------------------------/
                                             "<BASEFONT COLOR=WHITE>Repeating the mantra, you gradually enter a state of enlightened meditation.<br><br>As you contemplate your worthiness, an image of the Book of Circles comes into focus.<br><br>Perhaps you are ready for La Insep Om?<br>" +
                                             "</BODY>", false, true);

            this.AddImage(430, 9, 10441);
            this.AddImageTiled(40, 38, 17, 391, 9263);
            this.AddImage(6, 25, 10421);
            this.AddImage(34, 12, 10420);
            this.AddImageTiled(94, 25, 342, 15, 10304);
            this.AddImageTiled(40, 427, 415, 16, 10304);
            this.AddImage(-10, 314, 10402);
            this.AddImage(56, 150, 10411);
            this.AddImage(136, 84, 96);
            this.AddButton(315, 380, 12018, 12019, 1, GumpButtonType.Reply, 1);
            this.AddButton(114, 380, 12000, 12001, 0, GumpButtonType.Reply, 0);
        }

        public static void Initialize()
        {
            CommandSystem.Register("SQGump", AccessLevel.GameMaster, new CommandEventHandler(SQGump_OnCommand));
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.SendGump(new SQ1Gump(from));
                        from.CloseGump(typeof(SQGump));
                       
                        break;
                    }
                case 1:
                    {
                        from.SendLocalizedMessage(1112683);
                        from.CloseGump(typeof(SQGump));
                        break;
                    }
            }
        }

        private static void SQGump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new SQGump(e.Mobile));
        }
    }
}