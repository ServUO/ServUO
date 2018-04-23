
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x170b, 0x170c )]
	public class WhiteWizardsBoots : BaseShoes
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 9; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 5; } }

		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 30; } }

		public override int BaseStrBonus{ get{ return -3; } }
		public override int BaseDexBonus{ get{ return -3; } }
		public override int BaseIntBonus{ get{ return +3; } }

		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		[Constructable]
		public WhiteWizardsBoots() : this( 0 )
		{
		}

		[Constructable]
		public WhiteWizardsBoots( int hue ) : base( 0x170b, hue )
		{
			Name = "White Wizards Boots";
			Weight = 2.0;
                        Hue = 1153;
			LootType = LootType.Blessed;
		}

		public WhiteWizardsBoots( Serial serial ) : base( serial )
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
	}
}