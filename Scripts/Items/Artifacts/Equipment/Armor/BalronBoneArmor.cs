using System;

namespace Server.Items
{
    public class BalronBoneArmor : BoneChest
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BalronBoneArmor()
        {
            Name = "Balron Bone Armor";
            Hue = 1109;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
        }

        public BalronBoneArmor(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 60; } }
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

    public class GargishBalronBoneArmor : GargishPlateChest
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishBalronBoneArmor()
        {
            Name = "Gargish Balron Bone Armor";
            Hue = 1109;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
        }

        public GargishBalronBoneArmor(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 60; } }
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
