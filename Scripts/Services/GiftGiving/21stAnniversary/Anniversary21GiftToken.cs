using Server.Gumps;

namespace Server.Items
{
    public class Anniversary21GiftToken : Item, IRewardOption
    {
        public override int LabelNumber => 1158485;  // 21st Anniversary Gift Token

        [Constructable]
        public Anniversary21GiftToken()
            : base(19398)
        {
            Hue = 2113;
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
            list.Add(1, 1158479); // Brass Orrery
            list.Add(2, 1158480); // Brass Telescope
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            Bag bag = new Bag
            {
                Hue = 2720
            };

            switch (choice)
            {
                default:
                    bag.Delete();
                    break;
                case 1:
                    bag.DropItem(new BrassOrrery());
                    from.AddToBackpack(bag); Delete();
                    break;
                case 2:
                    bag.DropItem(new PersonalTelescope());
                    from.AddToBackpack(bag);
                    Delete(); break;
            }
        }

        public Anniversary21GiftToken(Serial serial)
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
