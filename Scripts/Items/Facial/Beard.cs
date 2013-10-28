using System;

namespace Server.Items
{
    public abstract class Beard : Item
    {
        public Beard(Serial serial)
            : base(serial)
        {
        }

        /*public static Beard CreateByID( int id, int hue )
        {
        switch ( id )
        {
        case 0x203E: return new LongBeard( hue );
        case 0x203F: return new ShortBeard( hue );
        case 0x2040: return new Goatee( hue );
        case 0x2041: return new Mustache( hue );
        case 0x204B: return new MediumShortBeard( hue );
        case 0x204C: return new MediumLongBeard( hue );
        case 0x204D: return new Vandyke( hue );
        default: return new GenericBeard( id, hue );
        }
        }*/
        protected Beard(int itemID)
            : this(itemID, 0)
        {
        }

        protected Beard(int itemID, int hue)
            : base(itemID)
        {
            this.LootType = LootType.Blessed;
            this.Layer = Layer.FacialHair;
            this.Hue = hue;
        }

        public override bool DisplayLootType
        {
            get
            {
                return false;
            }
        }
        public override bool VerifyMove(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            //Dupe( Amount );
            parent.FacialHairItemID = this.ItemID;
            parent.FacialHairHue = this.Hue;

            return DeathMoveResult.MoveToCorpse;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            this.LootType = LootType.Blessed;

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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}