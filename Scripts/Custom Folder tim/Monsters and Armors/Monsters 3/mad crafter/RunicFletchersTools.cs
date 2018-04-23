using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class RunicFletchersTools : BaseRunicTool
	{
		public override CraftSystem CraftSystem{ get{ return DefBowFletching.CraftSystem; } }
		
		[Constructable]
		public RunicFletchersTools( CraftResource resource ) : base( resource, 0x1022 )
		{
			Weight = 2.0;
			Hue = CraftResources.GetHue( resource );
		}

		[Constructable]
		public RunicFletchersTools( CraftResource resource, int uses ) : base( resource, uses, 0x1022 )
		{
			Weight = 2.0;
			Hue = CraftResources.GetHue( resource );
		}

		public RunicFletchersTools( Serial serial ) : base( serial )
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

		}
		
		public override void AddNameProperty( ObjectPropertyList list )
		{
			string woodType;

			if ( Hue == 0 )
			{
				woodType = "";
			}
			else
			{
				switch ( Resource )
				{
					//case CraftResource.Pine:			woodType = "pine"; break; 
					//case CraftResource.Cedar:			woodType = "cedar"; break; 
					//case CraftResource.Cherry:			woodType = "cherry"; break; 
					//case CraftResource.Mahogany:			woodType = "mahogany"; break; 					
					//case CraftResource.Oak:				woodType = "oak"; break;
					//case CraftResource.Ash:				woodType = "ash"; break;
					//case CraftResource.Yew:				woodType = "yew"; break;
					case CraftResource.Heartwood:			woodType = "heartwood"; break;
					case CraftResource.Bloodwood:			woodType = "bloodwood"; break;
					case CraftResource.Frostwood:			woodType = "frostwood"; break; 					
					default: woodType = ""; break;
				}
			}			
			list.Add( "Runic Fletchers Tools" );
			list.Add( woodType );
		}
	}
}