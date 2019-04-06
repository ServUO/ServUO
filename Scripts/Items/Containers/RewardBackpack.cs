using System;

namespace Server.Items
{
    public class BaseRewardBackpack : Backpack
    {
        public BaseRewardBackpack()
            : base()
        {
            this.Hue = 1127;
        }

        public BaseRewardBackpack(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DustyAdventurersBackpack : BaseRewardBackpack
    {
        [Constructable]
        public DustyAdventurersBackpack()
            : base()
        {
            this.AddItem(new MagicalResidue(20));
            this.AddItem(new Amber());
            this.AddItem(new Sapphire());
            this.AddItem(new Gold(2000));
            this.AddItem(new Bow());
            this.AddItem(new GargishBracelet());

            switch (Utility.Random(5))
            {
                case 0:
                    this.AddItem(new LeatherChest());
                    break;
                case 1:
                    this.AddItem(new LeatherArms());
                    break;
                case 2:
                    this.AddItem(new LeatherLegs());
                    break;
                case 3:
                    this.AddItem(new GargishLeatherChest());
                    break;
                case 4:
                    this.AddItem(new GargishLeatherArms());
                    break;
                case 5:
                    this.AddItem(new GargishLeatherLegs());
                    break;
            }
        }

        public DustyAdventurersBackpack(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DustyExplorersBackpack : BaseRewardBackpack
    {
        [Constructable]
        public DustyExplorersBackpack()
            : base()
        {
            this.AddItem(new EnchantedEssence(10));
            this.AddItem(new Amber());
            this.AddItem(new Citrine());
            this.AddItem(new Amethyst());
            this.AddItem(new Gold(4000));
            switch (Utility.Random(4))
            {
                case 0:
                    this.AddItem(new GargishRing());
                    break;
                case 1:
                    this.AddItem(new GargishNecklace());
                    break;
                case 2:
                    this.AddItem(new GargishBracelet());
                    break;
                case 3:
                    this.AddItem(new GargishEarrings());
                    break;
            }

            switch (Utility.Random(6))
            {
                case 0:
                    this.AddItem(new GlassSword());
                    break;
                case 1:
                    this.AddItem(new Katana());
                    break;
                case 2:
                    this.AddItem(new Broadsword());
                    break;
                case 3:
                    this.AddItem(new Mace());
                    break;
                case 4:
                    this.AddItem(new Halberd());
                    break;
                case 5:
                    this.AddItem(new Shortblade());
                    break;
            }
            switch (Utility.Random(6))
            {
                case 0:
                    this.AddItem(new LeatherChest());
                    break;
                case 1:
                    this.AddItem(new LeatherArms());
                    break;
                case 2:
                    this.AddItem(new LeatherLegs());
                    break;
                case 3:
                    this.AddItem(new GargishLeatherChest());
                    break;
                case 4:
                    this.AddItem(new GargishLeatherArms());
                    break;
                case 5:
                    this.AddItem(new GargishLeatherLegs());
                    break;
            }
        }

        public DustyExplorersBackpack(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DustyHuntersBackpack : BaseRewardBackpack
    {
        [Constructable]
        public DustyHuntersBackpack()
            : base()
        {
            this.AddItem(new RelicFragment(1));
            this.AddItem(new Amber());
            this.AddItem(new Ruby());
            this.AddItem(new Diamond());
            this.AddItem(new Gold(6000));

            switch (Utility.Random(4))
            {
                case 0:
                    this.AddItem(new GargishRing());
                    break;
                case 1:
                    this.AddItem(new GargishNecklace());
                    break;
                case 2:
                    this.AddItem(new GargishBracelet());
                    break;
                case 3:
                    this.AddItem(new GargishEarrings());
                    break;
            }

            switch (Utility.Random(6))
            {
                case 0:
                    this.AddItem(new GlassSword());
                    break;
                case 1:
                    this.AddItem(new Katana());
                    break;
                case 2:
                    this.AddItem(new Broadsword());
                    break;
                case 3:
                    this.AddItem(new Mace());
                    break;
                case 4:
                    this.AddItem(new Halberd());
                    break;
                case 5:
                    this.AddItem(new Shortblade());
                    break;
            }
            switch (Utility.Random(6))
            {
                case 0:
                    this.AddItem(new LeatherChest());
                    break;
                case 1:
                    this.AddItem(new LeatherArms());
                    break;
                case 2:
                    this.AddItem(new LeatherLegs());
                    break;
                case 3:
                    this.AddItem(new GargishLeatherChest());
                    break;
                case 4:
                    this.AddItem(new GargishLeatherArms());
                    break;
                case 5:
                    this.AddItem(new GargishLeatherLegs());
                    break;
            }

            this.AddItem(new LeatherTalons());
            this.AddItem(new Boomerang());
        }

        public DustyHuntersBackpack(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}