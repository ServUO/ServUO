using System;

namespace Server.Items
{
    [FlipableAttribute(0x6380, 0x639B)]
    public class AOLLegendsShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AOLLegendsShield()
            : base(0x6380)
        {
            this.Movable = false;
        }

        public AOLLegendsShield(Serial serial)
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

    [FlipableAttribute(0x6381, 0x639C)]
    public class ArirangShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ArirangShield()
            : base(0x6381)
        {
            this.Movable = false;
        }

        public ArirangShield(Serial serial)
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

    [FlipableAttribute(0x6382, 0x639D)]
    public class AsukaShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AsukaShield()
            : base(0x6382)
        {
            this.Movable = false;
        }

        public AsukaShield(Serial serial)
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

    [FlipableAttribute(0x6383, 0x639E)]
    public class AlanticShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AlanticShield()
            : base(0x6383)
        {
            this.Movable = false;
        }

        public AlanticShield(Serial serial)
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

    [FlipableAttribute(0x6384, 0x639F)]
    public class BajaShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BajaShield()
            : base(0x6384)
        {
            this.Movable = false;
        }

        public BajaShield(Serial serial)
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

    [FlipableAttribute(0x6385, 0x63A0)]
    public class BalhaeShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BalhaeShield()
            : base(0x6385)
        {
            this.Movable = false;
        }

        public BalhaeShield(Serial serial)
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

    [FlipableAttribute(0x6386, 0x63A1)]
    public class CatskillsShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public CatskillsShield()
            : base(0x6386)
        {
            this.Movable = false;
        }

        public CatskillsShield(Serial serial)
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

    [FlipableAttribute(0x6387, 0x63A2)]
    public class ChesapeakeShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ChesapeakeShield()
            : base(0x6387)
        {
            this.Movable = false;
        }

        public ChesapeakeShield(Serial serial)
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

    [FlipableAttribute(0x6388, 0x63A3)]
    public class DrachenfelsShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DrachenfelsShield()
            : base(0x6388)
        {
            this.Movable = false;
        }

        public DrachenfelsShield(Serial serial)
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

    [FlipableAttribute(0x6389, 0x63A4)]
    public class EuropaShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public EuropaShield()
            : base(0x6389)
        {
            this.Movable = false;
        }

        public EuropaShield(Serial serial)
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

    [FlipableAttribute(0x638A, 0x63A5)]
    public class FormosaShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FormosaShield()
            : base(0x638A)
        {
            this.Movable = false;
        }

        public FormosaShield(Serial serial)
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

    [FlipableAttribute(0x638B, 0x63A6)]
    public class GreatLakesShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GreatLakesShield()
            : base(0x638B)
        {
            this.Movable = false;
        }

        public GreatLakesShield(Serial serial)
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

    [FlipableAttribute(0x638C, 0x63A7)]
    public class HokutoShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public HokutoShield()
            : base(0x638C)
        {
            this.Movable = false;
        }

        public HokutoShield(Serial serial)
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

    [FlipableAttribute(0x638D, 0x63A8)]
    public class IzumoShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public IzumoShield()
            : base(0x638D)
        {
            this.Movable = false;
        }

        public IzumoShield(Serial serial)
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

    [FlipableAttribute(0x638E, 0x63A9)]
    public class LakeAustinShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public LakeAustinShield()
            : base(0x638E)
        {
            this.Movable = false;
        }

        public LakeAustinShield(Serial serial)
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

    [FlipableAttribute(0x638F, 0x63AA)]
    public class LakeSuperiorShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public LakeSuperiorShield()
            : base(0x638F)
        {
            this.Movable = false;
        }

        public LakeSuperiorShield(Serial serial)
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

    [FlipableAttribute(0x6390, 0x63AB)]
    public class MizuhoShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MizuhoShield()
            : base(0x6390)
        {
            this.Movable = false;
        }

        public MizuhoShield(Serial serial)
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

    [FlipableAttribute(0x6391, 0x63AC)]
    public class MugenShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MugenShield()
            : base(0x6391)
        {
            this.Movable = false;
        }

        public MugenShield(Serial serial)
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

    [FlipableAttribute(0x6392, 0x63AD)]
    public class NapaValleyShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public NapaValleyShield()
            : base(0x6392)
        {
            this.Movable = false;
        }

        public NapaValleyShield(Serial serial)
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

    [FlipableAttribute(0x6393, 0x63AE)]
    public class OceaniaShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public OceaniaShield()
            : base(0x6393)
        {
            this.Movable = false;
        }

        public OceaniaShield(Serial serial)
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

    [FlipableAttribute(0x6394, 0x63AF)]
    public class OrginShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public OrginShield()
            : base(0x6394)
        {
            this.Movable = false;
        }

        public OrginShield(Serial serial)
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

    [FlipableAttribute(0x6395, 0x63B0)]
    public class PacificShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public PacificShield()
            : base(0x6395)
        {
            this.Movable = false;
        }

        public PacificShield(Serial serial)
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

    [FlipableAttribute(0x6396, 0x63B1)]
    public class SakuraShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SakuraShield()
            : base(0x6396)
        {
            this.Movable = false;
        }

        public SakuraShield(Serial serial)
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

    [FlipableAttribute(0x6397, 0x63B2)]
    public class SiegePerilousShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SiegePerilousShield()
            : base(0x6397)
        {
            this.Movable = false;
        }

        public SiegePerilousShield(Serial serial)
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

    [FlipableAttribute(0x6398, 0x63B3)]
    public class SonomaShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SonomaShield()
            : base(0x6398)
        {
            this.Movable = false;
        }

        public SonomaShield(Serial serial)
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

    [FlipableAttribute(0x6399, 0x63B4)]
    public class WakokuShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public WakokuShield()
            : base(0x6399)
        {
            this.Movable = false;
        }

        public WakokuShield(Serial serial)
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

    [FlipableAttribute(0x639A, 0x63B5)]
    public class YamatoShield : Item
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public YamatoShield()
            : base(0x639A)
        {
            this.Movable = false;
        }

        public YamatoShield(Serial serial)
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