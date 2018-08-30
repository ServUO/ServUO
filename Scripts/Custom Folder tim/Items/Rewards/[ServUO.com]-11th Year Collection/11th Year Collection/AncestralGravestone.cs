using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Events;

namespace Server.Items
{
	[Flipable( 0x1174, 0x1173 )]
	public class AncestralGravestone : Item, ISecurable
	{
		public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

		private static void EventSink_Login( LoginEventArgs e )
		{
			RemoveEffect( e.Mobile );
		}

		private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes( 90.0 );
		private static readonly TimeSpan Duration = TimeSpan.FromMinutes( 20.0 );

		public override int LabelNumber { get { return 1071096; } } // Ancestral Gravestone

		private SecureLevel m_Level;

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get { return m_Level; }
			set { m_Level = value; }
		}

		[Constructable]
		public AncestralGravestone()
			: base( 0x1174 )
		{
			Weight = 10.0;
			LootType = LootType.Blessed;
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			SetSecureLevelEntry.AddTo( from, this, list );
		}

		private static List<Mobile> m_CooldownList = new List<Mobile>();
		private static Dictionary<Mobile, BonusContext> m_Table = new Dictionary<Mobile, BonusContext>();

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsLockedDown )
			{
				if ( !from.InRange( this.Location, 2 ) )
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
					return;
				}

				//IHouse house = HousingHelper.FindHouseAt( from );
				BaseHouse house = BaseHouse.FindHouseAt(from);

				if ( house != null )
				{
					from.PublicOverheadMessage( MessageType.Regular, 0x3B2, 1071131 ); // Praying…

					if ( m_CooldownList.Contains( from ) )
					{
						from.SendLocalizedMessage( 1071145 ); // In order to summon an undead again, you have to wait for at least 90 minutes.
					}
					else
					{
						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback(
							delegate
							{
								// Add the skill mod
								SkillMod mod = new DefaultSkillMod( SkillName.SpiritSpeak, true, 5.0 );
								mod.ObeyCap = true;
								from.AddSkillMod( mod );
								m_Table[from] = new BonusContext( Timer.DelayCall( Duration, new TimerStateCallback<Mobile>( RemoveEffect ), from ), mod );

								// Set the cooldown
								m_CooldownList.Add( from );
								Timer.DelayCall( Cooldown, new TimerCallback( delegate { m_CooldownList.Remove( from ); } ) );

								// Spawn our undead friend :)
								SummonUndead( from );

								from.SendLocalizedMessage( 1071148 ); // You feel a greater sense.
							} ) );
					}
				}
				else
					from.SendLocalizedMessage( 502436 ); // That is not accessible.
			}
			else
				from.SendLocalizedMessage( 502692 ); // This must be in a house and be locked down to work.
		}

		private static void RemoveEffect( Mobile m )
		{
			if ( m_Table.ContainsKey( m ) )
			{
				BonusContext context = m_Table[m];

				if ( context != null )
				{
					if ( context.Timer != null )
						context.Timer.Stop();

					if ( context.Mod != null )
						m.RemoveSkillMod( context.Mod );
				}

				m.SendLocalizedMessage( 1071146 ); // Your sense goes to normal.

				m_Table.Remove( m );
			}
		}

		private static UndeadEntry[] UndeadTable = new UndeadEntry[]
			{
				new UndeadEntry( 0x1A,	0x4001,	"a shade" ),
				new UndeadEntry( 0x94,	0x0,	"a bone magi" ),
				new UndeadEntry( 0x3CA,	0x453,	"a restless soul" ),
				new UndeadEntry( 0x18,	0x0,	"a lich" )
			};

		private static void SummonUndead( Mobile from )
		{
			Mobile undead = UndeadTable[Utility.Random( UndeadTable.Length )].Spawn();

			undead.MoveToWorld( new Point3D( from.X, from.Y + 1, from.Z ), from.Map );
			undead.Say( Utility.Random( 1071135, 10 ) );

			Effects.PlaySound( from.Location, from.Map, 0x17F );
			Effects.SendLocationParticles( from, 0x373A, 1, 17, 0x454, 7, 0x26BA, 0 );
			Effects.SendLocationParticles( from, 0x376A, 1, 22, 0x43, 7, 0x251E, 0 );

			Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerCallback(
				delegate { undead.Delete(); } ) );
		}

		public AncestralGravestone( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );

			writer.Write( (int) m_Level );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version > 0 )
				m_Level = (SecureLevel) reader.ReadInt();
		}

		private class BonusContext
		{
			private Timer m_Timer;
			private SkillMod m_Mod;

			public Timer Timer { get { return m_Timer; } }
			public SkillMod Mod { get { return m_Mod; } }

			public BonusContext( Timer timer, SkillMod mod )
			{
				m_Timer = timer;
				m_Mod = mod;
			}
		}

		private class InternalMobile : BaseCreature
		{
			public InternalMobile( int body, int hue, string name )
				: base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
			{
				Body = body;
				Hue = hue;
				Name = name;

				Blessed = true;
				CantWalk = true;
			}

			public override bool NoHouseRestrictions { get { return true; } }

			public InternalMobile( Serial serial )
				: base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( 0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				/*int version = */
				reader.ReadInt();
			}
		}

		private class UndeadEntry
		{
			private int m_Body;
			private int m_Hue;
			private string m_Name;

			public UndeadEntry( int body, int hue, string name )
			{
				m_Body = body;
				m_Hue = hue;
				m_Name = name;
			}

			public Mobile Spawn()
			{
				return new InternalMobile( m_Body, m_Hue, m_Name );
			}
		}
	}
}