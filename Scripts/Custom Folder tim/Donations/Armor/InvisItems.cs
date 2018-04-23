using System;

namespace Server.Items
{
	public class InvisRing : BaseRing
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + m_Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisRing() : this( 0 )
		{
		}

		[Constructable]
		public InvisRing( int hue ) : base( 0x108a )
		{
			Weight = 4.0;
			Name = "Ring of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisRing( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();

		}
	}

	public class InvisBoots : BaseShoes
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + m_Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisBoots() : this( 0 )
		{
		}

		[Constructable]
		public InvisBoots( int hue ) : base( 0x170B )
		{
			Weight = 4.0;
			Name = "Boots of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisBoots( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();

		}
	}

	public class InvisBrace : BaseBracelet
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisBrace() : this( 0 )
		{
		}

		[Constructable]
		public InvisBrace( int hue ) : base( 0x1086 )
		{
			Weight = 4.0;
			Name = "Bracelet of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisBrace( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();

		}
	}

	[FlipableAttribute( 0x2309, 0x230A )]
	public class InvisCloak : BaseCloak
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisCloak() : this( 0 )
		{
		}

		[Constructable]
		public InvisCloak( int hue ) : base( 0x1515, hue )
		{
			Weight = 4.0;
			Name = "Cloak of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisCloak( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();

		}
	}

	public class InvisEarrings : BaseEarrings
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisEarrings() : this( 0 )
		{
		}

		[Constructable]
		public InvisEarrings( int hue ) : base( 0x1087 )
		{
			Weight = 4.0;
			Name = "Earrings of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisEarrings( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();

		}
	}

	public class InvisHat : BaseHat
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisHat() : this( 0 )
		{
		}

		[Constructable]
		public InvisHat( int hue ) : base( 0x1716 )
		{
			Weight = 4.0;
			Name = "Straw hat of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisHat( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();

		}
	}

	public class InvisSash : BaseMiddleTorso
	{

		public override void OnSingleClick( Mobile from )
		{
			this.LabelTo( from, Name + "(charges:" + Charges.ToString() + ")" );
		}

		private int m_Charges;
		[Constructable]
		public InvisSash() : this( 0 )
		{
		}

		[Constructable]
		public InvisSash( int hue ) : base( 0x1541 )
		{
			Weight = 4.0;
			Name = "Body Sash of Invisibility";
			Charges = Utility.Random( 3, 43 );
		}

		public InvisSash( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; }
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				if ( Charges <= 0 )
				{
					((Mobile)parent).SendMessage( "That item is out of charges!" );
					return;
				}

				Charges--;
				((Mobile)parent).Hidden = true;

			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				((Mobile)parent).Hidden = false;
			}
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( (int) m_Charges );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Charges = (int)reader.ReadInt();
		}
	}
}