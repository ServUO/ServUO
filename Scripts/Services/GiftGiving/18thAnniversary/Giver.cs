using Server.Items;
using System;

namespace Server.Misc
{
    public class AnniversaryGiver18th : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register(new AnniversaryGiver18th());
        }

        public override DateTime Start => new DateTime(2016, 4, 08);
        public override DateTime Finish => new DateTime(2016, 12, 31);
        public override TimeSpan MinimumAge => TimeSpan.FromHours(12);

        public override void GiveGift(Mobile mob)
        {
            Bag bag = new AnniversaryBag18th(mob);

            switch (GiveGift(mob, bag))
            {
                case GiftResult.Backpack:
                    mob.SendLocalizedMessage(1156142); // Happy 18th Anniversary! We have placed a gift for you in your backpack.
                    break;
                case GiftResult.BankBox:
                    mob.SendLocalizedMessage(1156143); // Happy 18th Anniversary! We have placed a gift for you in your bank box. 
                    break;
            }
        }
    }
}