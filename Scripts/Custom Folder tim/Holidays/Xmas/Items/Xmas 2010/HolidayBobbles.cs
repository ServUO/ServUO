/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 12/4/2007
 * Time: 7:21 AM
 * http://www.shazzyshard.org
 * Santa Claus Quest 2007
 */
 
using System;
using Server;

namespace Server.Items
{
	public class HolidayBobbles : SilverEarrings
	{

		public override int ArtifactRarity{ get{ return 25; } }

		[Constructable]
		public HolidayBobbles()
		{
			Name ="Holiday Bobbles";
			Hue = 2351;

            Attributes.Luck = 50;
            Attributes.NightSight = 1;
                       
			switch ( Utility.Random( 3 ) )
			{
				case 0: Attributes.RegenHits = 2; 
				break;
				case 1: Attributes.RegenStam = 2; 
				break;
				case 2: Attributes.RegenMana = 2; 
				break;
			}
			
			switch ( Utility.Random( 5 ) )
			{
					case 0: Resistances.Fire = 5;
					break;
					case 1: Resistances.Cold = 5; 
					break;
					case 2: Resistances.Physical = 5; 
					break;
					case 3: Resistances.Energy = 5; 
					break;
					case 4: Resistances.Poison = 5; 
					break;
			}
					
		}

		public HolidayBobbles( Serial serial ) : base( serial )
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
