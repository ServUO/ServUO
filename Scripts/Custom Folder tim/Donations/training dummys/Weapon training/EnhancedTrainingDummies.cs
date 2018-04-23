// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
// this one is just a simple mod to the normal training dummies
using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x1070, 0x1074 )]
	public class EnhancedTrainingDummy : AddonComponent
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
		public EnhancedTrainingDummy() : this( 0x1074 ){}

		[Constructable]
		public EnhancedTrainingDummy( int itemID ) : base( itemID )
		{
			m_MinSkill = -25.0;
			m_MaxSkill = 100.0; // this is the only mod done - to make it so it trains to 100 skill
		}

		public void UpdateItemID()
		{
			int baseItemID = (ItemID / 2) * 2;

			ItemID = baseItemID + (Swinging ? 1 : 0);
		}

		public void BeginSwing()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = new InternalTimer( this );
			m_Timer.Start();
		}

		public void EndSwing()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;

			UpdateItemID();
		}

		public void OnHit()
		{
			UpdateItemID();
			Effects.PlaySound( GetWorldLocation(), Map, Utility.RandomList( 0x3A4, 0x3A6, 0x3A9, 0x3AE, 0x3B4, 0x3B6 ) );
		}

		public void Use( Mobile from, BaseWeapon weapon )
		{
			BeginSwing();

			from.Direction = from.GetDirectionTo( GetWorldLocation() );
			weapon.PlaySwingAnimation( from );

			from.CheckSkill( weapon.Skill, m_MinSkill, m_MaxSkill );
		}

		public override void OnDoubleClick( Mobile from )
		{
			BaseWeapon weapon = from.Weapon as BaseWeapon;

			if ( weapon is BaseRanged || weapon is BaseRanged)
				SendLocalizedMessageTo( from, 501822 );
			else if ( weapon == null || !from.InRange( GetWorldLocation(), weapon.MaxRange ) )
				SendLocalizedMessageTo( from, 501816 );
			else if ( Swinging )
				SendLocalizedMessageTo( from, 501815 );
			else if ( from.Skills[weapon.Skill].Base >= m_MaxSkill )
				SendLocalizedMessageTo( from, 501828 );
			else if ( !(weapon.Skill == SkillName.Swords || weapon.Skill == SkillName.Macing || weapon.Skill == SkillName.Lumberjacking
				|| weapon.Skill == SkillName.Fencing || weapon.Skill == SkillName.Wrestling) )
				from.SendMessage( "Only weapons using their normal skill can be trained on this" );
			else if ( from.Mounted )
				SendLocalizedMessageTo( from, 501829 );
			else
				Use( from, weapon );
		}

		public EnhancedTrainingDummy(Serial serial) : base(serial){}
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

			switch ( version )
			{
				case 0:
				{
					m_MinSkill = reader.ReadDouble();
					m_MaxSkill = reader.ReadDouble();
					break;
				}
			}

			UpdateItemID();
		}

		private class InternalTimer : Timer
		{
			private EnhancedTrainingDummy m_Dummy;
			private bool m_Delay = true;

			public InternalTimer( EnhancedTrainingDummy dummy ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.75 ) )
			{
				m_Dummy = dummy;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Delay )
					m_Dummy.OnHit();
				else
					m_Dummy.EndSwing();

				m_Delay = !m_Delay;
			}
		}
	}

	public class EnhancedTrainingDummyEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new EnhancedTrainingDummyEastDeed(); } }

		[Constructable]
		public EnhancedTrainingDummyEastAddon()
		{
			AddComponent( new EnhancedTrainingDummy( 0x1074 ), 0, 0, 0 );
		}

		public EnhancedTrainingDummyEastAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class EnhancedTrainingDummyEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new EnhancedTrainingDummyEastAddon(); } }

		[Constructable]
		public EnhancedTrainingDummyEastDeed()
		{
			Name = "Enhanced Training Dummy East Deed";
		}

		public EnhancedTrainingDummyEastDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class EnhancedTrainingDummySouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new EnhancedTrainingDummySouthDeed(); } }

		[Constructable]
		public EnhancedTrainingDummySouthAddon()
		{
			AddComponent( new EnhancedTrainingDummy( 0x1070 ), 0, 0, 0 );
		}

		public EnhancedTrainingDummySouthAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class EnhancedTrainingDummySouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new EnhancedTrainingDummySouthAddon(); } }

		[Constructable]
		public EnhancedTrainingDummySouthDeed()
		{
			Name = "Enhanced Training Dummy South Deed";
		}

		public EnhancedTrainingDummySouthDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}