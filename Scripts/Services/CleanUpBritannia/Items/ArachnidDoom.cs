using System;

namespace Server.Items
{
    public class ArachnidDoom : BaseInstrument
    {
        [Constructable]
        public ArachnidDoom()
        {
            this.Hue = 1944;
            this.Weight = 4;
            this.Slayer = SlayerName.ArachnidDoom;

            UsesRemaining = 450;
        }       

        public ArachnidDoom(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154724;
            }
        }// Arachnid Doom
        public override int InitMinUses
        {
            get
            {
                return 450;
            }
        }
        public override int InitMaxUses
        {
            get
            {
                return 450;
            }
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