namespace Server.Items
{
    public class ArcaneCircleScroll : SpellScroll
    {
        [Constructable]
        public ArcaneCircleScroll()
            : this(1)
        {
        }

        [Constructable]
        public ArcaneCircleScroll(int amount)
            : base(600, 0x2D51, amount)
        {
            Hue = 0x8FD;
        }

        public ArcaneCircleScroll(Serial serial)
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

    public class GiftOfRenewalScroll : SpellScroll
    {
        [Constructable]
        public GiftOfRenewalScroll()
            : this(1)
        {
        }

        [Constructable]
        public GiftOfRenewalScroll(int amount)
            : base(601, 0x2D52, amount)
        {
            Hue = 0x8FD;
        }

        public GiftOfRenewalScroll(Serial serial)
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

    public class ImmolatingWeaponScroll : SpellScroll
    {
        [Constructable]
        public ImmolatingWeaponScroll()
            : this(1)
        {
        }

        [Constructable]
        public ImmolatingWeaponScroll(int amount)
            : base(602, 0x2D53, amount)
        {
            Hue = 0x8FD;
        }

        public ImmolatingWeaponScroll(Serial serial)
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

    public class AttuneWeaponScroll : SpellScroll
    {
        [Constructable]
        public AttuneWeaponScroll()
            : this(1)
        {
        }

        [Constructable]
        public AttuneWeaponScroll(int amount)
            : base(603, 0x2D54, amount)
        {
            Hue = 0x8FD;
        }

        public AttuneWeaponScroll(Serial serial)
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

    public class ThunderstormScroll : SpellScroll
    {
        [Constructable]
        public ThunderstormScroll()
            : this(1)
        {
        }

        [Constructable]
        public ThunderstormScroll(int amount)
            : base(604, 0x2D55, amount)
        {
            Hue = 0x8FD;
        }

        public ThunderstormScroll(Serial serial)
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

    public class NatureFuryScroll : SpellScroll
    {
        [Constructable]
        public NatureFuryScroll()
            : this(1)
        {
        }

        [Constructable]
        public NatureFuryScroll(int amount)
            : base(605, 0x2D56, amount)
        {
            Hue = 0x8FD;
        }

        public NatureFuryScroll(Serial serial)
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

    public class SummonFeyScroll : SpellScroll
    {
        [Constructable]
        public SummonFeyScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonFeyScroll(int amount)
            : base(606, 0x2D57, amount)
        {
            Hue = 0x8FD;
        }

        public SummonFeyScroll(Serial serial)
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

    public class SummonFiendScroll : SpellScroll
    {
        [Constructable]
        public SummonFiendScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonFiendScroll(int amount)
            : base(607, 0x2D58, amount)
        {
            Hue = 0x8FD;
        }

        public SummonFiendScroll(Serial serial)
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

    public class ReaperFormScroll : SpellScroll
    {
        [Constructable]
        public ReaperFormScroll()
            : this(1)
        {
        }

        [Constructable]
        public ReaperFormScroll(int amount)
            : base(608, 0x2D59, amount)
        {
            Hue = 0x8FD;
        }

        public ReaperFormScroll(Serial serial)
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

    public class WildfireScroll : SpellScroll
    {
        [Constructable]
        public WildfireScroll()
            : this(1)
        {
        }

        [Constructable]
        public WildfireScroll(int amount)
            : base(609, 0x2D5A, amount)
        {
            Hue = 0x8FD;
        }

        public WildfireScroll(Serial serial)
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

    public class EssenceOfWindScroll : SpellScroll
    {
        [Constructable]
        public EssenceOfWindScroll()
            : this(1)
        {
        }

        [Constructable]
        public EssenceOfWindScroll(int amount)
            : base(610, 0x2D5B, amount)
        {
            Hue = 0x8FD;
        }

        public EssenceOfWindScroll(Serial serial)
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

    public class DryadAllureScroll : SpellScroll
    {
        [Constructable]
        public DryadAllureScroll()
            : this(1)
        {
        }

        [Constructable]
        public DryadAllureScroll(int amount)
            : base(611, 0x2D5C, amount)
        {
            Hue = 0x8FD;
        }

        public DryadAllureScroll(Serial serial)
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

    public class EtherealVoyageScroll : SpellScroll
    {
        [Constructable]
        public EtherealVoyageScroll()
            : this(1)
        {
        }

        [Constructable]
        public EtherealVoyageScroll(int amount)
            : base(612, 0x2D5D, amount)
        {
            Hue = 0x8FD;
        }

        public EtherealVoyageScroll(Serial serial)
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

    public class WordOfDeathScroll : SpellScroll
    {
        [Constructable]
        public WordOfDeathScroll()
            : this(1)
        {
        }

        [Constructable]
        public WordOfDeathScroll(int amount)
            : base(613, 0x2D5E, amount)
        {
            Hue = 0x8FD;
        }

        public WordOfDeathScroll(Serial serial)
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

    public class GiftOfLifeScroll : SpellScroll
    {
        [Constructable]
        public GiftOfLifeScroll()
            : this(1)
        {
        }

        [Constructable]
        public GiftOfLifeScroll(int amount)
            : base(614, 0x2D5F, amount)
        {
            Hue = 0x8FD;
        }

        public GiftOfLifeScroll(Serial serial)
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

    public class ArcaneEmpowermentScroll : SpellScroll
    {
        [Constructable]
        public ArcaneEmpowermentScroll()
            : this(1)
        {
        }

        [Constructable]
        public ArcaneEmpowermentScroll(int amount)
            : base(615, 0x2D60, amount)
        {
            Hue = 0x8FD;
        }

        public ArcaneEmpowermentScroll(Serial serial)
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