using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
	public class TownCrierb : BaseCreature
	{
		private static string path = "Data/news.txt";

		private int m_Count;
		private Timer m_Timer;
		private bool m_StoneActive;
		private bool m_Custom;
		private bool m_Active;
		private bool m_Random;
		private TimeSpan m_Delay;
		private TownCrierStone m_Stone;

		[CommandProperty( AccessLevel.Seer )]
		public TimeSpan Delay{ get{ return m_Delay; } set{ m_Delay = value; } }

		[CommandProperty( AccessLevel.Seer )]
		public TownCrierStone Stone
		{
			get{ return m_Stone; }
			set
			{
				m_Stone = value;

				TownCrierStone stone = m_Stone as TownCrierStone;

				if ( stone != null )
				{
					this.StoneActive = stone.Active;
					TownCrierb m_Crier = this as TownCrierb;
					stone.Registry.Add( m_Crier );
				}
			}
		}

		[CommandProperty( AccessLevel.Seer )]
		public bool Custom
		{
			get{ return m_Custom; }
			set
			{
				m_Custom = value;
				this.OnActivate();
			}
		}

		[CommandProperty( AccessLevel.Seer )]
		public bool Active
		{
			get
			{
				if ( !this.Custom )
				{
					m_Active = false;
					return m_Active;
				}
				else
				{
					return m_Active;
				}
			}
			set
			{
				m_Active = value;
				this.OnActivate();
			}
		}

		[CommandProperty( AccessLevel.Seer )]
		public bool Random
		{
			get
			{
				if ( !this.Custom )
				{
					m_Random = false;
					return m_Random;
				}
				else
				{
					return m_Random;
				}
			}
			set { m_Random = value; }
		}

		public int Count{ get{ return m_Count; } set{ m_Count = value; } }

		public bool StoneActive
		{
			get{ return m_StoneActive; }
			set
			{
				m_StoneActive = value;
				this.OnActivate();
			}
		}

		[Constructable]
		public TownCrierb() : base( AIType.AI_Thief, FightMode.None, 10, 1, 0.8, 1.6 )
		{
			InitStats( 100, 100, 25 );

			Title = "the town crier";
			Name = NameList.RandomName( "female" );
			Female = true;
			Body = 0x191;
			Hue = Utility.RandomSkinHue();
			NameHue = 0x35;
			m_Stone = null;
			m_StoneActive = false;
			m_Active = false;
			m_Custom = false;
			m_Random = false;
			m_Delay = new TimeSpan( 0, 0, 10 );
			this.Blessed = true;

			AddItem( new FancyShirt( Utility.RandomBlueHue() ) );

			Item skirt;

			switch ( Utility.Random( 2 ) )
			{
				case 0: skirt = new Skirt(); break;
				default: case 1: skirt = new Kilt(); break;
			}

			skirt.Hue = Utility.RandomGreenHue();

			AddItem( skirt );

			AddItem( new FeatheredHat( Utility.RandomGreenHue() ) );

			Item boots;

			switch ( Utility.Random( 2 ) )
			{
				case 0: boots = new Boots(); break;
				default: case 1: boots = new ThighBoots(); break;
			}

			AddItem( boots );

			HairItemID = 0x203C;   // Long hair
            HairHue = 1175;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.Seer )
				from.SendGump( new TownCrierbGump( null, this ) );
			else
				base.OnDoubleClick( from );
		}
      
		public override void OnDelete()
		{
			TownCrierStone stone = m_Stone as TownCrierStone;
			if ( stone != null )
			{
				TownCrierb m_Crier = this as TownCrierb;
				stone.Registry.Remove( m_Crier );
			}
			base.OnDelete();
		}

		public override void OnSpeech( SpeechEventArgs args )
		{
			if( args.Mobile.InRange( this, 4 ))
			{
				if ( args.Speech.ToLower().IndexOf( "news" ) >= 0 )
				{
					string message = SpamMessage( this );
					this.Say( message );
				}
			}
		}

		public virtual void OnActivate()
		{
			TownCrierStone stone = Stone as TownCrierStone;
			TimeSpan delay;

			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}

			if ( m_Timer == null )
			{
				if ( !Custom && (stone != null) && stone.Active )
				{
					delay = stone.Delay;
					m_Timer = new SpamTimer( this, delay );
					m_Timer.Start();
				}
				else if ( Active )
				{
					delay = Delay;
					m_Timer = new SpamTimer( this, delay );
					m_Timer.Start();
				}
			}
		}

		public class SpamTimer : Timer
		{
			private TownCrierb m_Crier;

			public SpamTimer( TownCrierb crier, TimeSpan m_Delay ) : base( TimeSpan.Zero, m_Delay )
			{
				m_Crier = crier as TownCrierb;
			}

			protected override void OnTick()
			{
				string message = SpamMessage( m_Crier );
				m_Crier.Say( message );
			}
		}

		public static string SpamMessage( TownCrierb crier )
		{
			ArrayList m_Lines = new ArrayList();

			if ( File.Exists( path ) )
			{
				using ( StreamReader ip = new StreamReader( path ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						if ( line.Length > 0 )
							m_Lines.Add( line );
					}
				}
			}

			string message;

			if ( m_Lines.Count == 0 )
			{
				message = "I have no news at this time.";
			}
			else if ( (crier.Custom && crier.Random) || (crier.Stone != null && !crier.Custom && crier.Stone.Random) )
			{
				int i = Utility.Random( m_Lines.Count );
				message = "Hear ye! Hear ye! "+ m_Lines[i];
			}
			else
			{
				try
				{
					message = "Hear ye! Hear ye! "+ m_Lines[crier.Count++];
					if ( crier.Count == m_Lines.Count )
					crier.Count = 0;
				}
				catch
				{
					crier.Count = 0;
					message = "Hear ye! Hear ye! "+ m_Lines[crier.Count++];
				}
			}
			return message;
		}

		public TownCrierb( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Custom );
			writer.Write( m_Random );
			writer.Write( m_Delay );
			writer.Write( this.Frozen );
			writer.Write( m_Stone );
			writer.Write( m_Active );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_Custom = reader.ReadBool();
					m_Random = reader.ReadBool();
					m_Delay = reader.ReadTimeSpan();
					this.Frozen = reader.ReadBool();
					m_Stone = reader.ReadItem() as TownCrierStone;
					m_Active = reader.ReadBool();

					break;
				}
			}
		}
	}
}
