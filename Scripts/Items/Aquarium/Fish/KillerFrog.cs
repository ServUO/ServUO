using System;

namespace Server.Items
{
    public class KillerFrog : BaseFish
    { 
        [Constructable]
        public KillerFrog()
            : base(0x3B0D)
        {
        }

        public KillerFrog(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1073825;
            }
        }// A Killer Frog 
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