using System;

namespace Server.Items
{
    public class Rope : Item
    {
        [Constructable]
        public Rope()
            : this(1)
        {
        }

        [Constructable]
        public Rope(int amount)
            : base(0x14F8)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public Rope(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class IronWire : Item
    {
        [Constructable]
        public IronWire()
            : this(1)
        {
        }

        [Constructable]
        public IronWire(int amount)
            : base(0x1876)
        {
            this.Stackable = true;
            this.Weight = 5.0;
            this.Amount = amount;
        }

        public IronWire(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.Weight == 2.0)
                this.Weight = 5.0;
        }
    }

    public class SilverWire : Item
    {
        [Constructable]
        public SilverWire()
            : this(1)
        {
        }

        [Constructable]
        public SilverWire(int amount)
            : base(0x1877)
        {
            this.Stackable = true;
            this.Weight = 5.0;
            this.Amount = amount;
        }

        public SilverWire(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.Weight == 2.0)
                this.Weight = 5.0;
        }
    }

    public class GoldWire : Item
    {
        [Constructable]
        public GoldWire()
            : this(1)
        {
        }

        [Constructable]
        public GoldWire(int amount)
            : base(0x1878)
        {
            this.Stackable = true;
            this.Weight = 5.0;
            this.Amount = amount;
        }

        public GoldWire(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.Weight == 2.0)
                this.Weight = 5.0;
        }
    }

    public class CopperWire : Item
    {
        [Constructable]
        public CopperWire()
            : this(1)
        {
        }

        [Constructable]
        public CopperWire(int amount)
            : base(0x1879)
        {
            this.Stackable = true;
            this.Weight = 5.0;
            this.Amount = amount;
        }

        public CopperWire(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1 && this.Weight == 2.0)
                this.Weight = 5.0;
        }
    }

    public class WhiteDriedFlowers : Item
    {
        [Constructable]
        public WhiteDriedFlowers()
            : this(1)
        {
        }

        [Constructable]
        public WhiteDriedFlowers(int amount)
            : base(0xC3C)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public WhiteDriedFlowers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GreenDriedFlowers : Item
    {
        [Constructable]
        public GreenDriedFlowers()
            : this(1)
        {
        }

        [Constructable]
        public GreenDriedFlowers(int amount)
            : base(0xC3E)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public GreenDriedFlowers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DriedOnions : Item
    {
        [Constructable]
        public DriedOnions()
            : this(1)
        {
        }

        [Constructable]
        public DriedOnions(int amount)
            : base(0xC40)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public DriedOnions(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DriedHerbs : Item
    {
        [Constructable]
        public DriedHerbs()
            : this(1)
        {
        }

        [Constructable]
        public DriedHerbs(int amount)
            : base(0xC42)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public DriedHerbs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HorseShoes : Item
    {
        [Constructable]
        public HorseShoes()
            : base(0xFB6)
        {
            this.Weight = 3.0;
        }

        public HorseShoes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ForgedMetal : Item
    {
        [Constructable]
        public ForgedMetal()
            : base(0xFB8)
        {
            this.Weight = 5.0;
        }

        public ForgedMetal(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Whip : Item
    {
        [Constructable]
        public Whip()
            : base(0x166E)
        {
            this.Weight = 1.0;
        }

        public Whip(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PaintsAndBrush : Item
    {
        [Constructable]
        public PaintsAndBrush()
            : base(0xFC1)
        {
            this.Weight = 1.0;
        }

        public PaintsAndBrush(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PenAndInk : Item
    {
        [Constructable]
        public PenAndInk()
            : base(0xFBF)
        {
            this.Weight = 1.0;
        }

        public PenAndInk(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ChiselsNorth : Item
    {
        [Constructable]
        public ChiselsNorth()
            : base(0x1026)
        {
            this.Weight = 1.0;
        }

        public ChiselsNorth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class ChiselsWest : Item
    {
        [Constructable]
        public ChiselsWest()
            : base(0x1027)
        {
            this.Weight = 1.0;
        }

        public ChiselsWest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtyPan : Item
    {
        [Constructable]
        public DirtyPan()
            : base(0x9E8)
        {
            this.Weight = 1.0;
        }

        public DirtyPan(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtySmallRoundPot : Item
    {
        [Constructable]
        public DirtySmallRoundPot()
            : base(0x9E7)
        {
            this.Weight = 1.0;
        }

        public DirtySmallRoundPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtyPot : Item
    {
        [Constructable]
        public DirtyPot()
            : base(0x9E6)
        {
            this.Weight = 1.0;
        }

        public DirtyPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtyRoundPot : Item
    {
        [Constructable]
        public DirtyRoundPot()
            : base(0x9DF)
        {
            this.Weight = 1.0;
        }

        public DirtyRoundPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtyFrypan : Item
    {
        [Constructable]
        public DirtyFrypan()
            : base(0x9DE)
        {
            this.Weight = 1.0;
        }

        public DirtyFrypan(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtySmallPot : Item
    {
        [Constructable]
        public DirtySmallPot()
            : base(0x9DD)
        {
            this.Weight = 1.0;
        }

        public DirtySmallPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DirtyKettle : Item
    {
        [Constructable]
        public DirtyKettle()
            : base(0x9DC)
        {
            this.Weight = 1.0;
        }

        public DirtyKettle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}