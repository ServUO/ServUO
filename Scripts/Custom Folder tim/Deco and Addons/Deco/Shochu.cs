using System;
using Server.Targeting;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	public class Shochu : Item, ISecurable
	{
		private int m_MaxUses;

		private int m_CurrentUses;

		private SecureLevel m_Level;

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get{ return m_Level; }
			set{ m_Level = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxUses
		{
			get{ return m_MaxUses; }
			set
			{
				if ( value < 0 )
					value = 0;
				m_MaxUses = value;
				if ( m_MaxUses < m_CurrentUses )
					m_CurrentUses = m_MaxUses;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CurrentUses
		{
			get{ return m_CurrentUses; }
			set
			{
				if ( value < 0 )
					value = 0;
				else if ( value > m_MaxUses )
					value = m_MaxUses;
				m_CurrentUses = value;
			}
		}

		[Constructable]
		public Shochu() : base( 0x1956 )
		{
			LootType = LootType.Blessed;
			m_CurrentUses = m_MaxUses = 10;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_CurrentUses > 0 )
			{
				if ( from.Thirst < 20 )
					++from.Thirst;
				from.BAC += 5;
				if ( from.BAC > 60 )
					from.BAC = 60;
				BaseBeverage.CheckHeaveTimer( from );
				--m_CurrentUses;
				from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
			}
			else
			{
				from.BeginTarget( -1, true, TargetFlags.None, new TargetCallback( Fill ) );
				SendLocalizedMessageTo( from, 500837 ); // Fill from what?
			}
		}

		public void Fill( Mobile from, object targ )
		{
			if ( m_CurrentUses < 1 && targ is BaseBeverage )
			{
				BaseBeverage bev = (BaseBeverage)targ;

				if ( bev.IsEmpty || !bev.ValidateUse( from, true ) )
					return;

				if ( bev.Quantity > m_MaxUses )
				{
					m_CurrentUses = m_MaxUses;
					bev.Quantity -= m_MaxUses;
				}
				else
				{
					m_CurrentUses += bev.Quantity;
					bev.Quantity = 0;
				}
			}
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public Shochu( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.WriteEncodedInt( m_MaxUses );
			writer.WriteEncodedInt( m_CurrentUses );

			writer.WriteEncodedInt( (int) m_Level );
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_MaxUses = reader.ReadEncodedInt();
			m_CurrentUses = reader.ReadEncodedInt();

			m_Level = (SecureLevel) reader.ReadEncodedInt();
		}
	}
}
