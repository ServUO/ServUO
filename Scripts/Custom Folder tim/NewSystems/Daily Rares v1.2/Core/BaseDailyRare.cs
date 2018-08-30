using System;

namespace Server.Items
{
	public abstract class BaseDailyRare : Item
	{
		private bool m_HasBeenMoved;
		private bool m_CantBeLifted;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasBeenMoved
		{
			get { return m_HasBeenMoved; }
			set { m_HasBeenMoved = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool CantBeLifted
		{
			get { return m_CantBeLifted; }
			set { m_CantBeLifted = value; }
		}

         	public virtual int ArtifactRarity 
      		{ 
         		get{ return 0; } 
      		} 

      		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; } }

		public BaseDailyRare( int itemID ) : base( itemID )
		{
			Movable = false;
			Weight = 10.0;
		}

   		public override void GetProperties( ObjectPropertyList list ) 
      		{ 
         		base.GetProperties( list ); 


         		if ( ArtifactRarity > 0 ) 
            			list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~ - display AR 
      		}

		public override bool VerifyMove( Mobile from ) 
		{
			if ( m_CantBeLifted == true ) // For fruit baskets that require you to eat fruit before taking basket.
				return false;

			else if ( this.Movable == false && m_HasBeenMoved == false )
			{
				this.Movable = true;
				m_HasBeenMoved = true;
				return true;
			}

			return Movable;       
		} 

		public BaseDailyRare( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_HasBeenMoved );
			writer.Write( m_CantBeLifted );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_HasBeenMoved = reader.ReadBool();
					m_CantBeLifted = reader.ReadBool();
					break;
				}
			}
		}
	}

	public abstract class BaseDailyRareCont : Container
	{
		private bool m_HasBeenMoved;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasBeenMoved
		{
			get { return m_HasBeenMoved; }
			set { m_HasBeenMoved = value; }
		}

         	public virtual int ArtifactRarity 
      		{ 
         		get{ return 0; } 
      		} 

      		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; } }

		public BaseDailyRareCont( int itemID ) : base( itemID )
		{
			Movable = false;
			Weight = 10.0;
		}

   		public override void GetProperties( ObjectPropertyList list ) 
      		{ 
         		base.GetProperties( list ); 


         		if ( ArtifactRarity > 0 ) 
            			list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~ - display AR 
      		}

		public override bool VerifyMove( Mobile from ) 
		{
			if ( this.Movable == false && m_HasBeenMoved == false )
			{
				this.Movable = true;
				m_HasBeenMoved = true;
				return true;
			}

			return Movable;       
		} 

		public BaseDailyRareCont( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_HasBeenMoved );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_HasBeenMoved = reader.ReadBool();
					break;
				}
			}
		}
	}
}