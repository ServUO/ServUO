using System;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class SQ5Gump : Gump
    {
        public SQ5Gump(Mobile owner)
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

            this.AddHtml(107, 155, 300, 230, "<BODY>" +
                                             //----------------------/----------------------------------------------/
                                             "<BASEFONT COLOR=WHITE>Answering the last question correctly, you feel a strange energy wash over you.<br><br>You don't understand how you know, but you are absolutely certain that the guardians will no longer bar you from entering the Stygian Abyss.<br><br>It seems you have proven yourself worthy of La Insep Om.<br>" +
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

            this.AddButton(114, 380, 12009, 12010, 0, GumpButtonType.Reply, 0);
        }

        public static void Initialize()
        {
            CommandSystem.Register("SQ5Gump", AccessLevel.GameMaster, new CommandEventHandler(SQ5Gump_OnCommand));
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            PlayerMobile from = state.Mobile as PlayerMobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.CloseGump(typeof(SQ5Gump));
                        from.PlaySound(0x1E0);
                        from.FixedParticles(0x3709, 1, 30, 1153, 13, 3, EffectLayer.Head);
                        from.AbyssEntry = true;

                        break;
                    }
            }
        }

        private static void SQ5Gump_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new SQ5Gump(e.Mobile));
        }
    }
}