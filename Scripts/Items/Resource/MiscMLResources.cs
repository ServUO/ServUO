using System;

namespace Server.Items
{
    public class Blight : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public Blight(Serial serial)
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

    public class LuminescentFungi : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public LuminescentFungi(Serial serial)
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

    public class CapturedEssence : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public CapturedEssence(Serial serial)
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

    public class EyeOfTheTravesty : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public EyeOfTheTravesty(Serial serial)
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

    public class Corruption : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public Corruption(Serial serial)
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

    public class DreadHornMane : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public DreadHornMane(Serial serial)
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

    public class ParasiticPlant : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public ParasiticPlant(Serial serial)
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

    public class Muculent : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public Muculent(Serial serial)
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

    public class DiseasedBark : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public DiseasedBark(Serial serial)
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

    public class BarkFragment : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public BarkFragment(Serial serial)
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

    public class GrizzledBones : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public GrizzledBones(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version <= 0 && this.ItemID == 0x318F)
                this.ItemID = 0x318C;
        }
    }

    public class LardOfParoxysmus : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public LardOfParoxysmus(Serial serial)
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

    public class PerfectEmerald : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public PerfectEmerald(Serial serial)
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

    public class DarkSapphire : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public DarkSapphire(Serial serial)
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

    public class Turquoise : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public Turquoise(Serial serial)
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

    public class EcruCitrine : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public EcruCitrine(Serial serial)
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

    public class WhitePearl : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public WhitePearl(Serial serial)
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

    public class FireRuby : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public FireRuby(Serial serial)
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

    public class BlueDiamond : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public BlueDiamond(Serial serial)
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

    public class BrilliantAmber : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public BrilliantAmber(Serial serial)
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

    public class Scourge : Item
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
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 150;
        }

        public Scourge(Serial serial)
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

    public class Putrefication : Item
    {
        [Constructable]
        public Putrefication()
            : this(1)
        {
        }

        [Constructable]
        public Putrefication(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Putrefication(int amount)
            : base(0x3186)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 883;
        }

        public Putrefication(Serial serial)
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

    public class Taint : Item
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
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 731;
        }

        public Taint(Serial serial)
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

    [Flipable(0x315A, 0x315B)]
    public class PristineDreadHorn : Item
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

    public class SwitchItem : Item
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
            this.Stackable = true;
            this.Amount = amount;
        }

        public SwitchItem(Serial serial)
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