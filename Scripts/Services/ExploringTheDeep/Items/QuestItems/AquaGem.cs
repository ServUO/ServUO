using System;
using Server.Network;

namespace Server.Items
{
    public class AquaGem : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154244; } } // Aqua Gem

        [Constructable]
        public AquaGem() : base(0x4B48)
        {
            this.Stackable = false;
            this.Weight = 1.0;
            this.Hue = 1916;
            this.LootType = LootType.Blessed;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154245); // *You hold the gem and admire its brilliance as it radiates blue beams of light*
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }

        public AquaGem(Serial serial) : base(serial)
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