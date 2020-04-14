using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.VeteranRewards
{
    public class StatRewardGump : BaseGump
    {
        public StatRewardGump(PlayerMobile from)
            : base(from, 200, 200)
        {
            from.CloseGump(typeof(StatRewardGump));
            TypeID = 0x193;
        }

        public override void AddGumpLayout()
        {
            AddHtmlLocalized(0, 0, 0, 0, 1015313, false, false); // <center></center>
            AddHtmlLocalized(0, 0, 0, 0, 1015313, false, false); // <center></center>
            AddHtmlLocalized(0, 0, 0, 0, 1076664, false, false); // <B>Ultima Online Rewards Program</B><BR>Thank you for being part of the Ultima Online community for over 6 months. As a token of our appreciation, your stat cap will be increased. 
            AddHtmlLocalized(0, 0, 0, 0, 1011036, false, false); // OKAY
            AddHtmlLocalized(0, 0, 0, 0, 1011012, false, false); // CANCEL

            AddPage(0);

            AddBackground(0, 0, 291, 173, 0x13BE);
            AddImageTiled(5, 5, 280, 140, 0xA40);
            AddHtmlLocalized(9, 9, 272, 140, 1076664, 0x7FFF, false, false); // <B>Ultima Online Rewards Program</B><BR>Thank you for being part of the Ultima Online community for over 6 months. As a token of our appreciation, your stat cap will be increased. 
            AddButton(160, 147, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 149, 120, 20, 1006044, 0x7FFF, false, false); // OK
            AddButton(5, 147, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 149, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
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
