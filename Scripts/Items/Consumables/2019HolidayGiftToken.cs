using Server.Gumps;
using Server.Misc;
using System;

namespace Server.Items
{
    public class HolidayGiftGiver2019 : GiftGiver
    {
        public static void Initialize()
        {
            GiftGiving.Register(new HolidayGiftGiver2019());
        }

        public override DateTime Start => new DateTime(2019, 12, 03);
        public override DateTime Finish => new DateTime(2020, 01, 03);
        public override TimeSpan MinimumAge => TimeSpan.FromDays(30);

        public override void GiveGift(Mobile mob)
        {
            Item token = new HolidayGiftToken2019();

            switch (GiveGift(mob, token))
            {
                case GiftResult.Backpack:
                    mob.SendLocalizedMessage(1062835); // Happy Holidays from the Origin team!  Gift items have been placed in your backpack.
                    break;
                case GiftResult.BankBox:
                    mob.SendLocalizedMessage(1062836); // Happy Holidays from the Origin team!  Gift items have been placed in your bank box.
                    break;
            }
        }
    }

    public enum HolidayGift2019
    {
        None = 0,
        HolidayWallArt = 1,
        DoveCage = 2,
        JollyHolidayTree = 3
    }

    public class HolidayGiftToken2019 : Item, IRewardOption
    {
        public override int LabelNumber => 1159257;  // 2019 Holiday Gift Token

        public HolidayGift2019 Gift { get; set; }

        [Constructable]
        public HolidayGiftToken2019()
           : this(HolidayGift2019.None)
        { }

        [Constructable]
        public HolidayGiftToken2019(HolidayGift2019 gift)
            : base(0xA094)
        {
            LootType = LootType.Blessed;
            Light = LightType.Circle300;
            Weight = 1.0;
            Gift = gift;
            Hue = 133;
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)HolidayGift2019.HolidayWallArt, 1159254);
            list.Add((int)HolidayGift2019.DoveCage, 1159255);
            list.Add((int)HolidayGift2019.JollyHolidayTree, 1159256);
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            Gift = (HolidayGift2019)choice;

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
                case HolidayGift2019.JollyHolidayTree:
                    {
                        box.DropItem(new JollyHolidayTreeDeed());
                        break;
                    }
                case HolidayGift2019.DoveCage:
                    {
                        box.DropItem(new DoveCage());
                        break;
                    }
                case HolidayGift2019.HolidayWallArt:
                    {
                        switch (Utility.Random(2))
                        {
                            case 0:
                                {
                                    box.DropItem(new HolidayWallArt1());
                                    break;
                                }
                            case 1:
                                {
                                    box.DropItem(new HolidayWallArt2());
                                    break;
                                }
                            case 2:
                                {
                                    box.DropItem(new HolidayWallArt3());
                                    break;
                                }
                        }

                        break;
                    }
            }

            from.SendLocalizedMessage(1062306); // Your gift has been created and placed in your backpack.
            from.AddToBackpack(box);

            Delete();
        }

        public HolidayGiftToken2019(Serial serial)
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
