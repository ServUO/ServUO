using System;

namespace Server.Items
{
    public class AriellesBauble : Item
    { 
        [Constructable]
        public AriellesBauble()
            : base(0x23B)
        {
            this.Weight = 2.0;
            this.LootType = LootType.Blessed;
        }

        public AriellesBauble(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073137;
            }
        }// A bauble
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