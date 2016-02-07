using System;
using Server.Items;

namespace Server.Items
{

	public class SimpleArtifact : Artifact
	{
		private int m_ArtifactRarity = 0;

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ArtifactRarity
		{ 
			get{ return m_ArtifactRarity; }
			set {m_ArtifactRarity = value; InvalidateProperties();}
		}

		[Constructable]
		public SimpleArtifact(int itemID) : base(itemID)
		{
		}

		public SimpleArtifact( Serial serial ) : base( serial )
		{
		}
 

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_ArtifactRarity );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_ArtifactRarity = reader.ReadInt();

		}
	}
}
