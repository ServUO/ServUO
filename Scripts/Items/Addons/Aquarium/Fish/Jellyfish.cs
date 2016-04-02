using System;

namespace Server.Items
{
    public class Jellyfish : BaseFish
    { 
        [Constructable]
        public Jellyfish()
            : base(0x3B0E)
        {
        }

        public Jellyfish(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1074593;
            }
        }// Jellyfish
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