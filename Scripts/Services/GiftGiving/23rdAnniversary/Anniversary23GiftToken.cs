using Server.Gumps;

namespace Server.Items
{
    public class Anniversary23GiftToken : Item, IRewardOption
    {
        public override int LabelNumber => 1159506;  // 23rd Anniversary Gift Token	

        [Constructable]
        public Anniversary23GiftToken()
            : base(0x4BC6)
        {
            Hue = 2725;
            LootType = LootType.Blessed;
        }

        public Anniversary23GiftToken(Serial serial)
            : base(serial)
        {
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
            list.Add(1, 1159507); // Silver Plated Lamppost
            list.Add(2, 1159508); // Silver Plated Bubbling Cauldron
            list.Add(3, 1159509); // Silver Plated Tome
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            Bag bag = new Bag
            {
                Hue = 2751
            };

            switch (choice)
            {
                default:
                    bag.Delete();
                    break;
                case 1:
                    {
                        bag.DropItem(new SilverPlatedLamppost());
                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }
                case 2:
                    {
                        bag.DropItem(new SilverPlatedBubblingCauldronDeed());

                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }

                case 3:
                    {
                        bag.DropItem(new SilverPlatedTome());
                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
