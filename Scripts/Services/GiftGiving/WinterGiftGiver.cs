using Server.Items;
using System;

namespace Server.Misc
{
    public class WinterGiftGiver : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register(new WinterGiftGiver());
        }

        public override DateTime Start => new DateTime(2019, 09, 01); // When do you want your gift hand out?
        public override DateTime Finish => new DateTime(2019, 10, 01); // When do you want your gift hand out to stop?
        public override TimeSpan MinimumAge => TimeSpan.FromDays(30); // How old does a character have to be to obtain?

        public override void GiveGift(Mobile mob)
        {
            GiftBox box = new GiftBox();

            box.DropItem(new MistletoeDeed());
            box.DropItem(new PileOfGlacialSnow());
            box.DropItem(new LightOfTheWinterSolstice());

            int random = Utility.Random(100);

            if (random < 60)
                box.DropItem(new DecorativeTopiary());
            else if (random < 84)
                box.DropItem(new FestiveCactus());
            else
                box.DropItem(new SnowyTree());

            switch (GiveGift(mob, box))
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
