using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic; 
using Server.ContextMenus; 

namespace Server.Items
{
	public abstract class BaseDailyRareFood : BaseDailyRare
	{
		private Mobile m_Poisoner;
		private Poison m_Poison;
		private int m_FillFactor;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Poisoner
		{
			get { return m_Poisoner; }
			set { m_Poisoner = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get { return m_Poison; }
			set { m_Poison = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int FillFactor
		{
			get { return m_FillFactor; }
			set { m_FillFactor = value; }
		}

		public BaseDailyRareFood( int itemID ) : this( 1, itemID )
		{
		}

		public BaseDailyRareFood( int amount, int itemID ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;
			m_FillFactor = 1;
		}

		public BaseDailyRareFood( Serial serial ) : base( serial )
		{
		}
            public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		//public override void GetContextMenuEntries( Mobile from, ArrayList list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new ContextMenus.RareEatEntry( from, this ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable && CantBeLifted == false )
				return;

			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				Eat( from );
			}
		}

		public virtual bool Eat( Mobile from )
		{
			// Fill the Mobile with FillFactor
			if ( FillHunger( from, m_FillFactor ) )
			{
				// Play a random "eat" sound
				from.PlaySound( Utility.Random( 0x3A, 3 ) );

				if ( from.Body.IsHuman && !from.Mounted )
					from.Animate( 34, 5, 1, true, false, 0 );

				if ( m_Poison != null )
					from.ApplyPoison( m_Poisoner, m_Poison );

				Consume();

				return true;
			}

			return false;
		}

		static public bool FillHunger( Mobile from, int fillFactor )
		{
			if ( from.Hunger >= 20 )
			{
				from.SendLocalizedMessage( 500867 ); // You are simply too full to eat any more!
				return false;
			}
			
			int iHunger = from.Hunger + fillFactor;
			if ( from.Stam < from.StamMax )
				from.Stam += Utility.Random( 6, 3 ) + fillFactor/5;//restore some stamina
			if ( iHunger >= 20 )
			{
				from.Hunger = 20;
				from.SendLocalizedMessage( 500872 ); // You manage to eat the BaseDailyRareFood, but you are stuffed!
			}
			else
			{
				from.Hunger = iHunger;

				if ( iHunger < 5 )
					from.SendLocalizedMessage( 500868 ); // You eat the BaseDailyRareFood, but are still extremely hungry.
				else if ( iHunger < 10 )
					from.SendLocalizedMessage( 500869 ); // You eat the BaseDailyRareFood, and begin to feel more satiated.
				else if ( iHunger < 15 )
					from.SendLocalizedMessage( 500870 ); // After eating the BaseDailyRareFood, you feel much less hungry.
				else
					from.SendLocalizedMessage( 500871 ); // You feel quite full after consuming the BaseDailyRareFood.
			}

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version

			writer.Write( m_Poisoner );

			Poison.Serialize( m_Poison, writer );
			writer.Write( m_FillFactor );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					switch ( reader.ReadInt() )
					{
						case 0: m_Poison = null; break;
						case 1: m_Poison = Poison.Lesser; break;
						case 2: m_Poison = Poison.Regular; break;
						case 3: m_Poison = Poison.Greater; break;
						case 4: m_Poison = Poison.Deadly; break;
					}

					break;
				}
				case 2:
				{
					m_Poison = Poison.Deserialize( reader );
					break;
				}
				case 3:
				{
					m_Poison = Poison.Deserialize( reader );
					m_FillFactor = reader.ReadInt();
					break;
				}
				case 4:
				{
					m_Poisoner = reader.ReadMobile();
					goto case 3;
				}
			}
		}
	}
}