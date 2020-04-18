namespace Server.Items
{
    #region Basket1Artifact
    public class Basket1Artifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 1;

        [Constructable]
        public Basket1Artifact()
            : base(0x24DD)
        {
        }

        public Basket1Artifact(Serial serial)
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
    #endregion

    #region Basket2Artifact
    public class Basket2Artifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 1;

        [Constructable]
        public Basket2Artifact()
            : base(0x24D7)
        {
        }

        public Basket2Artifact(Serial serial)
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
    #endregion

    #region Basket3WestArtifact
    public class Basket3WestArtifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 1;

        [Constructable]
        public Basket3WestArtifact()
            : base(0x24D9)
        {
        }

        public Basket3WestArtifact(Serial serial)
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
    #endregion

    #region Basket3NorthArtifact
    public class Basket3NorthArtifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 1;

        [Constructable]
        public Basket3NorthArtifact()
            : base(0x24DA)
        {
        }

        public Basket3NorthArtifact(Serial serial)
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
    #endregion

    #region Basket4Artifact
    public class Basket4Artifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 2;

        [Constructable]
        public Basket4Artifact()
            : base(0x24D8)
        {
        }

        public Basket4Artifact(Serial serial)
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
    #endregion

    #region Basket5WestArtifact
    public class Basket5WestArtifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 2;

        [Constructable]
        public Basket5WestArtifact()
            : base(0x24DC)
        {
        }

        public Basket5WestArtifact(Serial serial)
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
    #endregion

    #region Basket5NorthArtifact
    public class Basket5NorthArtifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 2;

        [Constructable]
        public Basket5NorthArtifact()
            : base(0x24DB)
        {
        }

        public Basket5NorthArtifact(Serial serial)
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
    #endregion

    #region Basket6Artifact
    public class Basket6Artifact : BaseDecorationContainerArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 2;

        [Constructable]
        public Basket6Artifact()
            : base(0x24D5)
        {
        }

        public Basket6Artifact(Serial serial)
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
    #endregion

    #region BowlArtifact
    public class BowlArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public BowlArtifact()
            : base(0x24DE)
        {
        }

        public BowlArtifact(Serial serial)
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
    #endregion

    #region BowlsVerticalArtifact
    public class BowlsVerticalArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public BowlsVerticalArtifact()
            : base(0x24DF)
        {
        }

        public BowlsVerticalArtifact(Serial serial)
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
    #endregion

    #region BowlsHorizontalArtifact
    public class BowlsHorizontalArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public BowlsHorizontalArtifact()
            : base(0x24E0)
        {
        }

        public BowlsHorizontalArtifact(Serial serial)
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
    #endregion

    #region CupsArtifact
    public class CupsArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public CupsArtifact()
            : base(0x24E1)
        {
        }

        public CupsArtifact(Serial serial)
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
    #endregion

    #region FanWestArtifact
    public class FanWestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public FanWestArtifact()
            : base(0x240A)
        {
        }

        public FanWestArtifact(Serial serial)
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
    #endregion

    #region FanNorthArtifact
    public class FanNorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public FanNorthArtifact()
            : base(0x2409)
        {
        }

        public FanNorthArtifact(Serial serial)
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
    #endregion

    #region TripleFanWestArtifact
    public class TripleFanWestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public TripleFanWestArtifact()
            : base(0x240C)
        {
        }

        public TripleFanWestArtifact(Serial serial)
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
    #endregion

    #region TripleFanNorthArtifact
    public class TripleFanNorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public TripleFanNorthArtifact()
            : base(0x240B)
        {
        }

        public TripleFanNorthArtifact(Serial serial)
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
    #endregion

    #region FlowersArtifact
    public class FlowersArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 7;

        [Constructable]
        public FlowersArtifact()
            : base(0x284A)
        {
        }

        public FlowersArtifact(Serial serial)
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
    #endregion

    #region Painting1WestArtifact
    public class Painting1WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public Painting1WestArtifact()
            : base(0x240E)
        {
        }

        public Painting1WestArtifact(Serial serial)
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
    #endregion

    #region Painting1NorthArtifact
    public class Painting1NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public Painting1NorthArtifact()
            : base(0x240D)
        {
        }

        public Painting1NorthArtifact(Serial serial)
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
    #endregion

    #region Painting2WestArtifact
    public class Painting2WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public Painting2WestArtifact()
            : base(0x2410)
        {
        }

        public Painting2WestArtifact(Serial serial)
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
    #endregion

    #region Painting2NorthArtifact
    public class Painting2NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public Painting2NorthArtifact()
            : base(0x240F)
        {
        }

        public Painting2NorthArtifact(Serial serial)
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
    #endregion

    #region Painting3Artifact
    public class Painting3Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 5;

        [Constructable]
        public Painting3Artifact()
            : base(0x2411)
        {
        }

        public Painting3Artifact(Serial serial)
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
    #endregion

    #region Painting4WestArtifact
    public class Painting4WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 6;

        [Constructable]
        public Painting4WestArtifact()
            : base(0x2412)
        {
        }

        public Painting4WestArtifact(Serial serial)
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
    #endregion

    #region Painting4NorthArtifact
    public class Painting4NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 6;

        [Constructable]
        public Painting4NorthArtifact()
            : base(0x2411)
        {
        }

        public Painting4NorthArtifact(Serial serial)
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
    #endregion

    #region Painting5WestArtifact
    public class Painting5WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public Painting5WestArtifact()
            : base(0x2416)
        {
        }

        public Painting5WestArtifact(Serial serial)
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
    #endregion

    #region Painting5NorthArtifact
    public class Painting5NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public Painting5NorthArtifact()
            : base(0x2415)
        {
        }

        public Painting5NorthArtifact(Serial serial)
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
    #endregion

    #region Painting6WestArtifact
    public class Painting6WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public Painting6WestArtifact()
            : base(0x2418)
        {
        }

        public Painting6WestArtifact(Serial serial)
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
    #endregion

    #region Painting6NorthArtifact
    public class Painting6NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public Painting6NorthArtifact()
            : base(0x2417)
        {
        }

        public Painting6NorthArtifact(Serial serial)
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
    #endregion

    #region SakeArtifact
    public class SakeArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 4;

        [Constructable]
        public SakeArtifact()
            : base(0x24E2)
        {
        }

        public SakeArtifact(Serial serial)
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
    #endregion

    #region Sculpture1Artifact
    public class Sculpture1Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public Sculpture1Artifact()
            : base(0x2419)
        {
        }

        public Sculpture1Artifact(Serial serial)
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
    #endregion

    #region Sculpture2Artifact
    public class Sculpture2Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public Sculpture2Artifact()
            : base(0x241B)
        {
        }

        public Sculpture2Artifact(Serial serial)
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
    #endregion

    #region DolphinLeftArtifact
    public class DolphinLeftArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public DolphinLeftArtifact()
            : base(0x2846)
        {
        }

        public DolphinLeftArtifact(Serial serial)
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
    #endregion

    #region DolphinRightArtifact
    public class DolphinRightArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public DolphinRightArtifact()
            : base(0x2847)
        {
        }

        public DolphinRightArtifact(Serial serial)
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
    #endregion

    #region ManStatuetteSouthArtifact
    public class ManStatuetteSouthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public ManStatuetteSouthArtifact()
            : base(0x2848)
        {
        }

        public ManStatuetteSouthArtifact(Serial serial)
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
    #endregion

    #region ManStatuetteEastArtifact
    public class ManStatuetteEastArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public ManStatuetteEastArtifact()
            : base(0x2849)
        {
        }

        public ManStatuetteEastArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay1WestArtifact
    public class SwordDisplay1WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 5;

        [Constructable]
        public SwordDisplay1WestArtifact()
            : base(0x2842)
        {
        }

        public SwordDisplay1WestArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay1NorthArtifact
    public class SwordDisplay1NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 5;

        [Constructable]
        public SwordDisplay1NorthArtifact()
            : base(0x2843)
        {
        }

        public SwordDisplay1NorthArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay2WestArtifact
    public class SwordDisplay2WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 6;

        [Constructable]
        public SwordDisplay2WestArtifact()
            : base(0x2844)
        {
        }

        public SwordDisplay2WestArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay2NorthArtifact
    public class SwordDisplay2NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 6;

        [Constructable]
        public SwordDisplay2NorthArtifact()
            : base(0x2845)
        {
        }

        public SwordDisplay2NorthArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay3SouthArtifact
    public class SwordDisplay3SouthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public SwordDisplay3SouthArtifact()
            : base(0x2855)
        {
        }

        public SwordDisplay3SouthArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay3EastArtifact
    public class SwordDisplay3EastArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public SwordDisplay3EastArtifact()
            : base(0x2856)
        {
        }

        public SwordDisplay3EastArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay4WestArtifact
    public class SwordDisplay4WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 8;

        [Constructable]
        public SwordDisplay4WestArtifact()
            : base(0x2853)
        {
        }

        public SwordDisplay4WestArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay4NorthArtifact
    public class SwordDisplay4NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public SwordDisplay4NorthArtifact()
            : base(0x2854)
        {
        }

        public SwordDisplay4NorthArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay5WestArtifact
    public class SwordDisplay5WestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public SwordDisplay5WestArtifact()
            : base(0x2851)
        {
        }

        public SwordDisplay5WestArtifact(Serial serial)
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
    #endregion

    #region SwordDisplay5NorthArtifact
    public class SwordDisplay5NorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 9;

        [Constructable]
        public SwordDisplay5NorthArtifact()
            : base(0x2852)
        {
        }

        public SwordDisplay5NorthArtifact(Serial serial)
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
    #endregion

    #region TeapotWestArtifact
    public class TeapotWestArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public TeapotWestArtifact()
            : base(0x24E7)
        {
        }

        public TeapotWestArtifact(Serial serial)
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
    #endregion

    #region TeapotNorthArtifact
    public class TeapotNorthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public TeapotNorthArtifact()
            : base(0x24E6)
        {
        }

        public TeapotNorthArtifact(Serial serial)
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
    #endregion

    #region TowerLanternArtifact
    public class TowerLanternArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsOn
        {
            get
            {
                return ItemID == 0x24BF;
            }
            set
            {
                ItemID = value ? 0x24BF : 0x24C0;
            }
        }

        [Constructable]
        public TowerLanternArtifact()
            : base(0x24C0)
        {
            Light = LightType.Circle225;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                if (IsOn)
                {
                    IsOn = false;
                    from.PlaySound(0x3BE);
                }
                else
                {
                    IsOn = true;
                    from.PlaySound(0x47);
                }
            }
            else
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public TowerLanternArtifact(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
                Light = LightType.Circle225;
        }
    }
    #endregion

    #region Urn1Artifact
    public class Urn1Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public Urn1Artifact()
            : base(0x241D)
        {
        }

        public Urn1Artifact(Serial serial)
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
    #endregion

    #region Urn2Artifact
    public class Urn2Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public Urn2Artifact()
            : base(0x241E)
        {
        }

        public Urn2Artifact(Serial serial)
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
    #endregion

    #region ZenRock1Artifact
    public class ZenRock1Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 2;

        [Constructable]
        public ZenRock1Artifact()
            : base(0x24E4)
        {
        }

        public ZenRock1Artifact(Serial serial)
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
    #endregion

    #region ZenRock2Artifact
    public class ZenRock2Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public ZenRock2Artifact()
            : base(0x24E3)
        {
        }

        public ZenRock2Artifact(Serial serial)
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
    #endregion

    #region ZenRock3Artifact
    public class ZenRock3Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int ArtifactRarity => 3;

        [Constructable]
        public ZenRock3Artifact()
            : base(0x24E5)
        {
        }

        public ZenRock3Artifact(Serial serial)
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
    #endregion
}