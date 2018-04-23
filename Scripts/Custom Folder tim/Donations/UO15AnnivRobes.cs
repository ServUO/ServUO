using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x4B9F, 0x4BA0 )]
    public class UO15AnnivGargish : BaseOuterTorso, ITokunoDyable
	{
		public override Race RequiredRace { get { return Race.Gargoyle; } }
	    public override bool CanBeWornByGargoyles{ get{ return true; } }

		[Constructable]
		public UO15AnnivGargish() : base( 0x4B9F )
		{
		    Name = "Gargish Robe";
			Weight = 3.0;
			Hue = 0;
			Attributes.Luck = 150;
			Attributes.RegenStam = 2;
			Attributes.DefendChance = 15;

		}

		public UO15AnnivGargish( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060662, "Ultima Online\t15th Anniversary" );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
    
    [Flipable( 0x4B9D, 0x4B9E )]
    public class UO15Anniv : BaseOuterTorso, ITokunoDyable
	{

		[Constructable]
		public UO15Anniv() : base( 0x4B9D )
		{
			Weight = 3.0;
			Hue = 0;
			Attributes.Luck = 150;
			Attributes.RegenStam = 2;
			Attributes.DefendChance = 15;

		}

		public UO15Anniv( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060662, "Ultima Online\t15th Anniversary" );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}