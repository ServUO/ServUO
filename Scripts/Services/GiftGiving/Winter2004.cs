using System;
using Server.Items;

namespace Server.Misc
{
    public class WinterGiftGiver2004 : GiftGiver
    {
        public override DateTime Start
        {
            get
            {
                return new DateTime(2004, 12, 24);
            }
        }
        public override DateTime Finish
        {
            get
            {
                return new DateTime(2005, 1, 1);
            }
        }
        public static void Initialize()
        {
            GiftGiving.Register(new WinterGiftGiver2004());
        }

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

            switch ( this.GiveGift(mob, box) )
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