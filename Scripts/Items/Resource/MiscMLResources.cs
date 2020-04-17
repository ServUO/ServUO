namespace Server.Items
{
    public class Blight : Item, ICommodity
    {
        [Constructable]
        public Blight()
            : this(1)
        {
        }

        [Constructable]
        public Blight(int amount)
            : base(0x3183)
        {
            Stackable = true;
            Amount = amount;
        }

        public Blight(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class LuminescentFungi : Item, ICommodity
    {
        [Constructable]
        public LuminescentFungi()
            : this(1)
        {
        }

        [Constructable]
        public LuminescentFungi(int amount)
            : base(0x3191)
        {
            Stackable = true;
            Amount = amount;
        }

        public LuminescentFungi(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class CapturedEssence : Item, ICommodity
    {
        [Constructable]
        public CapturedEssence()
            : this(1)
        {
        }

        [Constructable]
        public CapturedEssence(int amount)
            : base(0x318E)
        {
            Stackable = true;
            Amount = amount;
        }

        public CapturedEssence(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class EyeOfTheTravesty : Item, ICommodity
    {
        [Constructable]
        public EyeOfTheTravesty()
            : this(1)
        {
        }

        [Constructable]
        public EyeOfTheTravesty(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public EyeOfTheTravesty(int amount)
            : base(0x318D)
        {
            Stackable = true;
            Amount = amount;
        }

        public EyeOfTheTravesty(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class Corruption : Item, ICommodity
    {
        [Constructable]
        public Corruption()
            : this(1)
        {
        }

        [Constructable]
        public Corruption(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Corruption(int amount)
            : base(0x3184)
        {
            Stackable = true;
            Amount = amount;
        }

        public Corruption(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class DreadHornMane : Item, ICommodity
    {
        [Constructable]
        public DreadHornMane()
            : this(1)
        {
        }

        [Constructable]
        public DreadHornMane(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public DreadHornMane(int amount)
            : base(0x318A)
        {
            Stackable = true;
            Amount = amount;
        }

        public DreadHornMane(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class ParasiticPlant : Item, ICommodity
    {
        [Constructable]
        public ParasiticPlant()
            : this(1)
        {
        }

        [Constructable]
        public ParasiticPlant(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public ParasiticPlant(int amount)
            : base(0x3190)
        {
            Stackable = true;
            Amount = amount;
        }

        public ParasiticPlant(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class Muculent : Item, ICommodity
    {
        [Constructable]
        public Muculent()
            : this(1)
        {
        }

        [Constructable]
        public Muculent(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Muculent(int amount)
            : base(0x3188)
        {
            Stackable = true;
            Amount = amount;
        }

        public Muculent(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class DiseasedBark : Item, ICommodity
    {
        [Constructable]
        public DiseasedBark()
            : this(1)
        {
        }

        [Constructable]
        public DiseasedBark(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public DiseasedBark(int amount)
            : base(0x318B)
        {
            Stackable = true;
            Amount = amount;
        }

        public DiseasedBark(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class BarkFragment : Item, ICommodity
    {
        [Constructable]
        public BarkFragment()
            : this(1)
        {
        }

        [Constructable]
        public BarkFragment(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public BarkFragment(int amount)
            : base(0x318F)
        {
            Stackable = true;
            Amount = amount;
        }

        public BarkFragment(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class GrizzledBones : Item, ICommodity
    {
        [Constructable]
        public GrizzledBones()
            : this(1)
        {
        }

        [Constructable]
        public GrizzledBones(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public GrizzledBones(int amount)
            : base(0x318C)
        {
            Stackable = true;
            Amount = amount;
        }

        public GrizzledBones(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version <= 0 && ItemID == 0x318F)
                ItemID = 0x318C;
        }
    }

    public class LardOfParoxysmus : Item, ICommodity
    {
        [Constructable]
        public LardOfParoxysmus()
            : this(1)
        {
        }

        [Constructable]
        public LardOfParoxysmus(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public LardOfParoxysmus(int amount)
            : base(0x3189)
        {
            Stackable = true;
            Amount = amount;
        }

        public LardOfParoxysmus(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class PerfectEmerald : Item, ICommodity
    {
        [Constructable]
        public PerfectEmerald()
            : this(1)
        {
        }

        [Constructable]
        public PerfectEmerald(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public PerfectEmerald(int amount)
            : base(0x3194)
        {
            Stackable = true;
            Amount = amount;
        }

        public PerfectEmerald(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class DarkSapphire : Item, ICommodity
    {
        [Constructable]
        public DarkSapphire()
            : this(1)
        {
        }

        [Constructable]
        public DarkSapphire(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public DarkSapphire(int amount)
            : base(0x3192)
        {
            Stackable = true;
            Amount = amount;
        }

        public DarkSapphire(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class Turquoise : Item, ICommodity
    {
        [Constructable]
        public Turquoise()
            : this(1)
        {
        }

        [Constructable]
        public Turquoise(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Turquoise(int amount)
            : base(0x3193)
        {
            Stackable = true;
            Amount = amount;
        }

        public Turquoise(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class EcruCitrine : Item, ICommodity
    {
        [Constructable]
        public EcruCitrine()
            : this(1)
        {
        }

        [Constructable]
        public EcruCitrine(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public EcruCitrine(int amount)
            : base(0x3195)
        {
            Stackable = true;
            Amount = amount;
        }

        public EcruCitrine(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class WhitePearl : Item, ICommodity
    {
        [Constructable]
        public WhitePearl()
            : this(1)
        {
        }

        [Constructable]
        public WhitePearl(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public WhitePearl(int amount)
            : base(0x3196)
        {
            Stackable = true;
            Amount = amount;
        }

        public WhitePearl(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class FireRuby : Item, ICommodity
    {
        [Constructable]
        public FireRuby()
            : this(1)
        {
        }

        [Constructable]
        public FireRuby(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public FireRuby(int amount)
            : base(0x3197)
        {
            Stackable = true;
            Amount = amount;
        }

        public FireRuby(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class BlueDiamond : Item, ICommodity
    {
        [Constructable]
        public BlueDiamond()
            : this(1)
        {
        }

        [Constructable]
        public BlueDiamond(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public BlueDiamond(int amount)
            : base(0x3198)
        {
            Stackable = true;
            Amount = amount;
        }

        public BlueDiamond(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class BrilliantAmber : Item, ICommodity
    {
        [Constructable]
        public BrilliantAmber()
            : this(1)
        {
        }

        [Constructable]
        public BrilliantAmber(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public BrilliantAmber(int amount)
            : base(0x3199)
        {
            Stackable = true;
            Amount = amount;
        }

        public BrilliantAmber(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class Scourge : Item, ICommodity
    {
        [Constructable]
        public Scourge()
            : this(1)
        {
        }

        [Constructable]
        public Scourge(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Scourge(int amount)
            : base(0x3185)
        {
            Stackable = true;
            Amount = amount;
            Hue = 150;
        }

        public Scourge(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    [TypeAlias("Server.Items.Putrefication")]
    public class Putrefaction : Item, ICommodity
    {
        [Constructable]
        public Putrefaction()
            : this(1)
        {
        }

        [Constructable]
        public Putrefaction(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Putrefaction(int amount)
            : base(0x3186)
        {
            Stackable = true;
            Amount = amount;
            Hue = 883;
        }

        public Putrefaction(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class Taint : Item, ICommodity
    {
        [Constructable]
        public Taint()
            : this(1)
        {
        }

        [Constructable]
        public Taint(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Taint(int amount)
            : base(0x3187)
        {
            Stackable = true;
            Amount = amount;
            Hue = 731;
        }

        public Taint(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    [Flipable(0x315A, 0x315B)]
    public class PristineDreadHorn : Item, ICommodity
    {
        [Constructable]
        public PristineDreadHorn()
            : base(0x315A)
        {
        }

        public PristineDreadHorn(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class SwitchItem : Item, ICommodity
    {
        [Constructable]
        public SwitchItem()
            : this(1)
        {
        }

        [Constructable]
        public SwitchItem(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public SwitchItem(int amount)
            : base(0x2F5F)
        {
            Stackable = true;
            Amount = amount;
        }

        public SwitchItem(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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
