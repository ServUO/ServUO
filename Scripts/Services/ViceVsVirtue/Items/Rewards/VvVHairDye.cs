using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
	public class VvVHairDye : Item
	{
        public override int LabelNumber
        {
            get
            {
                if (this.Hue == ViceVsVirtueSystem.VirtueHue)
                    return 1155538;

                return 1155539;
            }
        }

        [Constructable]
        public VvVHairDye(int hue)
            : base(3838)
        {
            Hue = hue;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                if (ViceVsVirtueSystem.IsVvV(m))
                {
                    m.HairHue = this.Hue;
                    m.FacialHairHue = this.Hue;

                    Delete();
                    m.PlaySound(0x4E);
                    m.SendLocalizedMessage(501199);  // You dye your hair
                }
                else
                {
                    m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

        public VvVHairDye(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}