using System;

namespace Server.Items
{
    public class UmbrascaleChampionsAegis : WingedHelm
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public UmbrascaleChampionsAegis()
        {
            Name = "Umbrascale Champion's Aegis";
            Hue = 1109;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
        }

        public UmbrascaleChampionsAegis(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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

    public class GargishUmbrascaleChampionsAegis : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishUmbrascaleChampionsAegis()
        {
            Name = "Gargish Umbrascale Champion's Aegis";
            Hue = 1109;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
        }

        public GargishUmbrascaleChampionsAegis(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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
