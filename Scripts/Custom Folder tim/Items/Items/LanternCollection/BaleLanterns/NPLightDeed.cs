//BaleFire//

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class NPLightDeed : Item
	{

		[Constructable]
		public NPLightDeed () : this( null )
		{
		}

		[Constructable]
		public NPLightDeed ( string name ) : base ( 0x14F0 )
		{
			Name = "-New Players Light-";
			Hue = 1366;
		}

		public NPLightDeed ( Serial serial ) : base ( serial )
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
/////////////////New Player Light Deed
                switch (Utility.Random(5))
                {
                    case 0: from.AddToBackpack(new Tundra()); break;
                    case 1: from.AddToBackpack(new NetherGuard()); break;
                    case 2: from.AddToBackpack(new HeroFlame()); break;
                    case 3: from.AddToBackpack(new ToxicLight()); break;
                    case 4: from.AddToBackpack(new MageGuide()); break;
                    
                    
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