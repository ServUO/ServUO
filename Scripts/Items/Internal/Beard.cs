namespace Server.Items
{
    public abstract class Beard : Item
    {
        public Beard(Serial serial)
            : base(serial)
        {
        }

        protected Beard(int itemID)
            : this(itemID, 0)
        {
        }

        protected Beard(int itemID, int hue)
            : base(itemID)
        {
            LootType = LootType.Blessed;
            Layer = Layer.FacialHair;
            Hue = hue;
        }

        public override bool DisplayLootType => false;
		
        public override bool VerifyMove(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            parent.FacialHairItemID = ItemID;
            parent.FacialHairHue = Hue;

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

    public class GenericBeard : Beard
    {
        public GenericBeard(Serial serial)
            : base(serial)
        {
        }

        private GenericBeard(int itemID)
            : this(itemID, 0)
        {
        }

        private GenericBeard(int itemID, int hue)
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

    public class LongBeard : Beard
    {
        public LongBeard(Serial serial)
            : base(serial)
        {
        }

        private LongBeard()
            : this(0)
        {
        }

        private LongBeard(int hue)
            : base(0x203E, hue)
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

    public class ShortBeard : Beard
    {
        public ShortBeard(Serial serial)
            : base(serial)
        {
        }

        private ShortBeard()
            : this(0)
        {
        }

        private ShortBeard(int hue)
            : base(0x203f, hue)
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

    public class Goatee : Beard
    {
        public Goatee(Serial serial)
            : base(serial)
        {
        }

        private Goatee()
            : this(0)
        {
        }

        private Goatee(int hue)
            : base(0x2040, hue)
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

    public class Mustache : Beard
    {
        public Mustache(Serial serial)
            : base(serial)
        {
        }

        private Mustache()
            : this(0)
        {
        }

        private Mustache(int hue)
            : base(0x2041, hue)
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

    public class MediumShortBeard : Beard
    {
        public MediumShortBeard(Serial serial)
            : base(serial)
        {
        }

        private MediumShortBeard()
            : this(0)
        {
        }

        private MediumShortBeard(int hue)
            : base(0x204B, hue)
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

    public class MediumLongBeard : Beard
    {
        public MediumLongBeard(Serial serial)
            : base(serial)
        {
        }

        private MediumLongBeard()
            : this(0)
        {
        }

        private MediumLongBeard(int hue)
            : base(0x204C, hue)
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

    public class Vandyke : Beard
    {
        public Vandyke(Serial serial)
            : base(serial)
        {
        }

        private Vandyke()
            : this(0)
        {
        }

        private Vandyke(int hue)
            : base(0x204D, hue)
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