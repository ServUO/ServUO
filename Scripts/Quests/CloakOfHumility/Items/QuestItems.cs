using Server.Items;
namespace Server.Engines.Quests
{
    public class IronChain : Item
    {
        public override int LabelNumber => 1075788;  // Iron Chain

        [Constructable]
        public IronChain() : base(0x1A07)
        {
        }

        public IronChain(Serial serial)
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

    public class GreyCloak : Cloak
    {
        public override int LabelNumber => 1075789;  // A Plain Grey Cloak

        [Constructable]
        public GreyCloak()
        {
        }

        public GreyCloak(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                reader.ReadMobile();
        }
    }

    public class SeasonedSkillet : Item
    {
        public override int LabelNumber => 1075774;  // Seasoned Skillet

        [Constructable]
        public SeasonedSkillet() : base(0x097F)
        {
        }

        public SeasonedSkillet(Serial serial)
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

    public class VillageCauldron : Item
    {
        public override int LabelNumber => 1075775;  // Village Cauldron

        [Constructable]
        public VillageCauldron()
            : base(Utility.RandomMinMax(0x0974, 0x0975))
        {
        }

        public VillageCauldron(Serial serial)
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

    public class ShortStool : Stool
    {
        public override int LabelNumber => 1075776;  // Short Stool

        [Constructable]
        public ShortStool()
        {
        }

        public ShortStool(Serial serial)
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

    public class FriendshipMug : CeramicMug
    {
        public override int LabelNumber => 1075777;  // Friendship Mug

        [Constructable]
        public FriendshipMug()
        {
        }

        public FriendshipMug(Serial serial)
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

    public class BrassRing : GoldRing
    {
        public override int LabelNumber => 1075778;  // Brass Ring

        [Constructable]
        public BrassRing()
        {
        }

        public BrassRing(Serial serial)
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

    public class WornHammer : Item
    {
        public override int LabelNumber => 1075779;  // Worn Hammer

        [Constructable]
        public WornHammer() : base(0x102A)
        {
        }

        public WornHammer(Serial serial)
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

    public class PairOfWorkGloves : LeatherGloves
    {
        public override int LabelNumber => 1075780;  // Pair of Work Gloves

        [Constructable]
        public PairOfWorkGloves()
        {
        }

        public PairOfWorkGloves(Serial serial)
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