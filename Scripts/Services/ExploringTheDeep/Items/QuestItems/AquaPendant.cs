using System;
using Server.Network;

namespace Server.Items
{
    public class AquaPendant : GoldNecklace
    {
        public override int LabelNumber { get { return 1154246; } } // Aqua Pendant

        [Constructable]
        public AquaPendant()
        {
            this.Hue = 1916;
            this.LootType = LootType.Blessed;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154247); // *As you hold the pendant in your hands you suddenly feel as though you no longer need to breathe.  The pendant pulses with magical energy!*
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public AquaPendant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}