using System;

namespace Server.Items
{
    public class Aegis : HeaterShield
    {
        [Constructable]
        public Aegis()
        {
            this.Hue = 0x47E;
            this.ArmorAttributes.SelfRepair = 5;
            this.Attributes.ReflectPhysical = 15;
            this.Attributes.DefendChance = 15;
            this.Attributes.LowerManaCost = 8;
        }

        public Aegis(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061602;
            }
        }// Ægis
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
                this.PhysicalBonus = 0;
        }
    }
}