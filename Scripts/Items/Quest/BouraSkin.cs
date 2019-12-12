using System;

namespace Server.Items
{
    public class BouraSkin : Item
    {
		public override int LabelNumber { get { return 1112900; } }// Boura Skin
		
        [Constructable]
        public BouraSkin()
            : base(0x11F4)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            Hue = 0x292;
        }

        public BouraSkin(Serial serial)
            : base(serial)
        {
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