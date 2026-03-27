using System;

namespace Server.Items
{
    public class SentinelsMempo : PlateMempo
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SentinelsMempo()
        {
            Name = "Sentinel's Mempo";
            Hue = 1157;

            Attributes.BonusStr = 4;
            Attributes.BonusDex = 4;
            Attributes.BonusHits = 8;
            Attributes.BonusStam = 12;
            Attributes.BonusMana = 8;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 8;
        }

        public SentinelsMempo(Serial serial)
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

    public class SentinelsNecklace : GargishNecklace
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SentinelsNecklace()
        {
            Name = "Sentinel's Necklace";
            Hue = 1157;

            Attributes.BonusStr = 4;
            Attributes.BonusDex = 4;
            Attributes.BonusHits = 8;
            Attributes.BonusStam = 12;
            Attributes.BonusMana = 8;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 8;
        }

        public SentinelsNecklace(Serial serial)
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
