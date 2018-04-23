// Created by Neptune

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class AkyndahEarrings : GoldEarrings
	{
                public override int ArtifactRarity{ get{ return 2106; } }

		[Constructable]
		public AkyndahEarrings()
		{
			 
			Name = "Earrings of Akyndah";
            Hue = 1360;
			Attributes.Luck = 100;

            		
		 	Attributes.BonusHits = 15;
		 	Attributes.BonusStam = 15;				
			Attributes.BonusMana = 15;
			Attributes.BonusStr = 25;
			Attributes.BonusDex = 25;
			Attributes.BonusInt = 25;
			Attributes.WeaponDamage = 15;
			Attributes.WeaponSpeed = 25;
		}
		
		public override bool OnEquip(Mobile m) 
	      { 

			if ( m.Mounted )
			{
				m.SendMessage( "You cant do that while mounted" );
				return false;
			}


			
		        m.BodyMod = 173;
			m.HueMod = 2654;
               	
			m.SendMessage( "The earrings have transformed you into a Spider Servant" );
            m.PlaySound( 484 );
			return base.OnEquip(m);
				

		}
		
		public override void OnRemoved( object parent) 
	      { 
		if (parent is Mobile) 
	        { 
	         Mobile m = (Mobile)parent; 
		   m.NameMod = null;
                   m.BodyMod = 0;
           	   m.HueMod = -1;
		   m.SendMessage( "Your back to your old self." );
                   m.PlaySound( 484 );		
		  }

	         base.OnRemoved(parent); 
      	}

        public AkyndahEarrings(Serial serial): base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
 
		}
	}
}