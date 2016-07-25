using System;
using Server.Network;

namespace Server.Items
{
	public class GoblinTrap : BaseTrap
	{
		private Timer m_Concealing;

		public TimeSpan ConcealPeriod { get { return TimeSpan.FromSeconds( 30.0 ); } }

		public override bool PassivelyTriggered { get { return false; } }
		public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.Zero; } }
		public override int PassiveTriggerRange { get { return 0; } }
		public override TimeSpan ResetDelay { get { return TimeSpan.FromSeconds( 0.0 ); } }

		[Constructable]
		public GoblinTrap()
			: base( 0x4004 )
		{
			Visible = false;
		}

		[Constructable]
		public GoblinTrap( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			if ( Visible )
				BeginConceal();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();

			if ( Visible )
				BeginConceal();
		}

		public virtual void BeginConceal()
		{
			if ( m_Concealing != null )
				m_Concealing.Stop();

			m_Concealing = Timer.DelayCall( ConcealPeriod, new TimerCallback( Conceal ) );
		}

		public virtual void Conceal()
		{
			if ( m_Concealing != null )
				m_Concealing.Stop();

			m_Concealing = null;

			if ( !Deleted )
				Visible = false;
		}

		public override void OnTrigger( Mobile from )
		{
			if ( !from.IsPlayer() || !from.Alive || from.Flying || from.AccessLevel > AccessLevel.Player )
			{
				if ( m_Concealing == null && Visible )
					Visible = false;

				return;
			}

			AOS.Damage( from, Utility.RandomMinMax( 30, 70 ), 100, 0, 0, 0, 0 );

			Effects.PlaySound( from.Location, from.Map, 0x22B );
			from.LocalOverheadMessage( MessageType.Regular, 0x22, 1095157 ); // You stepped onto a goblin trap!

			BeginConceal();
		}

		internal void ExecuteTrap( Mobile from )
		{
			AOS.Damage( from, Utility.RandomMinMax( 30, 70 ), 100, 0, 0, 0, 0 );

			Effects.PlaySound( from.Location, from.Map, 0x22B );

			BeginConceal();
		}
	}
}
