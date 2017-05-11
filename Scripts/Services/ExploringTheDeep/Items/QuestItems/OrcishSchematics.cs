using System;
using Server.Network;

namespace Server.Items
{    
    public class OrcishSchematics : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154232; } } // Schematic for an Orcish Drilling Machine
        
        [Constructable]
        public OrcishSchematics() : base(0x2258)
        {
            this.Hue = 1945;
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154233); // *It appears to be the crude schematic to a drilling machine of Orcish origin. It is poorly devised and looks as if one were to build it the machine would explode*
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public OrcishSchematics(Serial serial) : base(serial)
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
