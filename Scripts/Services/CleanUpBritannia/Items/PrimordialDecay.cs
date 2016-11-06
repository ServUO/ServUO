using System;

namespace Server.Items
{
    public class PrimordialDecay : BaseInstrument
    {
        [Constructable]
        public PrimordialDecay()
            : base(0x0EB3)
        {
            RandomInstrument();
            this.Hue = 1927;
            this.Weight = 4;
            this.Slayer = SlayerName.ElementalBan;
        }

        public PrimordialDecay(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154723;
            }
        }// Primordial Decay
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