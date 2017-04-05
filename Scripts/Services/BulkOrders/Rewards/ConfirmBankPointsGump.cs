using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    public class ConfirmBankPointsGump : ConfirmCallbackGump
    {
        public Item BOD { get; set; }
        public int Points { get; set; }
        public double Banked { get; set; }

        public Mobile Owner { get; set; }
        public BODType BODType { get; set; }

        public ConfirmBankPointsGump(PlayerMobile user, Mobile owner, Item bod, BODType type, int points, double banked)
            : base(user, 1157076, 1157077, new object[] { points, banked, type, owner }, String.Format("{0}\t{1}", banked.ToString("0.000000"), points.ToString()), OnSave, OnClaim)
        {
            BOD = bod;
            Points = points;
            Banked = banked;

            Owner = owner;

            Rectangle2D rec = ItemBounds.Table[BOD.ItemID];

            AddItem(115 + rec.Width / 2 - rec.X, 110 + rec.Height / 2 - rec.Y, BOD.ItemID, BOD.Hue);
        }

        private static void OnSave(Mobile m, object state)
        {
            object[] ohs = (object[])state;

            BulkOrderSystem.SetPoints(m, (BODType)ohs[2], (double)ohs[1]);

            if(m is PlayerMobile)
                m.SendGump(new RewardsGump((Mobile)ohs[3], (PlayerMobile)m, (BODType)ohs[2]));
        }

        private static void OnClaim(Mobile m, object state)
        {
            object[] ohs = (object[])state;

            if(m is PlayerMobile)
                m.SendGump(new RewardsGump((Mobile)ohs[3], (PlayerMobile)m, (BODType)ohs[2], (int)ohs[0]));
        }
    }
}