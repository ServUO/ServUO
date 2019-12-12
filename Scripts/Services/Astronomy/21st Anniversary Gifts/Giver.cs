using System;
using Server;
using Server.Items;

namespace Server.Misc
{
    public class AnniversaryGiver21st : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register(new AnniversaryGiver21st());
        }

        public override DateTime Start { get { return new DateTime(2019, 4, 05); } }
        public override DateTime Finish { get { return new DateTime(2019, 5, 31); } }
        public override TimeSpan MinimumAge { get { return TimeSpan.FromHours(12); } }

        public override void GiveGift(Mobile mob)
        {
            var bag = new AnniversaryBag21st(mob);

            switch (GiveGift(mob, bag))
            {
                case GiftResult.Backpack:
                    mob.SendLocalizedMessage(1158482); // Happy 21st Anniversary! We have placed a gift for you in your backpack.
                    break;
                case GiftResult.BankBox:
                    mob.SendLocalizedMessage(1158483); // Happy 21st Anniversary! We have placed a gift for you in your bank box.
                    break;
            }
        }
    }
}
