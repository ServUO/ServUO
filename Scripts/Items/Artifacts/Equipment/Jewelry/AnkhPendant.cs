using System;
using Server;

namespace Server.Items
{
	public class AnkhPendant : BaseNecklace
	{
		private static bool AllowIlshenarShrines = false;
		private bool m_IsUseful;
        public override bool IsArtifact { get { return true; } }

        public override int LabelNumber{ get{ return 1079525; } } // Ankh Pendant
		private DateTime m_LastUse;
		public int VirtueEffect;
		private Timer m_Timer;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsUseful
		{
			get { return m_IsUseful; }
			set { m_IsUseful = value; }
		}

		[Constructable]
		public AnkhPendant() : base( 0x3BB5 ) //15285
		{
			Weight = 0.1;
			VirtueEffect = 0;
			m_IsUseful = false;
		}

		public override bool HandlesOnSpeech{ get{ return (Parent is Mobile); } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && Parent is Mobile )
			{
				Mobile m = Parent as Mobile;

				if ( e.Mobile.Serial != m.Serial )
					return;

				string theirwords = e.Speech;
				theirwords.ToLower();
				int chant = 99;

				for ( int i = 0; i < m_ShrineMantra.Length; i++ )
				{
					if ( theirwords == m_ShrineMantra[i] )
						chant = i;
				}

				if ( chant >= m_ShrineMantra.Length )
					return;

				bool disabled = false;
				Point3D[] place = m_BritanniaShrines;

				if ( e.Mobile.Map == Map.Ilshenar )
				{
					if ( !AllowIlshenarShrines )
						disabled = true;
					else
						place = m_IllshShrines;
				}
				else if ( e.Mobile.Map != Map.Felucca && e.Mobile.Map != Map.Trammel )
					disabled = true;

				if ( disabled == true )
					return;

				bool rightplace = false;

				foreach ( Mobile mobile in e.Mobile.Map.GetMobilesInRange( place[chant], 5 ) )
					if ( mobile != null )
						if ( mobile.Serial == e.Mobile.Serial )
							rightplace = true;

				if ( rightplace == true )
				{
					GiveBonus( chant, e.Mobile );
					e.Handled = true;
				}
			}
		}

		public void GiveBonus( int chant, Mobile from )
		{
			if ( VirtueEffect == chant )
				from.SendLocalizedMessage( 1079544, String.Format( "{0}" ,m_ShrineWords[chant]) ); // You already feel ~1_VIRTUE~ from your earlier contemplation of the virtues.

			TimeSpan delay = m_LastUse - DateTime.UtcNow;

			if ( delay < TimeSpan.Zero )
				delay = TimeSpan.Zero;

			if ( delay > TimeSpan.Zero )
			{
				int seconds = (int)delay.TotalSeconds;
				int minutes = 0;
				int hours = 0;

				if ( seconds >= 60 )
					minutes = (seconds + 59) / 60;

				if ( minutes >= 60 )
					hours = (seconds + 3599) / 3600;

				if ( hours > 1 )
					from.SendLocalizedMessage( 1079566, String.Format( "{0}", hours ) ); // You feel as if you should contemplate what you've learned for another ~1_TIME~ hours.
				else if ( hours == 1 )
					from.SendLocalizedMessage( 1079565 ); // You feel as if you should contemplate what you've learned for another hour or so.
				else if ( minutes > 0 )
					from.SendLocalizedMessage( 1079568, String.Format( "{0}", minutes ) ); // You feel as if you should contemplate what you've learned for another ~1_TIME~ minutes.
				else if ( seconds > 0 )
					from.SendLocalizedMessage( 1079567, String.Format( "{0}", seconds ) ); // You feel almost ready to learn more about the virtue again.

				return;
			}

			double chance = Utility.RandomDouble();

			switch ( chant )
			{
				case 0: //Compassion
				{
					Attributes.RegenHits = 2;
					break;
				}
				case 1: //Honesty
				{
					Attributes.RegenMana = 2;
					break;
				}
				case 2: //Honor
				{
					Attributes.RegenStam = (chance >= 0.75) ? 2 : 1;
					Attributes.RegenMana = (chance <= 0.25) ? 2 : 1;
					break;
				}
				case 3: //Humility
				{
					if ( chance >= 0.66 )
						Attributes.RegenHits = 3;
					else if ( chance >= 0.33 )
						Attributes.RegenStam = 3;
					else
						Attributes.RegenMana = 3;

					break;
				}
				case 4: //Justice
				{
					Attributes.RegenHits = (chance >= 0.75) ? 2 : 1;
					Attributes.RegenMana = (chance <= 0.25) ? 2 : 1;
					break;
				}
				case 5: //Sacrifice
				{
					Attributes.RegenHits = (chance >= 0.75) ? 2 : 1;
					Attributes.RegenStam = (chance <= 0.25) ? 2 : 1;
					break;
				}
				case 6: //Spirituality
				{
					Attributes.RegenHits = (chance >= 0.25) ? 2 : 1;
					chance = Utility.RandomDouble();
					Attributes.RegenStam = (chance >= 0.25) ? 2 : 1;
					chance = Utility.RandomDouble();
					Attributes.RegenMana = (chance >= 0.25) ? 2 : 1;
					break;
				}
				case 7: //Valor
				{
					Attributes.RegenStam = 2;
					break;
				}
			}

			if ( IsUseful )
			{
				Attributes.RegenHits *= 3;
				Attributes.RegenStam *= 3;
				Attributes.RegenMana *= 3;
			}

			m_LastUse = DateTime.UtcNow + TimeSpan.FromMinutes( 61 );
			VirtueEffect = chant;

			Timer timer = new InternalTimer( this );
			timer.Start();
			m_Timer = timer;

			from.SendLocalizedMessage( 1079546, String.Format( "{0}", m_ShrineWords[chant] ) ); // Contemplating at the shrine has left you feeling more ~1_VIRTUE~.
		}

		private static readonly string[] m_ShrineMantra = new string[]
		{
			"mu mu mu", //Compassion
			"ahm ahm ahm", //Honesty
			"summ summ summ", //Honor
			"lum lum lum", //Humility
			"beh beh beh", //Justice
			"cah cah cah", //Sacrifice
			"om om om", //Spirituality
			"ra ra ra" //Valor
		};

		private static readonly Point3D[] m_BritanniaShrines = new Point3D[]
		{
			new Point3D( 1857, 865, -1 ), //Compassion
			new Point3D( 4264, 3707, 0 ), //Honesty
			new Point3D( 1732, 3528, 0 ), //Honor
			new Point3D( 4220, 563, 36 ), //Humilty
			new Point3D( 1300, 644, 8 ), //Justice
			new Point3D( 3355, 302, 9 ), //Sacrifice
			new Point3D( 1606, 2490, 5 ), //Spirituality
			new Point3D( 2500, 3931, 3 ) //Valor
		};

		private static readonly Point3D[] m_IllshShrines = new Point3D[]
		{
			new Point3D( 1222, 474, -17 ), //Compassion
			new Point3D( 718, 1360, -60), //Honesty
			new Point3D( 744, 724, -28 ), //Honor
			new Point3D( 297, 1014, -19 ), //Humilty
			new Point3D( 986, 1006, -36 ), //Justice
			new Point3D( 1180, 1288, -30 ), //Sacrifice
			new Point3D( 1538, 1341, -3 ), //Spirituality
			new Point3D( 528, 223, -38 ) //Valor
		};

		public static readonly string[] m_ShrineWords = new string[]
		{
			"compassionate",
			"just",
			"charitable",
			"honest",
			"honorable",
			"humble",
			"spiritual",
			"valorous",
			"virtuous"
		};

		public AnkhPendant( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (bool)m_IsUseful );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			Attributes.RegenHits = 0;
			Attributes.RegenStam = 0;
			Attributes.RegenMana = 0;
			VirtueEffect = 0;
			m_IsUseful = reader.ReadBool();
		}

		private class InternalTimer : Timer
		{
			private AnkhPendant m_From;
			private int m_Count;

			public InternalTimer( AnkhPendant from ) : base( TimeSpan.FromMinutes( 5 ) )
			{
				m_From = from;
			}

			protected override void OnTick()
			{
				m_Count++;

				if ( m_Count >= 12 )
				{
					if ( m_From != null )
					{
						m_From.Attributes.RegenHits = 0;
						m_From.Attributes.RegenStam = 0;
						m_From.Attributes.RegenMana = 0;
						m_From.VirtueEffect = 0;

						if ( m_From.Parent is Mobile )
						{
							Mobile m = m_From.Parent as Mobile;
							m.SendLocalizedMessage( 1079553 ); // The effects of meditating at the shrine have worn off.
						}
					}

					Stop();
				}
			}
		}

	}
}

/*
Five on Friday: http://www.uo.com/fof/fiveonfriday79.html

"What does the ankh necklace do? Is it just for show?"
As a number of alert players have pointed our, should you visit one of 
the Shrines of the Virtues and meditate (chant a mantra) there, 
you will receive a system message having to do with that virtue. 
But what does it mean? 

The ankh will give a bonus to one or more of the 3 regeneration stats 
that will last for 1 hour. The bonus can only be activated once per 
day. Each of the shrines will give a unique bonus combination to 
regeneration stats based on the virtue for that particular shrine 
(this is based on the 3 principles). 

Each principle is related to a regeneration stat as follows:

Truth - Mana Regeneration
Love - Hit Point Regeneration
Courage - Stamina Regeneration
Each virtue will give the following bonus to regeneration stats as follows: 

Honesty +2 Mana Regen
Compassion +2 HP Regen
Valor +2 Stamina Regen
Justice +1 Mana Regen, +1 HP Regen (50% chance to get one regen bumped up to +2) 
Sacrifice +1 HP Regen, +1 Stamina Regen (50% chance to get one regen bumped up to +2) 
Honor +1 Mana Regen, +1 Stamina Regen (50% chance to get one regen bumped up to +2) 
Spirituality +1 All Regens (25% chance (3 independent rolls) to get each regen bumped up to +2) 
Humility +3 Random Regen

***

IsUseful?

Private joke to my self, your welcome to learn what it means.


And I quote
"The event uses the Guaranteed Reward Point system, 
which was created for the Treasures of Tokuno event"
...
"The overall loot drop chance for this event will be lower than ToT, ..."


Lets look at two of the nonweapon LESSER ToT items and the Ankh Pendant

AncientFarmersKasa
	Resistances: 0/5/9/5/5
	BonusStr +5
	BonusStam +5
	RegenStam +5
	AnimalLore +5.0

GlovesOfTheSun
	Resistances: 2/4/3/3/3 //Can it be enhanced?
	RegenHits +2
	NightSight
	LowerManaCost +5%
	LowerRegCost +18%

Ankh Pendant
	Resistances: 0/0/0/0/0
	RegenHits/Stam/Mana
		Random one has +3 for one hour. OR
		One of your choice has +2 for one hour. OR
		Any two of your choice has +1 for one hour, one of which may be could possible be increaed to +2. OR
		+1 in all three regen stats, with the remote possiblity one or two of them become +2 bonuses.
	Downsides: Works for one hour per day (not 24/7 like magic loot), must recall to a shrine to activate.


You give up your entire neck slot for that?
You have got to be kidding me.

*/