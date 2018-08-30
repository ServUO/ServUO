
using System;
using Server;
using Server.Items;

namespace Server.Items
{
		public class WhiteWizardsCloak : BaseCloak
	{

		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 9; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 5; } }

		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 30; } }

		public override int BaseStrBonus{ get{ return -4; } }
		public override int BaseDexBonus{ get{ return -4; } }
		public override int BaseIntBonus{ get{ return +4; } }

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
				((Mobile)parent).VirtualArmorMod += 2;
		}

		public override void OnRemoved(object parent)
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
				((Mobile)parent).VirtualArmorMod -= 2;
		}

		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		[Constructable]
		public WhiteWizardsCloak() : this( 0 )
		{
		}

		[Constructable]
		public WhiteWizardsCloak( int hue ) : this( hue, 0 )
		{
		}

		[Constructable]
		public WhiteWizardsCloak( int hue, int labelNumber ) : base( 0x1515, hue )
		{
			Name = "White Wizards Cloak";
			Weight = 5.0;
			LootType = LootType.Blessed;
                        Hue = 1153;
		}

		public WhiteWizardsCloak( Serial serial ) : base( serial )
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

			if ( Parent is Mobile )
				((Mobile)Parent).VirtualArmorMod += 2;
		}
	}
}
