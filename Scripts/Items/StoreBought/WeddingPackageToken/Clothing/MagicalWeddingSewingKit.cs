using Server.Gumps;

namespace Server.Items
{
    public class MagicalWeddingSewingKit : Item, IRewardOption
    {
        public override int LabelNumber => 1157338; // Magical Wedding Sewing Kit

        [Constructable]
        public MagicalWeddingSewingKit()
            : base(0x0F9D)
        {
            Weight = 2;
            LootType = LootType.Blessed;
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, 1157333); // Bride Set 
            list.Add(2, 1157334); // Groom Set
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            Bag bag = new Bag();

            switch (choice)
            {
                default:
                    break;
                case 1:
                    {
                        bag.DropItem(new WeddingDress());
                        bag.DropItem(new FemaleTopper());
                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }
                case 2:
                    {
                        bag.DropItem(new TopHat());
                        bag.DropItem(new TuxedoPants());
                        bag.DropItem(new Tuxedo());
                        bag.DropItem(new MaleTopper());
                        from.AddToBackpack(bag);
                        Delete();
                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1157335)); // Please make a selection:
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public MagicalWeddingSewingKit(Serial serial)
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
            reader.ReadInt();
        }
    }
}
