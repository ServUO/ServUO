// Scripted by Jumpnjahosofat //
//Modded by Forgotten//

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class LanternDeed : Item
	{

		[Constructable]
		public LanternDeed () : this( null )
		{
		}

		[Constructable]
		public LanternDeed ( string name ) : base ( 0x14F0 )
		{
			Name = "A Random Lantern Prize Ticket";
			Hue = 37;
		}

		public LanternDeed ( Serial serial ) : base ( serial )
		{
		}

      		public override void OnDoubleClick( Mobile from ) 
      		{
			if ( !IsChildOf( from.Backpack ) )
			{
                from.SendLocalizedMessage(1042001);
            }
            else
            {
/////////////////Prize Lantern Deed
                switch (Utility.Random(5))
                {
                    case 0: from.AddToBackpack(new ArcticBeacon()); break;
                    case 1: from.AddToBackpack(new AuraOfShadows()); break;
                    case 2: from.AddToBackpack(new EternalFlame()); break;
                    case 3: from.AddToBackpack(new NoxNightlight()); break;
                    case 4: from.AddToBackpack(new PowerSurge()); break;
                    
                    
                }
                this.Delete();
			}

		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}