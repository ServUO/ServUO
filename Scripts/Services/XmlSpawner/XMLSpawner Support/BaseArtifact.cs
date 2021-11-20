using System;
using Server.Items;

namespace Server.Items
{

	public abstract class Artifact : Item
	{

		[Constructable]
		public Artifact(int itemID) : base(itemID)
		{
		}

		public Artifact( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int ArtifactRarity
		{ 
			get{ return 0; }
			set {}
		}

		public override bool ForceShowProperties{ get{ return true; } }
            
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			if(ArtifactRarity > 0)
				list.Add( 1061078, ArtifactRarity.ToString() );
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
