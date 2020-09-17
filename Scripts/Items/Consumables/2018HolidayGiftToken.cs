using Server.Gumps;

namespace Server.Items
{
    public enum HolidayGift2018
    {
        None = 0,
        Wreath = 1,
        Sign = 2,
        RecipeBook = 3
    }

    public class HolidayGiftToken2018 : Item, IRewardOption
    {
        public override int LabelNumber => 1158831;  // 2018 Holiday Gift Token

        public HolidayGift2018 Gift { get; set; }

        [Constructable]
        public HolidayGiftToken2018()
           : this(HolidayGift2018.None)
        { }

        [Constructable]
        public HolidayGiftToken2018(HolidayGift2018 gift)
            : base(0xA094)
        {
            LootType = LootType.Blessed;
            Light = LightType.Circle300;
            Weight = 1.0;
            Gift = gift;
            Hue = 1371;
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)HolidayGift2018.Wreath, 1158812);
            list.Add((int)HolidayGift2018.Sign, 1158811);
            list.Add((int)HolidayGift2018.RecipeBook, 1158810);
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            Gift = (HolidayGift2018)choice;

            if (!Deleted)
                GiveGift(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1156381));
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GiveGift(Mobile from)
        {
            GiftBox box = new GiftBox();

            Item gift = new GingerBreadCookie
            {
                LootType = LootType.Regular
            };
            box.DropItem(gift);

            if (Utility.Random(100) < 60)
                box.DropItem(new Poinsettia(33));
            else
                box.DropItem(new Poinsettia(1154));

            switch (Gift)
            {
                case HolidayGift2018.RecipeBook:
                    {
                        box.DropItem(new RecipeBook());
                        break;
                    }
                case HolidayGift2018.Sign:
                    {
                        box.DropItem(new HolidaysSign());
                        break;
                    }
                case HolidayGift2018.Wreath:
                    {
                        box.DropItem(new HolidayWreath());
                        break;
                    }
            }

            from.AddToBackpack(box);

            Delete();
        }

        public HolidayGiftToken2018(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
