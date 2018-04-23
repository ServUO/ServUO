using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SemidisRobe : HoodedShroudOfShadows
	{

		public override int ArtifactRarity{ get{ return 21; } }

		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		[Constructable]
		public SemidisRobe()
		{
			Hue = 1199;
			Name = "Semidi's Robes";
			Weight = 12.0;
			Attributes.BonusInt = 5;
			Attributes.RegenHits = 3;
			
		}
		
		public override bool OnEquip(Mobile m) 
	      { 
			m.NameMod = "a flitting shadow";
			m.DisplayGuildTitle = false;
			m.SendMessage( "Putting on the robes you become  unrecognisable" );
			return base.OnEquip(m); 
		}
		
		public override void OnRemoved( object parent) 
	      { 
		if (parent is Mobile) 
	        { 
	         Mobile m = (Mobile)parent; 
		   m.NameMod = null;
		   m.SendMessage( "Pulling off the robes reveals your identity" );
		   m.DisplayGuildTitle = true;
		  }

	         base.OnRemoved(parent); 
      	}

		public SemidisRobe ( Serial serial ) : base( serial )
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