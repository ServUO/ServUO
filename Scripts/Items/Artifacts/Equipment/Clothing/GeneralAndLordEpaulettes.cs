using System;

namespace Server.Items
{
    public class GeneralLethesEpaulettes : Epaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GeneralLethesEpaulettes()
        {
            Name = "General Lethe's Epaulettes";
            Hue = 1157;

            Attributes.BonusMana = 8;
            Attributes.RegenMana = 1;
            Attributes.CastRecovery = 1;
            Attributes.LowerRegCost = 10;
        }

        public GeneralLethesEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishGeneralLethesEpaulettes : GargishEpaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishGeneralLethesEpaulettes()
        {
            Name = "Gargish General Lethe's Epaulettes";
            Hue = 1157;

            Attributes.BonusMana = 8;
            Attributes.RegenMana = 1;
            Attributes.CastRecovery = 1;
            Attributes.LowerRegCost = 10;
        }

        public GargishGeneralLethesEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class LordMorphiusEpaulettes : Epaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public LordMorphiusEpaulettes()
        {
            Name = "Lord Morphius' Epaulettes";
            Hue = 1161;

            Attributes.BonusStam = 8;
            Attributes.RegenStam = 2;
            Attributes.LowerManaCost = 5;
            Attributes.WeaponSpeed = 10;
        }

        public LordMorphiusEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishLordMorphiusEpaulettes : GargishEpaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishLordMorphiusEpaulettes()
        {
            Name = "Gargish Lord Morphius' Epaulettes";
            Hue = 1161;

            Attributes.BonusStam = 8;
            Attributes.RegenStam = 2;
            Attributes.LowerManaCost = 5;
            Attributes.WeaponSpeed = 10;
        }

        public GargishLordMorphiusEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
