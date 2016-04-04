using System;

namespace Server.Items
{
    public class PiecesOfCrystal : PeerlessKey
    {
        [Constructable]
        public PiecesOfCrystal()
            : base(0x2245)
        {
            this.Weight = 1;
            this.Hue = 0x2B2;
        }

        public PiecesOfCrystal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074263;
            }
        }// crushed crystal pieces
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