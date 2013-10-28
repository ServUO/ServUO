using System;

namespace Server.Items
{
    #region BackpackArtifact
    public class BackpackArtifact : BaseDecorationContainerArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public BackpackArtifact()
            : base(0x9B2)
        {
        }

        public BackpackArtifact(Serial serial)
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

    #region BloodyWaterArtifact
    public class BloodyWaterArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public BloodyWaterArtifact()
            : base(0xE23)
        {
        }

        public BloodyWaterArtifact(Serial serial)
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

    #region BooksWestArtifact
    public class BooksWestArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 3;
            }
        }

        [Constructable]
        public BooksWestArtifact()
            : base(0x1E25)
        {
        }

        public BooksWestArtifact(Serial serial)
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

    #region BooksNorthArtifact
    public class BooksNorthArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 3;
            }
        }

        [Constructable]
        public BooksNorthArtifact()
            : base(0x1E24)
        {
        }

        public BooksNorthArtifact(Serial serial)
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

    #region BooksFaceDownArtifact
    public class BooksFaceDownArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 3;
            }
        }

        [Constructable]
        public BooksFaceDownArtifact()
            : base(0x1E21)
        {
        }

        public BooksFaceDownArtifact(Serial serial)
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

    #region BottleArtifact
    public class BottleArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 1;
            }
        }

        [Constructable]
        public BottleArtifact()
            : base(0xE28)
        {
        }

        public BottleArtifact(Serial serial)
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

    #region BrazierArtifact
    public class BrazierArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 2;
            }
        }

        [Constructable]
        public BrazierArtifact()
            : base(0xE31)
        {
            this.Light = LightType.Circle150;
        }

        public BrazierArtifact(Serial serial)
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

    #region CocoonArtifact
    public class CocoonArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 7;
            }
        }

        [Constructable]
        public CocoonArtifact()
            : base(0x10DA)
        {
        }

        public CocoonArtifact(Serial serial)
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

    #region DamagedBooksArtifact
    public class DamagedBooksArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 1;
            }
        }

        [Constructable]
        public DamagedBooksArtifact()
            : base(0xC16)
        {
        }

        public DamagedBooksArtifact(Serial serial)
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

    #region EggCaseArtifact
    public class EggCaseArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public EggCaseArtifact()
            : base(0x10D9)
        {
        }

        public EggCaseArtifact(Serial serial)
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

    #region GruesomeStandardArtifact
    public class GruesomeStandardArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public GruesomeStandardArtifact()
            : base(0x428)
        {
        }

        public GruesomeStandardArtifact(Serial serial)
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

    #region LampPostArtifact
    public class LampPostArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 3;
            }
        }

        [Constructable]
        public LampPostArtifact()
            : base(0xB24)
        {
            this.Light = LightType.Circle300;
        }

        public LampPostArtifact(Serial serial)
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

    #region LeatherTunicArtifact
    public class LeatherTunicArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 9;
            }
        }

        [Constructable]
        public LeatherTunicArtifact()
            : base(0x13CA)
        {
        }

        public LeatherTunicArtifact(Serial serial)
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

    #region RockArtifact
    public class RockArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 1;
            }
        }

        [Constructable]
        public RockArtifact()
            : base(0x1363)
        {
        }

        public RockArtifact(Serial serial)
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

    #region RuinedPaintingArtifact
    public class RuinedPaintingArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 12;
            }
        }

        [Constructable]
        public RuinedPaintingArtifact()
            : base(0xC2C)
        {
        }

        public RuinedPaintingArtifact(Serial serial)
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

    #region SaddleArtifact
    public class SaddleArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 9;
            }
        }

        [Constructable]
        public SaddleArtifact()
            : base(0xF38)
        {
        }

        public SaddleArtifact(Serial serial)
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

    #region SkinnedDeerArtifact
    public class SkinnedDeerArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 8;
            }
        }

        [Constructable]
        public SkinnedDeerArtifact()
            : base(0x1E91)
        {
        }

        public SkinnedDeerArtifact(Serial serial)
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

    #region SkinnedGoatArtifact
    public class SkinnedGoatArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public SkinnedGoatArtifact()
            : base(0x1E88)
        {
        }

        public SkinnedGoatArtifact(Serial serial)
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

    #region SkullCandleArtifact
    public class SkullCandleArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 1;
            }
        }

        [Constructable]
        public SkullCandleArtifact()
            : base(0x1858)
        {
            this.Light = LightType.Circle150;
        }

        public SkullCandleArtifact(Serial serial)
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

    #region StretchedHideArtifact
    public class StretchedHideArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 2;
            }
        }

        [Constructable]
        public StretchedHideArtifact()
            : base(0x106B)
        {
        }

        public StretchedHideArtifact(Serial serial)
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

    #region StuddedLeggingsArtifact
    public class StuddedLeggingsArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public StuddedLeggingsArtifact()
            : base(0x13D8)
        {
        }

        public StuddedLeggingsArtifact(Serial serial)
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

    #region StuddedTunicArtifact
    public class StuddedTunicArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 7;
            }
        }

        [Constructable]
        public StuddedTunicArtifact()
            : base(0x13D9)
        {
        }

        public StuddedTunicArtifact(Serial serial)
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

    #region TarotCardsArtifact
    public class TarotCardsArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity
        {
            get
            {
                return 5;
            }
        }

        [Constructable]
        public TarotCardsArtifact()
            : base(0x12A5)
        {
        }

        public TarotCardsArtifact(Serial serial)
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