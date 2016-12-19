using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVRobe : BaseOuterTorso
	{
        public override int LabelNumber
        {
            get
            {
                if (this.Hue == ViceVsVirtueSystem.VirtueHue)
                    return 1155532;

                if (this.Hue == ViceVsVirtueSystem.ViceHue)
                    return 1155533;

                return base.LabelNumber;
            }
        }

        public VvVRobe(int hue)
            : base(0x2684, hue)
        {
            IsVvVItem = true;
        }

        public VvVRobe(Serial serial)
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