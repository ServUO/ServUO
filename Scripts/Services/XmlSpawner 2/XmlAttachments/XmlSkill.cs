using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlSkill : XmlAttachment
    {
        private string m_Word = null;// not speech activated by default
        private TimeSpan m_Duration = TimeSpan.FromMinutes(30.0);// 30 min default duration for effects
        private int m_Value = 10;// default value of 10
        private SkillName m_Skill;
        // note that support for player identification requires modification of the identification skill (see the installation notes for details)
        private bool m_Identified = false;// optional identification flag that can suppress application of the mod until identified when applied to items
        private bool m_RequireIdentification = false;// by default no identification is required for the mod to be activatable

        // a serial constructor is REQUIRED
        public XmlSkill(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlSkill(string name, string skill)
        {
            this.Name = name;
            try
            {
                this.m_Skill = (SkillName)Enum.Parse(typeof(SkillName), skill, true);
            }
            catch
            {
            }
        }

        [Attachable]
        public XmlSkill(string name, string skill, int value)
        {
            this.Name = name;
            try
            {
                this.m_Skill = (SkillName)Enum.Parse(typeof(SkillName), skill, true);
            }
            catch
            {
            }
            this.m_Value = value;
        }

        [Attachable]
        public XmlSkill(string name, string skill, int value, double duration)
        {
            this.Name = name;
            try
            {
                this.m_Skill = (SkillName)Enum.Parse(typeof(SkillName), skill, true);
            }
            catch
            {
            }
            this.m_Value = value;
            this.m_Duration = TimeSpan.FromMinutes(duration);
        }

        [Attachable]
        public XmlSkill(string name, string skill, int value, double duration, string word)
        {
            this.Name = name;
            try
            {
                this.m_Skill = (SkillName)Enum.Parse(typeof(SkillName), skill, true);
            }
            catch
            {
            }
            this.m_Value = value;
            this.m_Duration = TimeSpan.FromMinutes(duration);
            this.m_Word = word;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        // this property can be set allowing individual items to determine whether they must be identified for the mod to be activatable
        public bool RequireIdentification
        {
            get
            {
                return this.m_RequireIdentification;
            }
            set
            {
                this.m_RequireIdentification = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ActivationWord
        {
            get
            {
                return this.m_Word;
            }
            set
            {
                this.m_Word = value;
            }
        }
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword
        // Other overloads could be defined to handle other types of arguments
        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_Word);
            writer.Write((int)this.m_Skill);
            writer.Write(this.m_Value);
            writer.Write(this.m_Duration);
            writer.Write(this.m_RequireIdentification);
            writer.Write(this.m_Identified);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_Word = reader.ReadString();
            this.m_Skill = (SkillName)reader.ReadInt();
            this.m_Value = reader.ReadInt();
            this.m_Duration = reader.ReadTimeSpan();
            this.m_RequireIdentification = reader.ReadBool();
            this.m_Identified = reader.ReadBool();
        }

        public override string OnIdentify(Mobile from)
        {
            if (this.AttachedTo is BaseArmor || this.AttachedTo is BaseWeapon)
            {
                // can force identification before the skill mods can be applied
                if (from != null && from.IsPlayer())
                {
                    this.m_Identified = true;
                }
                return String.Format("activated by {0} : skill {1} mod of {2} when equipped", this.m_Word, this.m_Skill, this.m_Value);
            }
            else
            {
                return String.Format("activated by {0} : skill {1} mod of {2} lasting {3} mins", this.m_Word, this.m_Skill, this.m_Value, this.m_Duration.TotalMinutes);
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);
		    
            if (e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player)
                return;

            // dont respond to other players speech if this is attached to a mob
            if (this.AttachedTo is Mobile && (Mobile)this.AttachedTo != e.Mobile)
                return;

            if (e.Speech == this.m_Word)
            {
                this.OnTrigger(null, e.Mobile);
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod immediately
            if (this.AttachedTo is Mobile && this.m_Word == null)
            {
                this.OnTrigger(null, (Mobile)this.AttachedTo);
                // and then remove the attachment
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
                //Delete();
            }
            else if (this.AttachedTo is Item && this.m_Word == null)
            {
                // no way to activate if it is on an item and is not speech activated so just delete it
                this.Delete();
            }
        }

        public override void OnTrigger(object activator, Mobile m)
        {
            if (m == null || (this.RequireIdentification && !this.m_Identified))
                return;

            if ((this.AttachedTo is BaseArmor || this.AttachedTo is BaseWeapon) && (((Item)this.AttachedTo).Layer != Layer.Invalid))
            {
                // when activated via speech will apply mod when equipped by the speaker
                SkillMod sm = new EquipedSkillMod(this.m_Skill, true, this.m_Value, (Item)this.AttachedTo, m);
                m.AddSkillMod(sm);
                // and then remove the attachment
                this.Delete();
            }
            else
            {
                // when activated it will apply the skill mod that will last for the specified duration
                SkillMod sm = new TimedSkillMod(this.m_Skill, true, this.m_Value, this.m_Duration);
                m.AddSkillMod(sm);
                // and then remove the attachment
                this.Delete();
            }
        }
    }
}