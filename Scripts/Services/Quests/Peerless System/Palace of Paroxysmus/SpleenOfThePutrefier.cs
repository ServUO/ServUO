using System;

namespace Server.Items
{
    public class SpleenOfThePutrefier : PeerlessKey
    {
        [Constructable]
        public SpleenOfThePutrefier()
            : base(0x1CEE)
        {
            this.Weight = 1.0;
        }

        public SpleenOfThePutrefier(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074329;
            }
        }// spleen of the putrefier
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