using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.PartySystem;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.ShameRevamped
{
	public class ShameTeleporter : Teleporter
	{
		public ShameTeleporter(Point3D dest, Map map) : base(dest, map, true)
		{
		}

		public ShameTeleporter(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

            if (version == 0)
            {
                int count = reader.ReadInt();

                for (int i = 0; i < count; i++)
                {
                    reader.ReadMobile();
                }
            }
		}
	}
}