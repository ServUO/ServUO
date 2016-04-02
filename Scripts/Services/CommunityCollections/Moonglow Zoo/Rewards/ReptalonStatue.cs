using System;

namespace Server.Items
{
    public class ReptalonStatue : Item
    {
        [Constructable]
        public ReptalonStatue()
            : base(0x2D95)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;			
        }

        public ReptalonStatue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073192;
            }
        }// A Reptalon Contribution Statue from the Britannia Royal Zoo.
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