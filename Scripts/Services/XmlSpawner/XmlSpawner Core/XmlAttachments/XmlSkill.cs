using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlSkill : XmlAttachment
	{
		private string m_Word = null;          // not speech activated by default
		private TimeSpan m_Duration = TimeSpan.FromMinutes(30.0);     // 30 min default duration for effects
		private int m_Value = 10;       // default value of 10
		private SkillName m_Skill;

		// note that support for player identification requires modification of the identification skill (see the installation notes for details)
		private bool m_Identified = false;  // optional identification flag that can suppress application of the mod until identified when applied to items

		private bool m_RequireIdentification = false;  // by default no identification is required for the mod to be activatable

		[CommandProperty( AccessLevel.GameMaster )]
			// this property can be set allowing individual items to determine whether they must be identified for the mod to be activatable
		public bool RequireIdentification { get { return m_RequireIdentification; } set {m_RequireIdentification = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Value { get { return m_Value; } set { m_Value  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill { get { return m_Skill; } set { m_Skill  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Duration { get { return m_Duration; } set { m_Duration  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ActivationWord { get { return m_Word; } set { m_Word  = value; } }

		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlSkill(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlSkill(string name, string skill)
		{
			Name = name;
			try
			{
				m_Skill = (SkillName)Enum.Parse( typeof( SkillName ), skill, true );
			} 
			catch {}
		}

		[Attachable]
		public XmlSkill(string name, string skill, int value)
		{
			Name = name;
			try
			{
				m_Skill = (SkillName)Enum.Parse( typeof( SkillName ), skill, true );
			} 
			catch {}
			m_Value = value;
		}

		[Attachable]
		public XmlSkill(string name, string skill, int value, double duration)
		{
			Name = name;
			try
			{
				m_Skill = (SkillName)Enum.Parse( typeof( SkillName ), skill, true );
			} 
			catch {}
			m_Value = value;
			m_Duration = TimeSpan.FromMinutes(duration);

		}

		[Attachable]
		public XmlSkill(string name, string skill, int value, double duration, string word)
		{
			Name = name;
			try
			{
				m_Skill = (SkillName)Enum.Parse( typeof( SkillName ), skill, true );
			} 
			catch {}
			m_Value = value;
			m_Duration = TimeSpan.FromMinutes(duration);
			m_Word = word;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write(m_Word);
			writer.Write((int)m_Skill);
			writer.Write(m_Value);
			writer.Write(m_Duration);
			writer.Write(m_RequireIdentification);
			writer.Write(m_Identified);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_Word = reader.ReadString();
			m_Skill = (SkillName) reader.ReadInt();
			m_Value = reader.ReadInt();
			m_Duration = reader.ReadTimeSpan();
			m_RequireIdentification = reader.ReadBool();
			m_Identified = reader.ReadBool();
		}

		public override string OnIdentify(Mobile from)
		{
			if(AttachedTo is BaseArmor || AttachedTo is BaseWeapon)
			{
				// can force identification before the skill mods can be applied
				if(from != null && from.AccessLevel == AccessLevel.Player)
				{
					m_Identified = true;
				}
				return String.Format("activated by {0} : skill {1} mod of {2} when equipped",m_Word, m_Skill, m_Value);
			} 
			else
			{
				return String.Format("activated by {0} : skill {1} mod of {2} lasting {3} mins",m_Word, m_Skill, m_Value, m_Duration.TotalMinutes);
			}
		}


		public override bool HandlesOnSpeech { get { return true; } }

		public override void OnSpeech(SpeechEventArgs e )
		{
			base.OnSpeech(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;

			// dont respond to other players speech if this is attached to a mob
			if(AttachedTo is Mobile && (Mobile)AttachedTo != e.Mobile) return;

			if(e.Speech == m_Word)
			{
				OnTrigger(null, e.Mobile);
			}
		}

		public override void OnAttach()
		{
			base.OnAttach();

			// apply the mod immediately
			if(AttachedTo is Mobile && m_Word == null)
			{
				OnTrigger(null, (Mobile)AttachedTo);
				// and then remove the attachment
				Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
				//Delete();
			} 
			else
				if(AttachedTo is Item && m_Word == null)
			{
				// no way to activate if it is on an item and is not speech activated so just delete it
				Delete();
			}
		}

		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null || (RequireIdentification && !m_Identified)) return;

			if((AttachedTo is BaseArmor || AttachedTo is BaseWeapon) && (((Item)AttachedTo).Layer != Layer.Invalid))
			{
				// when activated via speech will apply mod when equipped by the speaker
				SkillMod sm = new EquipedSkillMod( m_Skill, true, m_Value, (Item)AttachedTo, m );
				m.AddSkillMod( sm );
				// and then remove the attachment
				Delete();
			} 
			else
			{
				// when activated it will apply the skill mod that will last for the specified duration
				SkillMod sm = new TimedSkillMod( m_Skill, true, m_Value, m_Duration );
				m.AddSkillMod( sm );
				// and then remove the attachment
				Delete();
			}
		}
	}
}
