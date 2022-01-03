using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlMagicWord : XmlAttachment
	{
		private string Word;
		private TimeSpan Duration = TimeSpan.FromSeconds(30.0);     // 30 sec default duration for effects
		private int Charges = 1;                        // single use by default, note a value of zero or less means unlimited use
		private TimeSpan Refractory = TimeSpan.Zero;    // no refractory period
		private DateTime m_EndTime = DateTime.MinValue;

		// static list used for random word assignment
		private static string [] keywordlist = new string[] { "Shoda", "Malik", "Lepto" , "Velas", "Tarda", "Marda", "Vas Malik", "Nartor", "Santor"};

		// note that support for player identification requires modification of the identification skill (see the installation notes for details)
		private bool m_Identified = false;  // optional identification flag that can suppress application of the mod until identified when applied to items

		private bool m_RequireIdentification = false;  // by default no identification is required for the mod to be activatable

		// this property can be set allowing individual items to determine whether they must be identified for the mod to be activatable
		public bool RequireIdentification { get { return m_RequireIdentification; } set {m_RequireIdentification = value; } }

		// a serial constructor is REQUIRED
		public XmlMagicWord(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlMagicWord()
		{
			Word = keywordlist[Utility.Random(keywordlist.Length)];
			Name = Word;
		}

		[Attachable]
		public XmlMagicWord(string word)
		{
			Word = word;
			Name = word;
		}

		[Attachable]
		public XmlMagicWord(string word, double duration)
		{
			Name = word;
			Word = word;
			Duration = TimeSpan.FromSeconds(duration);
		}

		[Attachable]
		public XmlMagicWord(string word, double duration, double refractory)
		{
			Name = word;
			Word = word;
			Duration = TimeSpan.FromSeconds(duration);
			Refractory = TimeSpan.FromSeconds(refractory);
		}

		[Attachable]
		public XmlMagicWord(string word, double duration, double refractory, int charges)
		{
			Name = word;
			Word = word;
			Duration = TimeSpan.FromSeconds(duration);
			Refractory = TimeSpan.FromSeconds(refractory);
			Charges = charges;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write(Word);
			writer.Write(Charges);
			writer.Write(Duration);
			writer.Write(Refractory);
			writer.Write(m_EndTime - DateTime.Now);
			writer.Write(m_RequireIdentification);
			writer.Write(m_Identified);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			Word = reader.ReadString();
			Charges = reader.ReadInt();
			Duration = reader.ReadTimeSpan();
			Refractory = reader.ReadTimeSpan();
			TimeSpan remaining = reader.ReadTimeSpan();
			m_EndTime = DateTime.Now + remaining;
			m_RequireIdentification = reader.ReadBool();
			m_Identified = reader.ReadBool();
		}
		
		public override string OnIdentify(Mobile from)
		{     
			string msg = null;

			// can force identification before the skill mods can be applied
			if(from != null && from.AccessLevel == AccessLevel.Player)
			{
				m_Identified = true;
			}

			if(RequireIdentification && !m_Identified) return null;

			if(Refractory > TimeSpan.Zero)
			{
				msg = String.Format("{0} lasting {1} secs : {2} secs between uses",Word,Duration.TotalSeconds, Refractory.TotalSeconds);
			} 
			else
			{
				msg = String.Format("{0} lasting {1} secs",Word,Duration.TotalSeconds);
			}
		    
			if(Charges > 0)
			{
				return String.Format("{0} : {1} charge(s) remaining",msg, Charges);
			} 
			else
			{
				return msg;
			}
		}

		// by overriding these properties armor and weapons can be restricted to trigger on speech only when equipped and not when in the pack or in the world
		public override bool CanActivateInBackpack 
		{ 
			get
			{
				if(AttachedTo is BaseWeapon || AttachedTo is BaseArmor)
					return false;
				else
					return true;
			}
		}

		public override bool CanActivateInWorld 
		{
			get
			{
				if(AttachedTo is BaseWeapon || AttachedTo is BaseArmor)
					return false;
				else
					return true;
			}
		}

		public override bool HandlesOnSpeech { get { return true; } }
		
		public override void OnSpeech(SpeechEventArgs e )
		{
			base.OnSpeech(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;

			// dont respond to other players speech if this is attached to a mob
			if(AttachedTo is Mobile && (Mobile)AttachedTo != e.Mobile) return;

			if(e.Speech == Word)
			{
				OnTrigger(null, e.Mobile);
			}
		}

		public void Hide_Callback(object state)
		{
			object[] args = (object[])state;
			Mobile m = (Mobile)args[0];

			m.Hidden = true;
		}

		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null || Word == null || (RequireIdentification && !m_Identified)) return;

			if(DateTime.Now < m_EndTime) return;

			string msgstr = "Activating the power of " + Word;

			// assign powers to certain words
			switch ( Word )
			{
				case "Shoda":
					m.AddStatMod( new StatMod( StatType.Int, "Shoda", 20, Duration ) );
					m.SendMessage("Your mind expands!");
					break;
				case "Malik":
					m.AddStatMod( new StatMod( StatType.Str, "Malik", 20, Duration ) );
					m.SendMessage("Your strength surges!");
					break;
				case "Lepto":
					m.AddStatMod( new StatMod( StatType.Dex, "Lepto", 20, Duration ) );
					m.SendMessage("You are more nimble!");
					break;
				case "Velas":
					Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Hide_Callback ), new object[]{ m } );
					m.SendMessage("You disappear!");
					break;
				case "Tarda":
					m.AddSkillMod( new TimedSkillMod( SkillName.Tactics, true, 20, Duration ) );
					m.SendMessage("You are more skillful warrior!");
					break;
				case "Marda":
					m.AddSkillMod( new TimedSkillMod( SkillName.Magery, true, 20, Duration ) );
					m.SendMessage("You are more skillful mage!");
					break;
				case "Vas Malik":
					m.AddStatMod( new StatMod( StatType.Str, "Vas Malik", 40, Duration ) );
					m.SendMessage("You are exceptionally strong!");
					break;
				case "Nartor":
					BaseCreature b = new Drake();
					b.MoveToWorld(m.Location, m.Map);
					b.Owners.Add( m );
					b.SetControlMaster( m );
					if(b.Controlled)
						m.SendMessage("You master the beast!");
					break;
				case "Santor":
					b = new Horse();
					b.MoveToWorld(m.Location, m.Map);
					b.Owners.Add( m );
					b.SetControlMaster( m );
					if(b.Controlled)
						m.SendMessage("You master the beast!");
					break;
				default:
					m.SendMessage("There is no effect.");
					break;
			}
            
			// display activation effects
			Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
			Effects.PlaySound( m, m.Map, 0x201 );

			// display a message over the item it was attached to
			if(AttachedTo is Item )
			{
				((Item)AttachedTo).PublicOverheadMessage( MessageType.Regular, 0x3B2, true, msgstr );
			}
            
			Charges--;

			// remove the attachment after the charges run out
			if(Charges == 0)
			{
				Delete();
			} 
			else
			{
				m_EndTime = DateTime.Now + Refractory;
			}
		}
	}
}
