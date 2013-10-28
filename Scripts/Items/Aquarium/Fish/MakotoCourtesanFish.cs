using System;

namespace Server.Items
{
    public class MakotoCourtesanFish : BaseFish
    { 
        [Constructable]
        public MakotoCourtesanFish()
            : base(0x3AFD)
        {
        }

        public MakotoCourtesanFish(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1073835;
            }
        }// A Makoto Courtesan Fish
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