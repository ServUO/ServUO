// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
// these scripts use the same base as the anatomy ones, please see them for how to modify
using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	[Flipable( 0x1070, 0x1074 )]
	public class EnhancedTacticsDummy : AddonComponent
	{
		private double m_MinSkill;
		private double m_MaxSkill;

		private Timer m_Timer;

		[CommandProperty( AccessLevel.GameMaster )]
		public double MinSkill
		{
			get{ return m_MinSkill; }
			set{ m_MinSkill = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double MaxSkill
		{
			get{ return m_MaxSkill; }
			set{ m_MaxSkill = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Swinging
		{
			get{ return ( m_Timer != null ); }
		}

		[Constructable]
		public EnhancedTacticsDummy() : this( 0x1074 ){}

		[Constructable]
		public EnhancedTacticsDummy( int itemID ) : base( itemID )
		{
			m_MinSkill = 0.0;
			m_MaxSkill = +100.0;
			Name = "Enhanced Tactics Trainer";
		}

		public void UpdateItemID()
		{
			int baseItemID = (ItemID / 2) * 2;
			ItemID = baseItemID + (Swinging ? 1 : 0);
		}

		public void BeginSwing()
		{
			if ( m_Timer != null ) m_Timer.Stop();
			m_Timer = new InternalTimer( this );
			m_Timer.Start();
		}

		public void EndSwing()
		{
			if ( m_Timer != null ) m_Timer.Stop();
			m_Timer = null;
			UpdateItemID();
		}

		public void OnHit()
		{
			UpdateItemID();
			Effects.PlaySound( GetWorldLocation(), Map, Utility.RandomList( 0x3A4, 0x3A6, 0x3A9, 0x3AE, 0x3B4, 0x3B6 ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 1 ) ) from.SendMessage( "You need to be closer to see the question" );
			else if ( Swinging ) from.SendMessage( "Slow down, the teacher is still setting up the next question" );
			else if ( from.Skills.Tactics.Base >= m_MaxSkill ) from.SendMessage( "Your Skill is to high to train here" );
			else if ( from.Mounted ) from.SendMessage( "You can not train while mounted, the teacher does not like the mess they leave" );
			else
			{
				BeginSwing();
				from.Direction = from.GetDirectionTo( GetWorldLocation() );
				from.CloseAllGumps();
				from.SendGump( new TacticsTrainingGump() );
			}
		}

		public EnhancedTacticsDummy(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_MinSkill );
			writer.Write( m_MaxSkill );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_MinSkill = reader.ReadDouble();
			m_MaxSkill = reader.ReadDouble();
			UpdateItemID();
		}

		private class InternalTimer : Timer
		{
			private EnhancedTacticsDummy m_Dummy;
			private bool m_Delay = true;

			public InternalTimer( EnhancedTacticsDummy dummy ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.75 ) )
			{
				m_Dummy = dummy;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Delay ) m_Dummy.OnHit();
				else m_Dummy.EndSwing();
				m_Delay = !m_Delay;
			}
		}
	}

	public class EnhancedTacticsDummyEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new EnhancedTacticsDummyEastDeed(); } }

		[Constructable]
		public EnhancedTacticsDummyEastAddon()
		{
			AddComponent( new EnhancedTacticsDummy( 0x1074 ), 0, 0, 0 );
		}

		public EnhancedTacticsDummyEastAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class EnhancedTacticsDummyEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new EnhancedTacticsDummyEastAddon(); } }

		[Constructable]
		public EnhancedTacticsDummyEastDeed()
		{
			Name = "Enhanced Tactics Training Dummy (East)";
		}

		public EnhancedTacticsDummyEastDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class EnhancedTacticsDummySouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new EnhancedTacticsDummySouthDeed(); } }

		[Constructable]
		public EnhancedTacticsDummySouthAddon()
		{
			AddComponent( new EnhancedTacticsDummy( 0x1070 ), 0, 0, 0 );
		}

		public EnhancedTacticsDummySouthAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class EnhancedTacticsDummySouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new EnhancedTacticsDummySouthAddon(); } }

		[Constructable]
		public EnhancedTacticsDummySouthDeed()
		{
			Name = "Enhanced Tactics Training Dummy (South)";
		}

		public EnhancedTacticsDummySouthDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}