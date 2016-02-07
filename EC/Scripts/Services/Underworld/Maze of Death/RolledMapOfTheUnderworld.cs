using Server;
using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class RolledMapOfTheUnderworld : Item
	{
		[Constructable]
		public RolledMapOfTheUnderworld() : base(5357)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 3))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump());
            }
        }
		
		public RolledMapOfTheUnderworld(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(75, 75)
            {
                AddImage(0, 0, 0x7739);
            }
        }
	}
}