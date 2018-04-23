using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class DungeonDoor : BaseDoor
	{
		public override void Use( Mobile from )
		{
            //find any keys on the person OR on the ground to avoid exploit.
            if (Key.ContainsKey(from.Backpack, KeyValue))
            {
                Key.RemoveKeys(from.Backpack, KeyValue);
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "As you unlock the door, the key crubles in your hand."); // You quickly unlock, open, and relock the door

                BaseDoor link = (BaseDoor)base.Link;
                //unlock door and its link
                base.Locked = false;
                if (link != null)
                    link.Locked = false;
            }

            ArrayList keys = new ArrayList();
            foreach (Item i in this.GetItemsInRange(5))
            {
                if (i is Key)
                    keys.Add(i);
            }
            foreach (Key k in keys)
                k.Delete();

            base.Use(from);
		}

		[Constructable]
		public DungeonDoor( DoorFacing facing ) : base( 0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset( facing ) )
		{
			Name = "a dungeon door";
		}

		public DungeonDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					break;
				}
			}
		}
	}
}