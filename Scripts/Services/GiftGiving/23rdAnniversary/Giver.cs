using Server.Items;
using System;

namespace Server.Misc
{
    public class AnniversaryGiver23rd : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register(new AnniversaryGiver23rd());
        }

        public override DateTime Start => new DateTime(2020, 09, 01);
        public override DateTime Finish => new DateTime(2020, 10, 01);
        public override TimeSpan MinimumAge => TimeSpan.FromDays(30);

        public override void GiveGift(Mobile mob)
        {
            Item token = new Anniversary23GiftToken();

            switch (GiveGift(mob, token))
            {
                case GiftResult.Backpack:
                    mob.SendLocalizedMessage(1159512); // Happy Anniversary! We have placed a gift for you in your backpack.
                    break;
                case GiftResult.BankBox:
                    mob.SendLocalizedMessage(1159513); // Happy Anniversary! We have placed a gift for you in your bank box. 
                    break;
            }
        }
    }
}
