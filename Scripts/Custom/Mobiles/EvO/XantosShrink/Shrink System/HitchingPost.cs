#region AuthorHeader
//
//	Shrink System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.ShrinkSystem
{
	[Flipable( 0x14E8, 0x14E7 )]
	public class HitchingPost : AddonComponent, IShrinkTool
	{
		private int m_Charges = ShrinkConfig.ShrinkCharges;

		[CommandProperty( AccessLevel.GameMaster )]
		public int ShrinkCharges
		{
			get { return m_Charges; }
			set
			{
				if ( 0 == m_Charges || 0 == (m_Charges = value ))
					Delete();
				else
					InvalidateProperties();
			}
		}

		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; }}

		[Constructable]
		public HitchingPost() : this( 0x14E7 )
		{
		}

		[Constructable]
		public HitchingPost( int itemID ) : base( itemID )
		{
		}

		public HitchingPost( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			if ( m_Charges >= 0 )
				list.Add( 1060658, "Charges\t{0}", m_Charges.ToString() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( from.InRange( this.GetWorldLocation(), 2 ) == false )
				from.SendLocalizedMessage( 500486 ); //That is too far away.
			
			else if ( from.Skills[SkillName.AnimalTaming].Value >= ShrinkConfig.TamingRequired )
				from.Target = new ShrinkTarget( from, this, false );
				
			else
				from.SendMessage( "You must have at least " + ShrinkConfig.TamingRequired + " animal taming to use a hitching post." );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( m_Charges );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Charges = reader.ReadInt();
		}
	}

	public class HitchingPostEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new HitchingPostEastDeed(); }}

		[Constructable]
		public HitchingPostEastAddon()
		{
			AddComponent( new HitchingPost( 0x14E7 ), 0, 0, 0);
		}

		public HitchingPostEastAddon( Serial serial ) : base( serial )
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

	public class HitchingPostEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new HitchingPostEastAddon(); }}

		[Constructable]
		public HitchingPostEastDeed()
		{
			Name = "Hitching Post (east)";
		}

		public HitchingPostEastDeed( Serial serial ) : base( serial )
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


	public class HitchingPostSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new HitchingPostSouthDeed(); }}

		[Constructable]
		public HitchingPostSouthAddon()
		{
			AddComponent( new HitchingPost( 0x14E8 ), 0, 0, 0);
		}

		public HitchingPostSouthAddon( Serial serial ) : base( serial )
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

	public class HitchingPostSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new HitchingPostSouthAddon(); }}

		[Constructable]
		public HitchingPostSouthDeed()
		{
			Name = "Hitching Post (south)";
		}

		public HitchingPostSouthDeed( Serial serial ) : base( serial )
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
