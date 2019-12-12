using System;
using Server;
using Server.Items;

namespace Server.Misc
{
    public class AnniversaryGiver22nd : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register(new AnniversaryGiver22nd());
        }

        public override DateTime Start { get { return new DateTime(2019, 09, 01); } }
        public override DateTime Finish { get { return new DateTime(2019, 10, 18); } }
        public override TimeSpan MinimumAge { get { return TimeSpan.FromDays(30); } }

        public override void GiveGift(Mobile mob)
        {
            Item token = new Anniversary22GiftToken();

            switch (GiveGift(mob, token))
            {
                case GiftResult.Backpack:
                    mob.SendLocalizedMessage(1159142); // Happy 22nd Anniversary! We have placed a gift for you in your backpack.
                    break;
                case GiftResult.BankBox:
                    mob.SendLocalizedMessage(1159143); // Happy 22nd Anniversary! We have placed a gift for you in your bank box. 
                    break;
            }
        }
    }
}
