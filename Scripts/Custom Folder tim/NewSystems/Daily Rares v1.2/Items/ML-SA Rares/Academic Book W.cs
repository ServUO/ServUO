using System;

namespace Server.Items
{
	public class AcademicBooksW : BaseDecorationArtifact
	{
		public override int ArtifactRarity{ get{ return 8; } }
		
		[Constructable]
		public AcademicBooksW() : base( 7717 )
		{
			Name = "Academic Books";
			Hue = 2008;
		}

		public AcademicBooksW( Serial serial ) : base( serial )
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