namespace Server.Items
{
    #region GargishBentasVaseArtifact
    public class GargishBentasVaseArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095947;// gargish bentas vase
        public override int ArtifactRarity => 7;

        [Constructable]
        public GargishBentasVaseArtifact()
            : base(0x42B3)
        {
        }

        public GargishBentasVaseArtifact(Serial serial)
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

    #region GargishPortraitArtifact
    public class GargishPortraitArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095950;// gargish portrait
        public override int ArtifactRarity => 7;

        [Constructable]
        public GargishPortraitArtifact()
            : base(0x42B6)
        {
        }

        public GargishPortraitArtifact(Serial serial)
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

    #region DyingPlantArtifact
    public class DyingPlantArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095954;// dying plant
        public override int ArtifactRarity => 5;

        [Constructable]
        public DyingPlantArtifact()
            : base(0x42BA)
        {
        }

        public DyingPlantArtifact(Serial serial)
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

    #region LargeDyingPlantArtifact
    public class LargeDyingPlantArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095953;// large dying plant
        public override int ArtifactRarity => 6;

        [Constructable]
        public LargeDyingPlantArtifact()
            : base(0x42B9)
        {
        }

        public LargeDyingPlantArtifact(Serial serial)
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

    #region GargishLuckTotemArtifact
    public class GargishLuckTotemArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095960;// gargish luck totem
        public override int ArtifactRarity => 6;

        [Constructable]
        public GargishLuckTotemArtifact()
            : base(0x42C0)
        {
        }

        public GargishLuckTotemArtifact(Serial serial)
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

    #region GargishKnowledgeTotemArtifact
    public class GargishKnowledgeTotemArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095961;// gargish knowledge totem
        public override int ArtifactRarity => 7;

        [Constructable]
        public GargishKnowledgeTotemArtifact()
            : base(0x42C1)
        {
        }

        public GargishKnowledgeTotemArtifact(Serial serial)
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

    #region BookOfTruthArtifact
    public class BookOfTruthArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095959;// Book of Truth
        public override int ArtifactRarity => 6;

        [Constructable]
        public BookOfTruthArtifact()
            : base(0x42BF)
        {
        }

        public BookOfTruthArtifact(Serial serial)
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

    #region GargishTraditionalVaseArtifact
    public class GargishTraditionalVaseArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095946;// gargish traditional vase
        public override int ArtifactRarity => 6;

        [Constructable]
        public GargishTraditionalVaseArtifact()
            : base(0x42B2)
        {
        }

        public GargishTraditionalVaseArtifact(Serial serial)
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

    #region GargishProtectiveTotemArtifact
    public class GargishProtectiveTotemArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095955;// gargish protective totem
        public override int ArtifactRarity => 6;

        [Constructable]
        public GargishProtectiveTotemArtifact()
            : base(0x42BB)
        {
        }

        public GargishProtectiveTotemArtifact(Serial serial)
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

    #region PushmePullyuArtifact
    public class PushmePullyuArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095445;// pushme pullyu
        public override int ArtifactRarity => 8;

        [Constructable]
        public PushmePullyuArtifact()
            : base(0x40BD)
        {
        }

        public PushmePullyuArtifact(Serial serial)
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

    #region LargePewterBowlArtifact
    public class LargePewterBowlArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095958;// large pewter bowl
        public override int ArtifactRarity => 5;

        [Constructable]
        public LargePewterBowlArtifact()
            : base(0x42BE)
        {
        }

        public LargePewterBowlArtifact(Serial serial)
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

    #region GargishMemorialStatueArtifact
    public class GargishMemorialStatueArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1095962;// gargish warrior statue
        public override int ArtifactRarity => 7;

        [Constructable]
        public GargishMemorialStatueArtifact()
            : base(0x42C3)
        {
        }

        public GargishMemorialStatueArtifact(Serial serial)
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

    #region StolenBottlesOfLiquor1Artifact
    public class StolenBottlesOfLiquor1Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113667;// stolen bottles of liquor (2)
        public override int ArtifactRarity => 4;

        [Constructable]
        public StolenBottlesOfLiquor1Artifact()
            : base(0x099C)
        {
        }

        public StolenBottlesOfLiquor1Artifact(Serial serial)
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

    #region StolenBottlesOfLiquor2Artifact
    public class StolenBottlesOfLiquor2Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113667;// stolen bottles of liquor (2)
        public override int ArtifactRarity => 4;

        [Constructable]
        public StolenBottlesOfLiquor2Artifact()
            : base(0x09A0)
        {
        }

        public StolenBottlesOfLiquor2Artifact(Serial serial)
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

    #region StolenBottlesOfLiquor3Artifact
    public class StolenBottlesOfLiquor3Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113666;// stolen bottles of liquor (3)
        public override int ArtifactRarity => 7;

        [Constructable]
        public StolenBottlesOfLiquor3Artifact()
            : base(0x099D)
        {
        }

        public StolenBottlesOfLiquor3Artifact(Serial serial)
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

    #region StolenBottlesOfLiquor4Artifact
    public class StolenBottlesOfLiquor4Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113668;// stolen bottles of liquor (4)
        public override int ArtifactRarity => 8;

        [Constructable]
        public StolenBottlesOfLiquor4Artifact()
            : base(0x099E)
        {
        }

        public StolenBottlesOfLiquor4Artifact(Serial serial)
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

    #region BottlesOfSpoiledWine1Artifact
    public class BottlesOfSpoiledWine1Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113676;// bottles of spoiled wine (2)
        public override int ArtifactRarity => 4;

        [Constructable]
        public BottlesOfSpoiledWine1Artifact()
            : base(0x09C6)
        {
        }

        public BottlesOfSpoiledWine1Artifact(Serial serial)
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

    #region BottlesOfSpoiledWine2Artifact
    public class BottlesOfSpoiledWine2Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113677;// bottles of spoiled wine (3)
        public override int ArtifactRarity => 6;

        [Constructable]
        public BottlesOfSpoiledWine2Artifact()
            : base(0x09C5)
        {
        }

        public BottlesOfSpoiledWine2Artifact(Serial serial)
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

    #region BottlesOfSpoiledWine3Artifact
    public class BottlesOfSpoiledWine3Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113678;// bottles of spoiled wine (4)
        public override int ArtifactRarity => 7;

        [Constructable]
        public BottlesOfSpoiledWine3Artifact()
            : base(0x09C4)
        {
        }

        public BottlesOfSpoiledWine3Artifact(Serial serial)
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

    #region NaverysWeb1Artifact
    public class NaverysWeb1Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113671;// Navrey's web (1)
        public override int ArtifactRarity => 4;

        [Constructable]
        public NaverysWeb1Artifact()
            : base(0x0EE3)
        {
        }

        public NaverysWeb1Artifact(Serial serial)
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

    #region NaverysWeb2Artifact
    public class NaverysWeb2Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113672;// Navrey's web (2)
        public override int ArtifactRarity => 4;

        [Constructable]
        public NaverysWeb2Artifact()
            : base(0x0EE5)
        {
        }

        public NaverysWeb2Artifact(Serial serial)
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

    #region NaverysWeb3Artifact
    public class NaverysWeb3Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113673;// Navrey's web (3)
        public override int ArtifactRarity => 5;

        [Constructable]
        public NaverysWeb3Artifact()
            : base(0x0EE4)
        {
        }

        public NaverysWeb3Artifact(Serial serial)
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

    #region NaverysWeb4Artifact 
    public class NaverysWeb4Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113675;// Navrey's Web (4)
        public override int ArtifactRarity => 5;

        [Constructable]
        public NaverysWeb4Artifact()
            : base(0x0EE6)
        {
        }

        public NaverysWeb4Artifact(Serial serial)
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

    #region NaverysWeb5Artifact
    public class NaverysWeb5Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113672;// Navrey's web (2)
        public override int ArtifactRarity => 5;

        [Constructable]
        public NaverysWeb5Artifact()
            : base(0x10D2)
        {
        }

        public NaverysWeb5Artifact(Serial serial)
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

    #region NaverysWeb6Artifact
    public class NaverysWeb6Artifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113671;// Navrey's web (1)
        public override int ArtifactRarity => 5;

        [Constructable]
        public NaverysWeb6Artifact()
            : base(0x10D3)
        {
        }

        public NaverysWeb6Artifact(Serial serial)
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

    #region RottedOarsArtifact
    public class RottedOarsArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113665;// rotted oars
        public override int ArtifactRarity => 8;

        [Constructable]
        public RottedOarsArtifact()
            : base(0x1E2B)
        {
        }

        public RottedOarsArtifact(Serial serial)
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

    #region BloodySpoonArtifact
    public class BloodySpoonArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113664;// bloody spoon
        public override int ArtifactRarity => 5;

        [Constructable]
        public BloodySpoonArtifact()
            : base(0x09C2)
        {
        }

        public BloodySpoonArtifact(Serial serial)
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

    #region MysteriousSupperArtifact
    public class MysteriousSupperArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113663;// mysterious supper
        public override int ArtifactRarity => 3;

        [Constructable]
        public MysteriousSupperArtifact()
            : base(0x09DB)
        {
        }

        public MysteriousSupperArtifact(Serial serial)
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

    #region DriedUpInkWellArtifact
    public class DriedUpInkWellArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113662;// dried up ink well
        public override int ArtifactRarity => 7;

        [Constructable]
        public DriedUpInkWellArtifact()
            : base(0x2D61)
        {
        }

        public DriedUpInkWellArtifact(Serial serial)
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

    #region TyballsFlaskStandArtifact
    public class TyballsFlaskStandArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113659;// Tyball's flask stand
        public override int ArtifactRarity => 9;

        [Constructable]
        public TyballsFlaskStandArtifact()
            : base(0x1829)
        {
        }

        public TyballsFlaskStandArtifact(Serial serial)
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

    #region BlockAndTackleArtifact
    public class BlockAndTackleArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113660;// block and tackle
        public override int ArtifactRarity => 9;

        [Constructable]
        public BlockAndTackleArtifact()
            : base(0x1E9A)
        {
        }

        public BlockAndTackleArtifact(Serial serial)
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

    #region RemnantsOfMeatLoafArtifact
    public class RemnantsOfMeatLoafArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113661;// remnants of meat loaf
        public override int ArtifactRarity => 5;

        [Constructable]
        public RemnantsOfMeatLoafArtifact()
            : base(0x09AE)
        {
        }

        public RemnantsOfMeatLoafArtifact(Serial serial)
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

    #region HalfEatenSupperArtifact
    public class HalfEatenSupperArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113658;// half eaten supper
        public override int ArtifactRarity => 5;

        [Constructable]
        public HalfEatenSupperArtifact()
            : base(0x0A19)
        {
        }

        public HalfEatenSupperArtifact(Serial serial)
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

    #region PricelessTreasureArtifact
    public class PricelessTreasureArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113680;// priceless treasure
        public override int ArtifactRarity => 8;

        [Constructable]
        public PricelessTreasureArtifact()
            : base(0x1B54)
        {
        }

        public PricelessTreasureArtifact(Serial serial)
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

    #region FakeCopperIngotsArtifact
    public class FakeCopperIngotsArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113679;// fake copper ingots
        public override int ArtifactRarity => 7;

        [Constructable]
        public FakeCopperIngotsArtifact()
            : base(0x1BE5)
        {
        }

        public FakeCopperIngotsArtifact(Serial serial)
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

    #region JugsOfGoblinRotgutArtifact
    public class JugsOfGoblinRotgutArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113681;// jugs of goblin rotgut
        public override int ArtifactRarity => 3;

        [Constructable]
        public JugsOfGoblinRotgutArtifact()
            : base(0x098E)
        {
        }

        public JugsOfGoblinRotgutArtifact(Serial serial)
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

    #region BatteredPanArtifact
    public class BatteredPanArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113669;// battered pan
        public override int ArtifactRarity => 6;

        [Constructable]
        public BatteredPanArtifact()
            : base(0x09DE)
        {
        }

        public BatteredPanArtifact(Serial serial)
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

    #region RustedPanArtifact
    public class RustedPanArtifact : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113670;// rusted pan
        public override int ArtifactRarity => 6;

        [Constructable]
        public RustedPanArtifact()
            : base(0x09E8)
        {
        }

        public RustedPanArtifact(Serial serial)
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
