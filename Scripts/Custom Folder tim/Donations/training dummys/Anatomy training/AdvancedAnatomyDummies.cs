// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
// is is just a simple mod to the normal anatomy training dummy, please see it for how to modify
using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
	[Flipable( 0x1070, 0x1074 )]
	public class AdvancedAnatomyDummy : AddonComponent
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
		public AdvancedAnatomyDummy() : this( 0x1074 ){}

		[Constructable]
		public AdvancedAnatomyDummy( int itemID ) : base( itemID )
		{
			m_MinSkill = 100.0;
			m_MaxSkill = 200.0; // just changing our min & max skills allowed on here
			Name = "Advanced Anatomy Trainer";
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
			else if ( from.Skills.Anatomy.Base >= m_MaxSkill ) from.SendMessage( "Your Skill is to high to train here" );
			else if ( from.Mounted ) from.SendMessage( "You can not train while mounted, the teacher does not like the mess they leave" );
			else
			{
				BeginSwing();
				from.Direction = from.GetDirectionTo( GetWorldLocation() );
				from.CloseAllGumps();
				from.SendGump( new AnatomyTrainingGump() );
			}
		}

		public AdvancedAnatomyDummy(Serial serial) : base(serial){}
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
			private AdvancedAnatomyDummy m_Dummy;
			private bool m_Delay = true;

			public InternalTimer( AdvancedAnatomyDummy dummy ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.75 ) )
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

	public class AdvancedAnatomyDummyEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new AdvancedAnatomyDummyEastDeed(); } }

		[Constructable]
		public AdvancedAnatomyDummyEastAddon()
		{
			AddComponent( new AdvancedAnatomyDummy( 0x1074 ), 0, 0, 0 );
		}

		public AdvancedAnatomyDummyEastAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class AdvancedAnatomyDummyEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new AdvancedAnatomyDummyEastAddon(); } }

		[Constructable]
		public AdvancedAnatomyDummyEastDeed()
		{
			Name = "Advanced Anatomy Training Dummy (East)";
		}

		public AdvancedAnatomyDummyEastDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class AdvancedAnatomyDummySouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new AdvancedAnatomyDummySouthDeed(); } }

		[Constructable]
		public AdvancedAnatomyDummySouthAddon()
		{
			AddComponent( new AdvancedAnatomyDummy( 0x1070 ), 0, 0, 0 );
		}

		public AdvancedAnatomyDummySouthAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class AdvancedAnatomyDummySouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new AdvancedAnatomyDummySouthAddon(); } }

		[Constructable]
		public AdvancedAnatomyDummySouthDeed()
		{
			Name = "Advanced Anatomy Training Dummy (South)";
		}

		public AdvancedAnatomyDummySouthDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}