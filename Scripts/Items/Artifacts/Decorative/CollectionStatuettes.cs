using Server.Network;

namespace Server.Items
{
    public class CollectionStatuette : BaseStatuette
    {
        public override bool IsArtifact => true;
        public CollectionStatuette(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public CollectionStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (TurnedOn && IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, Location, 2) && !Utility.InRange(oldLocation, Location, 2))
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.RandomMinMax(1073207, 1073216));
                Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x5A, 0xE9));
            }

            base.OnMovement(m, oldLocation);
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

    public class SilverSteedZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SilverSteedZooStatuette()
            : base(0x259D)
        {
        }

        public SilverSteedZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073219;// Interactive Silver Steed Contribution Statue
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

    public class QuagmireZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public QuagmireZooStatuette()
            : base(0x2614)
        {
        }

        public QuagmireZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074848;// Interactive Quagmire Contribution Statue from the Britannia Royal Zoo.
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

    public class BakeKitsuneZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BakeKitsuneZooStatuette()
            : base(0x2763)
        {
        }

        public BakeKitsuneZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074849;// Interactive Bake Kitsune Contribution Statue from the Britannia Royal Zoo.
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

    public class DireWolfZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DireWolfZooStatuette()
            : base(0x25D0)
        {
        }

        public DireWolfZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073196;// Interactive Dire Wolf Contribution Statue from the Britannia Royal Zoo.
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

    public class CraneZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public CraneZooStatuette()
            : base(0x2764)
        {
        }

        public CraneZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073197;// An Interactive Crane Contribution Statue from the Britannia Royal Zoo.
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

    public class PolarBearZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PolarBearZooStatuette()
            : base(0x20E1)
        {
        }

        public PolarBearZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074851;// Interactive Polar Bear Contribution Statue from the Britannia Royal Zoo.
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

    public class ChangelingZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ChangelingZooStatuette()
            : base(0x2D8A)
        {
        }

        public ChangelingZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074850;// Interactive Changeling Contribution Statue from the Britannia Royal Zoo.
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

    public class ReptalonZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ReptalonZooStatuette()
            : base(0x2D95)
        {
        }

        public ReptalonZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074852;// Interactive Reptalon Contribution Statue from the Britannia Royal Zoo.
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

    public class SpecialAchievementZooStatuette : CollectionStatuette
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SpecialAchievementZooStatuette()
            : base(0x2FF6)
        {
            Weight = 10.0;
        }

        public SpecialAchievementZooStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073226;// Britannia Royal Zoo Special Achievement Award
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