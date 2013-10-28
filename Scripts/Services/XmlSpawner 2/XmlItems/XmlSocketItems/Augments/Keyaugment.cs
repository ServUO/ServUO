using System;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    // ---------------------------------------------------
    // Key augment to open doors and locked containers
    // ---------------------------------------------------
    public class KeyAugment : BaseSocketAugmentation
    {
        private SkillName m_RequiredSkill = SkillName.Lockpicking;// default skill required to use this augment
        private int m_RequiredSkillLevel = 100;// default skill level required to use this augment
        private int m_OpenSound = 0x1F4;// default opening sound
        private int m_KeyValue;// matched against the keyvalue of the door or container it is intended to open
        private int m_UsesRemaining = 1;// can only use once by default
        [Constructable]
        public KeyAugment(string name, int keyvalue, string skillname, int minlevel)
            : base(0x1f14)
        {
            this.Hue = 20;
            this.Name = name;
            this.KeyValue = keyvalue;
            try
            {
                this.RequiredSkill = (SkillName)Enum.Parse(typeof(SkillName), skillname);
            }
            catch
            {
            }
            this.RequiredSkillLevel = minlevel;
        }

        [Constructable]
        public KeyAugment(string name, int keyvalue)
            : base(0x1f14)
        {
            this.Hue = 20;
            this.Name = name;
            this.KeyValue = keyvalue;
        }

        [Constructable]
        public KeyAugment(string name)
            : base(0x1f14)
        {
            this.Hue = 20;
            this.Name = name;
        }

        [Constructable]
        public KeyAugment()
            : base(0x1f14)
        {
            this.Hue = 20;
        }

        public KeyAugment(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName RequiredSkill
        {
            get
            {
                return this.m_RequiredSkill;
            }
            set
            {
                this.m_RequiredSkill = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredSkillLevel
        {
            get
            {
                return this.m_RequiredSkillLevel;
            }
            set
            {
                this.m_RequiredSkillLevel = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OpenSound
        {
            get
            {
                return this.m_OpenSound;
            }
            set
            {
                this.m_OpenSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int KeyValue
        {
            get
            {
                return this.m_KeyValue;
            }
            set
            {
                this.m_KeyValue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }
        public override bool DestroyAfterUse
        {
            get
            {
                return (this.UsesRemaining == 0);
            }
        }
        public override int IconXOffset
        {
            get
            {
                return 5;
            }
        }
        public override int IconYOffset
        {
            get
            {
                return 20;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_UsesRemaining >= 0)
                list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override string OnIdentify(Mobile from)
        {
            return String.Format("Key to {0}", this.Name);
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if (from == null || target == null || this.UsesRemaining == 0)
                return false;

            if (target is BaseDoor)
            {
                BaseDoor d = target as BaseDoor;
                
                // open the door
                d.Open = true;
                
                if (this.OpenSound > 0)
                    from.PlaySound(this.OpenSound);
                
                // and add a fresh socket attachment to the door
                XmlAttach.AttachTo(d, new XmlSockets(1,false));
                
                // decrement the uses
                if (this.UsesRemaining > 0)
                    this.UsesRemaining--;

                return true;
            }
            else if (target is LockableContainer)
            {
                LockableContainer d = target as LockableContainer;
                
                // open the container
                d.Locked = false;
                
                if (this.OpenSound > 0)
                    from.PlaySound(this.OpenSound);
                
                // decrement the uses
                if (this.UsesRemaining > 0)
                    this.UsesRemaining--;

                return true;
            }

            return false;
        }

        public override bool CanAugment(Mobile from, object target)
        {
            if (from == null)
                return false;

            if (target is BaseDoor || target is LockableContainer)
            {
                Item d = target as Item;

                if (this.RequiredSkillLevel > 0 && from.Skills[this.RequiredSkill].Value < this.RequiredSkillLevel)
                {
                    from.SendMessage("You lack the {0} skill to use this", this.RequiredSkill.ToString());
                    return false;
                }
                
                if ((d is BaseDoor && ((BaseDoor)d).KeyValue == this.KeyValue) || (d is LockableContainer && ((LockableContainer)d).KeyValue == this.KeyValue))
                    return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
			
            writer.Write((int)this.m_RequiredSkill);
            writer.Write((int)this.m_RequiredSkillLevel);
            writer.Write((int)this.m_OpenSound);
            writer.Write((int)this.m_KeyValue);
            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_RequiredSkill = (SkillName)reader.ReadInt();
            this.m_RequiredSkillLevel = reader.ReadInt();
            this.m_OpenSound = reader.ReadInt();
            this.m_KeyValue = reader.ReadInt();
            this.m_UsesRemaining = reader.ReadInt();
        }
    }
}