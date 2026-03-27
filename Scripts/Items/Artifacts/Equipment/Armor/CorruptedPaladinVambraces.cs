using System;

namespace Server.Items
{
    public class CorruptedPaladinVambraces : PlateArms
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public CorruptedPaladinVambraces()
        {
            Name = "Corrupted Paladin Vambraces";
            Hue = 1109;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 10;
            Attributes.BonusMana = 10;
            Attributes.RegenHits = 4;
            Attributes.RegenStam = 4;
            Attributes.RegenMana = 4;
            Attributes.LowerManaCost = 8;
        }

        public CorruptedPaladinVambraces(Serial serial)
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishCorruptedPaladinVambraces : GargishPlateArms
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishCorruptedPaladinVambraces()
        {
            Name = "Gargish Corrupted Paladin Vambraces";
            Hue = 1109;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 10;
            Attributes.BonusMana = 10;
            Attributes.RegenHits = 4;
            Attributes.RegenStam = 4;
            Attributes.RegenMana = 4;
            Attributes.LowerManaCost = 8;
        }

        public GargishCorruptedPaladinVambraces(Serial serial)
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
