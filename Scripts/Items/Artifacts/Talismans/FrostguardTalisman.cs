using System;
using Server;

namespace Server.Items
{
    public class FrostguardTalisman : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115516; } } // Frostguard Talisman
        public override int ColdResistance { get { return 3; } }

        [Constructable]
        public FrostguardTalisman()
            : base(0x2F5B)
        {
            this.Hue = 0x556;

            this.Weight = 1.0;
            this.SAAbsorptionAttributes.EaterCold = 5;
            this.Attributes.RegenMana = 1;
            this.Attributes.LowerManaCost = 5;
        }

        public FrostguardTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}