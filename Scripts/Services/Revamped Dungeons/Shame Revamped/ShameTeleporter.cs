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

    public class ShameWallTeleporter : Teleporter
    {
        public ShameWallTeleporter(Point3D dest, Map map)
            : base(dest, map, true)
        {
        }

        public override bool CanTeleport(Mobile m)
        {
            if(Deleted || Map == null || Map == Map.Internal)
                return false;

            IPooledEnumerable eable = Map.GetItemsInRange(Location, 1);
            bool active = false;

            foreach (Item item in eable)
            {
                if (item is AddonComponent && ((AddonComponent)item).Addon is ShameWall && ((AddonComponent)item).Addon.Visible)
                {
                    active = true;
                    break;
                }
            }

            eable.Free();
            return active;
        }

        public override void DoTeleport(Mobile m)
        {
            m.SendLocalizedMessage(1072790); // The wall becomes transparent, and you push your way through it.

            base.DoTeleport(m);
        }

        public ShameWallTeleporter(Serial serial)
            : base(serial)
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