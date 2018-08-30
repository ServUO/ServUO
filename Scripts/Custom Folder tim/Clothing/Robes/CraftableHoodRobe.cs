/*Scripted by  _____         
*	  		   \_   \___ ___ 
*			    / /\/ __/ _ \
*		     /\/ /_| (_|  __/
*			 \____/ \___\___|
*/

using System;
using Server.Items;

namespace Server.Items
{
	public class CraftableHoodRobe : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 4; } }
		public override int BaseFireResistance{ get{ return 1; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }

		public override int AosStrReq{ get{ return 25; } }
		public override int OldStrReq{ get{ return 15; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		[Constructable]
		public CraftableHoodRobe() : base( 0x2684 )
		{
			Weight = 10.0;
			Name = "Leather Hood Robe";
			Hue = 1823;
			Layer = Layer.OuterTorso;
		}

		public CraftableHoodRobe( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDoubleClick( Mobile m )
      	{
        	if( Parent != m )
         	{
            	m.SendMessage( "You must be wearing the robe to use it!" );
         	}
         	else
        	{
            	if ( ItemID == 0x2683 || ItemID == 0x2684 )
            	{
               		m.SendMessage( "You lower the hood." );
               		m.PlaySound( 0x57 );
               		ItemID = 0x1F03;
               		m.RemoveItem(this);
               		m.EquipItem(this);
            	}
            	else if ( ItemID == 0x1F03 || ItemID == 0x1F04 )
            	{
              		m.SendMessage( "You pull the hood over your head." );
               		m.PlaySound( 0x57 );
               		ItemID = 0x2683;
               		m.RemoveItem(this);
               		m.EquipItem(this);
				}
			}
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
			if ( Weight == 1.0 )
				 Weight = 10.0;
		}
	}
}
