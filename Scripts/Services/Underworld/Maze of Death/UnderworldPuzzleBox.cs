using System;
using Server;
using System.Collections.Generic;

namespace Server.Items
{
	public class UnderworldPuzzleBox : MetalChest
	{
		private static Dictionary<Mobile, DateTime> m_Table = new Dictionary<Mobile, DateTime>();
		
		[Constructable]
		public UnderworldPuzzleBox()
		{
            Movable = false;
            ItemID = 3712;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
            if (from.InRange(this.Location, 3))
            {
                if (from.AccessLevel == AccessLevel.Player && IsInCooldown(from))
                    from.SendLocalizedMessage(1113413); // You have recently participated in this challenge. You must wait 24 hours to try again.
                else if (from.Backpack != null)
                {
                    Item item = from.Backpack.FindItemByType(typeof(UnderworldPuzzleItem));

                    if (item != null)
                        from.SendMessage("You already have a puzzle board.");
                    else
                    {
                        from.AddToBackpack(new UnderworldPuzzleItem());
                        from.SendMessage("You recieve a puzzle piece.");

                        if (from.AccessLevel == AccessLevel.Player)
                            m_Table[from] = DateTime.UtcNow + TimeSpan.FromHours(24);
                    }
                }
            }
		}
		
		public static bool IsInCooldown(Mobile from)
		{
			if(m_Table.ContainsKey(from))
			{
				if(m_Table[from] < DateTime.UtcNow)
					m_Table.Remove(from);
			}
			
			return m_Table.ContainsKey(from);
		}
		
		public UnderworldPuzzleBox(Serial serial) : base(serial)
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
	}
}