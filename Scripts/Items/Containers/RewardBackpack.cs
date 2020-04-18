
using Server.Engines.Quests;

namespace Server.Items
{
    public class BaseRewardBackpack : Backpack
    {
        public virtual int Level => 1;

        public BaseRewardBackpack()
            : base()
        {
            Hue = 1127;

            DropItem(new Gold(Level * 2000));
            DropItem(new TerMurQuestRewardBook());

            int itemDrop;

            switch (Level)
            {
                case 3: itemDrop = 5; break;
                default: itemDrop = 3; break;
            }

            for (int i = 0; i < itemDrop; i++)
            {
                Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(false, false, true);

                if (item != null)
                {
                    BaseReward.ApplyMods(item);
                    DropItem(item);
                }
            }

            itemDrop = Utility.RandomMinMax(2, 3);

            for (int i = 0; i < itemDrop; i++)
            {
                if (Level == 1)
                {
                    DropItemStacked(Loot.RandomGem());
                }
                else
                {
                    DropItemStacked(Loot.RandomRareGem());
                }
            }

            switch (Level)
            {
                case 1: DropItem(new MagicalResidue(20)); break;
                case 2: DropItem(new EnchantedEssence(10)); break;
                case 3: DropItem(new RelicFragment()); break;
            }
        }

        public BaseRewardBackpack(Serial serial)
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

            reader.ReadInt();
        }
    }

    public class DustyAdventurersBackpack : BaseRewardBackpack
    {
        public override int LabelNumber => 1113189;  // Dusty Adventurer's Backpack

        [Constructable]
        public DustyAdventurersBackpack()
            : base()
        {
        }

        public DustyAdventurersBackpack(Serial serial)
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

            reader.ReadInt();
        }
    }

    public class DustyExplorersBackpack : BaseRewardBackpack
    {
        public override int LabelNumber => 1113190;  // Dusty Explorer's Backpack
        public override int Level => 2;

        [Constructable]
        public DustyExplorersBackpack()
            : base()
        {
        }

        public DustyExplorersBackpack(Serial serial)
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

            reader.ReadInt();
        }
    }

    public class DustyHuntersBackpack : BaseRewardBackpack
    {
        public override int LabelNumber => 1113191;  // Dusty Hunter's Backpack
        public override int Level => 3;

        [Constructable]
        public DustyHuntersBackpack()
            : base()
        {
        }

        public DustyHuntersBackpack(Serial serial)
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

            reader.ReadInt();
        }
    }
}
