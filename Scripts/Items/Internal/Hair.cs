namespace Server.Items
{
    public abstract class Hair : Item
    {
        public Hair(Serial serial)
            : base(serial)
        {
        }

        protected Hair(int itemID)
            : this(itemID, 0)
        {
        }

        protected Hair(int itemID, int hue)
            : base(itemID)
        {
            LootType = LootType.Blessed;
            Layer = Layer.Hair;
            Hue = hue;
        }

        public override bool DisplayLootType => false;
		
        public override bool VerifyMove(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            parent.HairItemID = ItemID;
            parent.HairHue = Hue;

            return DeathMoveResult.MoveToCorpse;
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

    public class GenericHair : Hair
    {
        public GenericHair(Serial serial)
            : base(serial)
        {
        }

        private GenericHair(int itemID)
            : this(itemID, 0)
        {
        }

        private GenericHair(int itemID, int hue)
            : base(itemID, hue)
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

    public class Mohawk : Hair
    {
        public Mohawk(Serial serial)
            : base(serial)
        {
        }

        private Mohawk()
            : this(0)
        {
        }

        private Mohawk(int hue)
            : base(0x2044, hue)
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

    public class PageboyHair : Hair
    {
        public PageboyHair(Serial serial)
            : base(serial)
        {
        }

        private PageboyHair()
            : this(0)
        {
        }

        private PageboyHair(int hue)
            : base(0x2045, hue)
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

    public class BunsHair : Hair
    {
        public BunsHair(Serial serial)
            : base(serial)
        {
        }

        private BunsHair()
            : this(0)
        {
        }

        private BunsHair(int hue)
            : base(0x2046, hue)
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

    public class LongHair : Hair
    {
        public LongHair(Serial serial)
            : base(serial)
        {
        }

        private LongHair()
            : this(0)
        {
        }

        private LongHair(int hue)
            : base(0x203C, hue)
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

    public class ShortHair : Hair
    {
        public ShortHair(Serial serial)
            : base(serial)
        {
        }

        private ShortHair()
            : this(0)
        {
        }

        private ShortHair(int hue)
            : base(0x203B, hue)
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

    public class PonyTail : Hair
    {
        public PonyTail(Serial serial)
            : base(serial)
        {
        }

        private PonyTail()
            : this(0)
        {
        }

        private PonyTail(int hue)
            : base(0x203D, hue)
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

    public class Afro : Hair
    {
        public Afro(Serial serial)
            : base(serial)
        {
        }

        private Afro()
            : this(0)
        {
        }

        private Afro(int hue)
            : base(0x2047, hue)
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

    public class ReceedingHair : Hair
    {
        public ReceedingHair(Serial serial)
            : base(serial)
        {
        }

        private ReceedingHair()
            : this(0)
        {
        }

        private ReceedingHair(int hue)
            : base(0x2048, hue)
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

    public class TwoPigTails : Hair
    {
        public TwoPigTails(Serial serial)
            : base(serial)
        {
        }

        private TwoPigTails()
            : this(0)
        {
        }

        private TwoPigTails(int hue)
            : base(0x2049, hue)
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

    public class KrisnaHair : Hair
    {
        public KrisnaHair(Serial serial)
            : base(serial)
        {
        }

        private KrisnaHair()
            : this(0)
        {
        }

        private KrisnaHair(int hue)
            : base(0x204A, hue)
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