using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Network;

namespace Server.Items
{
    public class MercutiosCutlass : Cutlass
    {
        public override int LabelNumber { get { return 1154240; } } // Mercutio's Cutlass

        [Constructable]
        public MercutiosCutlass()
        {
            this.Hue = 5185;
            this.LootType = LootType.Blessed;
        }
		
		public override void OnDoubleClick(Mobile from)
        {
			base.OnDoubleClick(from);			
			
			from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154241); // *It is a beautifully and masterfully crafted blade. The hilt bears the family crest of the former owner*
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            cold = fire = chaos = pois = nrgy = direct = 0;
            phys = 100;
        }

        public MercutiosCutlass(Serial serial) : base(serial)
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
