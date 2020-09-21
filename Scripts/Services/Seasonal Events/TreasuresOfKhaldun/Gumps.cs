using Server.Engines.Points;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Khaldun
{
    public class KhaldunRewardGump : BaseRewardGump
    {
        public KhaldunRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, KhaldunRewards.Rewards, 1158744)
        {
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.Khaldun.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.Khaldun.DeductPoints(User, points);
        }
    }

    public class GumshoeItemGump : Gump
    {
        public GumshoeItemGump(Mobile from, int itemID, int itemHue, string itemName, int cliloc, string secondLineText)
            : base(50, 50)
        {
            AddBackground(0, 0, 454, 400, 9380);
            AddItem(75, 120, itemID, itemHue);
            AddHtml(177, 50, 250, 20, string.Format("<center><basefont color=#6B1010>{0}</center>", itemName), false, false);

            if (!string.IsNullOrEmpty(secondLineText))
            {
                AddHtml(177, 77, 250, 40, string.Format("<center><basefont color=#6B1010>{0}</center>", secondLineText), false, false);
                AddHtmlLocalized(177, 122, 250, 228, cliloc, true, true);
            }
            else
                AddHtmlLocalized(177, 77, 250, 273, cliloc, true, true);
        }
    }
}
