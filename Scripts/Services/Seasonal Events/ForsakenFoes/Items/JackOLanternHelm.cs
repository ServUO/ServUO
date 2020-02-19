using System;

namespace Server.Items
{
    public class JackOLanternHelm : BaseArmor
    {
        public override int LabelNumber { get { return 1125986; } } // jack o' lantern helm
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public JackOLanternHelm()
            : base(0xA3EA)
        {
            Weight = 3.0;
            Layer = Layer.Helm;
            Light = LightType.Circle300;
        }

        public override int BasePhysicalResistance { get { return 12; } }
        public override int BaseFireResistance { get { return 14; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 10; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        public override int AosStrReq { get { return 10; } }

        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Plate;
            }
        }

        public JackOLanternHelm(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
