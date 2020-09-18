using Server.Items;
using System;

namespace Server.Misc
{
    public class HolidayGiftBox2018 : GiftGiver
    {
        public override DateTime Start => new DateTime(2018, 12, 30);
        public override DateTime Finish => new DateTime(2019, 1, 1);

        public static void Initialize()
        {
            GiftGiving.Register(new HolidayGiftBox2018());
        }

        public override void GiveGift(Mobile mob)
        {
            HolidayGiftToken2018 gift = new HolidayGiftToken2018();

            switch (GiveGift(mob, gift))
            {
                case GiftResult.Backpack:
                    mob.SendMessage(0x482, "Happy Holidays from the team!  Gift items have been placed in your backpack.");
                    break;
                case GiftResult.BankBox:
                    mob.SendMessage(0x482, "Happy Holidays from the team!  Gift items have been placed in your bank box.");
                    break;
            }
        }
    }
}
