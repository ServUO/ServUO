using System;

namespace Server.Items
{
    public class BouraSkin : Item
    {
        [Constructable]
        public BouraSkin()
            : base(0x11F4)
        {
            this.Name = "Boura Skin";
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
            this.Hue = 0x292;
        }

        public BouraSkin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113024;
            }
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