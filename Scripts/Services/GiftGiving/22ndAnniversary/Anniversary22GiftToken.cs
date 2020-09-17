using Server.Gumps;

namespace Server.Items
{
    public class Anniversary22GiftToken : Item, IRewardOption
    {
        public override int LabelNumber => 1159145;  // 22nd Anniversary Gift Token

        [Constructable]
        public Anniversary22GiftToken()
            : base(0x4BC6)
        {
            Hue = 1286;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1156888));
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, 1159146); // Copper Wings
            list.Add(2, 1159147); // Copper Portraits
            list.Add(3, 1159148); // Copper Ship Relief
            list.Add(4, 1159149); // Copper Sunflower
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            Bag bag = new Bag
            {
                Hue = 1286
            };

            bool chance = Utility.RandomDouble() < .1;

            Item item;

            switch (choice)
            {
                default:
                    bag.Delete();
                    break;
                case 1:
                    {
                        item = new CopperWings();

                        if (chance)
                        {
                            item.Hue = 2951;
                        }

                        bag.DropItem(item);
                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }
                case 2:
                    {
                        item = new CopperPortrait1();

                        if (chance)
                        {
                            item.Hue = 2951;
                        }

                        bag.DropItem(item);

                        item = new CopperPortrait2();

                        if (chance)
                        {
                            item.Hue = 2951;
                        }

                        bag.DropItem(item);

                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }

                case 3:
                    {
                        item = new CopperShipReliefAddonDeed();

                        if (chance)
                        {
                            item.Hue = 2951;
                        }

                        bag.DropItem(item);

                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }
                case 4:
                    item = new CopperSunflower();

                    if (chance)
                    {
                        item.Hue = 2951;
                    }

                    bag.DropItem(item);

                    from.AddToBackpack(bag);
                    Delete();
                    break;
            }
        }

        public Anniversary22GiftToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
