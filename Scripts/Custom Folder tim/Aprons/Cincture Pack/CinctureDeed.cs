// Scripted by Jumpnjahosofat //
//Modded by Velius//

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class CinctureDeed : Item
	{

		[Constructable]
		public CinctureDeed () : this( null )
		{
		}

		[Constructable]
		public CinctureDeed ( string name ) : base ( 0x14F0 )
		{
			Name = "A Random Cincture Prize Ticket";
			Hue = 144;
		}

		public CinctureDeed ( Serial serial ) : base ( serial )
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
/////////////////Prize Cincture Deed
                switch (Utility.Random(8))
                {
                    case 0: from.AddToBackpack(new AmberCincture()); break;
                    case 1: from.AddToBackpack(new AzureCincture()); break;
                    case 2: from.AddToBackpack(new GoldCincture()); break;
                    case 3: from.AddToBackpack(new IndigoCincture()); break;
                    case 4: from.AddToBackpack(new IvoryCincture()); break;
                    case 5: from.AddToBackpack(new JadeCincture()); break;
                    case 6: from.AddToBackpack(new SilverCincture()); break;
                    case 7: from.AddToBackpack(new CrimsonCincture()); break;
                    
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