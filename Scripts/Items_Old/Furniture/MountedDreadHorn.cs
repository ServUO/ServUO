using System;

namespace Server.Items
{
    [FlipableAttribute(0x3158, 0x3159)]
    public class MountedDreadHorn : Item
    {
        [Constructable]
        public MountedDreadHorn()
            : base(0x3158)
        {
            this.Weight = 1.0;
        }

        public MountedDreadHorn(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074464;
            }
        }// mounted Dread Horn
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