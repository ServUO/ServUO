using System;

namespace Server.Items
{
    public class PhoenixHelm : NorseHelm
    {
        [Constructable]
        public PhoenixHelm()
        {
            Hue = 0x8E;
			LootType = LootType.Blessed;     
        }

        public PhoenixHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041609;
            }
        }// norse helm of the phoenix
        
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