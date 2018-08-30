using System;

namespace Server.Items
{
	public class AcademicBooksE : BaseDecorationArtifact
	{
		public override int ArtifactRarity{ get{ return 8; } }
		
		[Constructable]
		public AcademicBooksE() : base( 7716 )
		{
			Name = "Academic Books";
			Hue = 2008;
		}

		public AcademicBooksE( Serial serial ) : base( serial )
		{
		}
		
		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Daily Rare" );
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