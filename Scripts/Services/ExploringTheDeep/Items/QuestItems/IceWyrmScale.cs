using System;
using Server.Network;

namespace Server.Items
{
    public class IceWyrmScale : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154224; } } // Ice Wyrm Scale

        [Constructable]
        public IceWyrmScale()
            : base(0x26B2)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 2729;
            this.Weight = 20.0;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154225); // *You run your hand across the scale, it is cold to the touch and smooth like glass. You decide to take it to the Master Tinker*
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }

        public IceWyrmScale(Serial serial)
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
