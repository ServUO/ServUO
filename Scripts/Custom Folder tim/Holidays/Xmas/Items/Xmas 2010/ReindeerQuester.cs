using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{	
	public class ReindeerQuester : BaseEscort
	{				
		public override bool InitialInnocent{ get{ return true; } }
		public override bool IsInvulnerable{ get{ return false; } }
	
		public override Type[] Quests{ get{ return new Type[] 
		{
			typeof( ReindeerRoundup )
		}; } }
	
		[Constructable]
		public ReindeerQuester() : base()
		{			
			Name = NameList.RandomName( "reindeer" );
		}
		
		public ReindeerQuester( Serial serial ) : base( serial )
		{
		}		
		
		public override bool CanBeDamaged()
		{
			return true;
		}
		
		public override void InitBody()
		{
			InitStats( 100, 100, 25 );
			
			Blessed = false;
			Female = false;
			Body = 0xEA;
			Hue = Utility.RandomList( 1810, 1811, 1812, 1813, 1814, 1815 );

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
		}
	}
}

