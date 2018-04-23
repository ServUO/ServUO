using System;
using Server;
using Server.Network;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	[Flipable( 0x1950, 0x1953, 0x1950, 0x1951, 0x1954, 0x1951 )] // locked down cant be used as player
	public class MongbatDartboard : Item, ISecurable
	{
		private DateTime m_NextSound = DateTime.Now;

		private bool IsFacingEast
		{
			get{ return ItemID > 0x1952; }
		}

		private SecureLevel m_Level;

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get{ return m_Level; }
			set{ m_Level = value; }
		}

		[Constructable]
		public MongbatDartboard() : base( 0x1950 )
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			Direction dir;
			if ( from.Location != this.Location )
				dir = from.GetDirectionTo( this );
			else if ( IsFacingEast )
				dir = Direction.West;
			else
				dir = Direction.North;

			from.Direction = dir;

			bool canThrow = true;

			if ( !from.InRange( this, 4 ) || !from.InLOS( this ) )
				canThrow = false;
			else if ( IsFacingEast )
				canThrow = ( dir == Direction.Left || dir == Direction.West || dir == Direction.Up );
			else
				canThrow = ( dir == Direction.Up || dir == Direction.North || dir == Direction.Right );

			if ( canThrow )
				Throw( from );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}

		public void Throw( Mobile from )
		{
			BaseKnife knife = from.Weapon as BaseKnife;

			if ( knife == null )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500751 ); // Try holding a knife...
				return;
			}

			from.Animate( from.Mounted ? 26 : 9, 7, 1, true, false, 0 );
			from.MovingEffect( this, knife.ItemID, 7, 1, false, false );
			from.PlaySound( 0x238 );

			double rand = Utility.RandomDouble();

			int message;
			if ( rand < 0.05 )
				message = 500752; // BULLSEYE! 50 Points!
			else if ( rand < 0.20 )
				message = 500753; // Just missed the center! 20 points.
			else if ( rand < 0.45 )
				message = 500754; // 10 point shot.
			else if ( rand < 0.70 )
				message = 500755; // 5 pointer.
			else if ( rand < 0.85 )
				message = 500756; // 1 point.  Bad throw.
			else
				message = 500757; // Missed.

			PublicOverheadMessage( MessageType.Regular, 0x3B2, message );
			if ( m_NextSound < DateTime.Now && message != 500757 )
			{
				if ( message == 500752 )
					Effects.PlaySound( Location, Map, 426 );
				else
					Effects.PlaySound( Location, Map, 425 );

				if ( message < 500756 )
					AnimateMongbat();

				m_NextSound = DateTime.Now + TimeSpan.FromSeconds( 2 );
			}
		}

		private void AnimateMongbat()
		{
			if ( Deleted )
				return;

			if ( ItemID == 0x1950 || ItemID == 0x1953 )
			{
				++ItemID;
				Timer.DelayCall( TimeSpan.FromSeconds( 1 ), new TimerCallback( AnimateMongbat ) );
			}
			else if ( ItemID == 0x1951 || ItemID == 0x1954 )
				--ItemID;
			else
				ItemID = IsFacingEast ? 0x1953 : 0x1950;
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public MongbatDartboard( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_Level );
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Level = (SecureLevel) reader.ReadEncodedInt();

			if ( IsFacingEast ) // Must be done because the Item can be saved while animating
				ItemID = 0x1953;
			else
				ItemID = 0x1950;
		}
	}
}
