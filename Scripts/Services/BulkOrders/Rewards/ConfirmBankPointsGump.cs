using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    public class ConfirmBankPointsGump : ConfirmCallbackGump
    {
        public int Points { get; set; }
        public double Banked { get; set; }

        public Mobile Owner { get; set; }
        public BODType BODType { get; set; }

        public ConfirmBankPointsGump(PlayerMobile user, Mobile owner, BODType type, int points, double banked)
            : base(user, 1157076, 1157077, new object[] { points, banked, type, owner }, string.Format("{0}\t{1}", banked.ToString("0.000000"), points.ToString()), OnSave, OnClaim)
        {
            Closable = false;
            user.CloseGump(typeof(ConfirmBankPointsGump));

            Points = points;
            Banked = banked;
            BODType = type;

            Owner = owner;

            Rectangle2D rec = ItemBounds.Table[0x2258];

            AddItem(115 + rec.Width / 2 - rec.X, 110 + rec.Height / 2 - rec.Y, 0x2258, BulkOrderSystem.GetBodHue(BODType));
        }

        private static void OnSave(Mobile m, object state)
        {
            object[] ohs = (object[])state;

            BulkOrderSystem.SetPoints(m, (BODType)ohs[2], (double)ohs[1]);
            BulkOrderSystem.RemovePending(m, (BODType)ohs[2]);

            if (m is PlayerMobile)
                m.SendGump(new RewardsGump((Mobile)ohs[3], (PlayerMobile)m, (BODType)ohs[2]));
        }

        private static void OnClaim(Mobile m, object state)
        {
            object[] ohs = (object[])state;

            if (m is PlayerMobile)
                m.SendGump(new RewardsGump((Mobile)ohs[3], (PlayerMobile)m, (BODType)ohs[2], (int)ohs[0]));
        }
    }
}
