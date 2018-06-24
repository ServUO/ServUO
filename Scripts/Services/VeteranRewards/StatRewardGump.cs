using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.VeteranRewards
{
    public class StatRewardGump : BaseGump
    {
        public StatRewardGump(PlayerMobile from)
            : base(from, 50, 50)
        {
            from.CloseGump(typeof(StatRewardGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 291, 173, 9200);
            AddImageTiled(5, 5, 281, 140, 2624);

            AddHtmlLocalized(7, 7, 277, 126, 1076664, 0x7FFF, false, false);
            /*<B>Ultima Online Rewards Program</B><BR>Thank you for being part of the Ultima Online community for over 6 months. 
             * As a token of our appreciation, your stat cap will be increased. */

            AddButton(5, 150, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 150, 100, 20, 1011012, 0x7FFF, false, false); // CANCEL

            AddButton(160, 150, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddHtml(195, 150, 100, 20, Color("#FFFFFF", "OK"), false, false);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (!User.HasStatReward && info.ButtonID == 1)
            {
                User.HasStatReward = true;
                User.StatCap += 5;

                User.SendLocalizedMessage(1062312); // Your stat cap has been increased.
            }
        }
    }
}