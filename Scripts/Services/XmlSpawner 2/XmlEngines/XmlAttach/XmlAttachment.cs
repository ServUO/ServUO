using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public interface IXmlAttachment
    {
        ASerial Serial { get; }
        string Name { get; set; }
        TimeSpan Expiration { get; set; }
        DateTime ExpirationEnd { get; }
        DateTime CreationTime { get; }
        bool Deleted { get; }
        bool DoDelete { get; set; }
        bool CanActivateInBackpack { get; }
        bool CanActivateEquipped { get; }
        bool CanActivateInWorld { get; }
        bool HandlesOnSpeech { get; }
        bool HandlesOnMovement { get; }
        bool HandlesOnKill { get; }
        bool HandlesOnKilled { get; }
        /*
        bool HandlesOnSkillUse { get; }

        void OnSkillUse( Mobile m, Skill skill, bool success);
        */
        object AttachedTo { get; set; }
        object OwnedBy { get; set; }
        string AttachedBy { get; }
        void OnSpeech(SpeechEventArgs args);

        void OnMovement(MovementEventArgs args);

        void OnKill(Mobile killed, Mobile killer);

        void OnBeforeKill(Mobile killed, Mobile killer);

        void OnKilled(Mobile killed, Mobile killer);

        void OnBeforeKilled(Mobile killed, Mobile killer);

        bool CanEquip(Mobile from);

        void OnEquip(Mobile from);

        void OnRemoved(object parent);

        void OnAttach();

        void OnReattach();

        void OnUse(Mobile from);

        void OnUser(object target);

        bool BlockDefaultOnUse(Mobile from, object target);

        bool OnDragLift(Mobile from, Item item);

        string OnIdentify(Mobile from);

        string DisplayedProperties(Mobile from);

        void AddProperties(ObjectPropertyList list);

        void GetPlayerMobileProperties(ObjectPropertyList list);

        void OnDelete();

        void Delete();

        void InvalidateParentProperties();

        void SetAttachedBy(string name);

        void OnTrigger(object activator, Mobile from);

        void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven);

        int OnArmorHit(Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven);

        void Serialize(GenericWriter writer);

        void Deserialize(GenericReader reader);
    }

    public abstract class XmlAttachment : IXmlAttachment
    {
        // ----------------------------------------------
        // Private fields
        // ----------------------------------------------
        private ASerial m_Serial;
        private string m_Name;
        private object m_AttachedTo;
        private object m_OwnedBy;
        private string m_AttachedBy;
        private bool m_Deleted;
        private AttachmentTimer m_ExpirationTimer;
        private TimeSpan m_Expiration = TimeSpan.Zero;// no expiration by default
        private DateTime m_ExpirationEnd;
        private DateTime m_CreationTime;// when the attachment was made

        // ----------------------------------------------
        // Constructors
        // ----------------------------------------------
        public XmlAttachment()
        {
            this.m_CreationTime = DateTime.UtcNow;

            // get the next unique serial id
            this.m_Serial = ASerial.NewSerial();

            // register the attachment in the serial keyed hashtable
            XmlAttach.HashSerial(this.m_Serial, this);
        }

        // needed for deserialization
        public XmlAttachment(ASerial serial)
        {
            this.m_Serial = serial;
        }

        // ----------------------------------------------
        // Public properties
        // ----------------------------------------------
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CreationTime
        {
            get
            {
                return this.m_CreationTime;
            }
        }
        public bool Deleted
        {
            get
            {
                return this.m_Deleted;
            }
        }
        public bool DoDelete
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                    this.Delete();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SerialValue
        {
            get
            {
                return this.m_Serial.Value;
            }
        }
        public ASerial Serial
        {
            get
            {
                return this.m_Serial;
            }
            set
            {
                this.m_Serial = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Expiration
        {
            get
            {
                // if the expiration timer is running then return the remaining time
                if (this.m_ExpirationTimer != null)
                {
                    return this.m_ExpirationEnd - DateTime.UtcNow;
                }
                else
                    return this.m_Expiration;
            }
            set
            {
                this.m_Expiration = value;
                // if it is already attached to something then set the expiration timer
                if (this.m_AttachedTo != null)
                {
                    this.DoTimer(this.m_Expiration);
                }
            }
        }
        public DateTime ExpirationEnd
        {
            get
            {
                return this.m_ExpirationEnd;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool CanActivateInBackpack
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool CanActivateEquipped
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool CanActivateInWorld
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool HandlesOnSpeech
        {
            get
            {
                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool HandlesOnMovement
        {
            get
            {
                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool HandlesOnKill
        {
            get
            {
                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool HandlesOnKilled
        {
            get
            {
                return false;
            }
        }
        /*
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool HandlesOnSkillUse { get{return false; } }
        */
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual object Attached
        {
            get
            {
                return this.m_AttachedTo;
            }
        }
        public virtual object AttachedTo
        {
            get
            {
                return this.m_AttachedTo;
            }
            set
            {
                this.m_AttachedTo = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string AttachedBy
        {
            get
            {
                return this.m_AttachedBy;
            }
        }
        public virtual object OwnedBy
        {
            get
            {
                return this.m_OwnedBy;
            }
            set
            {
                this.m_OwnedBy = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual object Owner
        {
            get
            {
                return this.m_OwnedBy;
            }
        }
        // ----------------------------------------------
        // Public methods
        // ----------------------------------------------
        public static void Initialize()
        {
            XmlAttach.CleanUp();
        }

        public virtual bool CanEquip(Mobile from)
        {
            return true;
        }

        public virtual void OnEquip(Mobile from)
        {
        }

        public virtual void OnRemoved(object parent)
        {
        }

        public virtual void OnAttach()
        {
            // start up the expiration timer on attachment
            if (this.m_Expiration > TimeSpan.Zero)
                this.DoTimer(this.m_Expiration);
        }

        public virtual void OnReattach()
        {
        }

        public virtual void OnUse(Mobile from)
        {
        }

        public virtual void OnUser(object target)
        {
        }

        public virtual bool BlockDefaultOnUse(Mobile from, object target)
        {
            return false;
        }

        public virtual bool OnDragLift(Mobile from, Item item)
        {
            return true;
        }

        public void SetAttachedBy(string name)
        {
            this.m_AttachedBy = name;
        }

        public virtual void OnSpeech(SpeechEventArgs args)
        {
        }

        public virtual void OnMovement(MovementEventArgs args)
        {
        }

        public virtual void OnKill(Mobile killed, Mobile killer)
        {
        }

        public virtual void OnBeforeKill(Mobile killed, Mobile killer)
        {
        }

        public virtual void OnKilled(Mobile killed, Mobile killer)
        {
        }

        public virtual void OnBeforeKilled(Mobile killed, Mobile killer)
        {
        }

        /*
        public virtual void OnSkillUse( Mobile m, Skill skill, bool success)
        {
        }
        */
        public virtual void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
        {
        }

        public virtual int OnArmorHit(Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven)
        {
            return 0;
        }

        public virtual string OnIdentify(Mobile from)
        {
            return null;
        }

        public virtual string DisplayedProperties(Mobile from)
        {
            return this.OnIdentify(from);
        }

        public virtual void AddProperties(ObjectPropertyList list)
        {
        }

        public virtual void GetPlayerMobileProperties(ObjectPropertyList list)
        {
        }

        public void InvalidateParentProperties()
        {
            if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).InvalidateProperties();
            }
        }

        public void SafeItemDelete(Item item)
        {
            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(DeleteItemCallback), new object[] { item });
        }

        public void DeleteItemCallback(object state)
        {
            object[] args = (object[])state;

            Item item = args[0] as Item;

            if (item != null)
            {
                // delete the item
                item.Delete();
            }
        }

        public void SafeMobileDelete(Mobile mob)
        {
            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(DeleteMobileCallback), new object[] { mob });
        }

        public void DeleteMobileCallback(object state)
        {
            object[] args = (object[])state;

            Mobile mob = args[0] as Mobile;

            if (mob != null)
            {
                // delete the mobile
                mob.Delete();
            }
        }

        public void Delete()
        {
            if (this.m_Deleted)
                return;

            this.m_Deleted = true;

            if (this.m_ExpirationTimer != null)
                this.m_ExpirationTimer.Stop();

            this.OnDelete();

            // dereference the attachment object
            this.AttachedTo = null;
            this.OwnedBy = null;
        }

        public virtual void OnDelete()
        {
        }

        public virtual void OnTrigger(object activator, Mobile from)
        {
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)2);
            // version 2
            writer.Write(this.m_AttachedBy);
            // version 1
            if (this.OwnedBy is Item)
            {
                writer.Write((int)0);
                writer.Write((Item)this.OwnedBy);
            }
            else if (this.OwnedBy is Mobile)
            {
                writer.Write((int)1);
                writer.Write((Mobile)this.OwnedBy);
            }
            else
                writer.Write((int)-1);

            // version 0
            writer.Write(this.Name);
            // if there are any active timers, then serialize
            writer.Write(this.m_Expiration);
            if (this.m_ExpirationTimer != null)
            {
                writer.Write(this.m_ExpirationEnd - DateTime.UtcNow);
            }
            else
            {
                writer.Write(TimeSpan.Zero);
            }
            writer.Write(this.m_CreationTime);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    this.m_AttachedBy = reader.ReadString();
                    goto case 1;
                case 1:
                    int owned = reader.ReadInt();
                    if (owned == 0)
                    {
                        this.OwnedBy = reader.ReadItem();
                    }
                    else if (owned == 1)
                    {
                        this.OwnedBy = reader.ReadMobile();
                    }
                    else
                        this.OwnedBy = null;

                    goto case 0;
                case 0:
                    // version 0
                    this.Name = (string)reader.ReadString();
                    this.m_Expiration = reader.ReadTimeSpan();
                    TimeSpan remaining = (TimeSpan)reader.ReadTimeSpan();

                    if (remaining > TimeSpan.Zero)
                        this.DoTimer(remaining);

                    this.m_CreationTime = reader.ReadDateTime();
                    break;
            }
        }

        // ----------------------------------------------
        // Private methods
        // ----------------------------------------------
        private void DoTimer(TimeSpan delay)
        {
            this.m_ExpirationEnd = DateTime.UtcNow + delay;

            if (this.m_ExpirationTimer != null)
                this.m_ExpirationTimer.Stop();

            this.m_ExpirationTimer = new AttachmentTimer(this, delay);
            this.m_ExpirationTimer.Start();
        }

        // a timer that can be implement limited lifetime attachments
        private class AttachmentTimer : Timer
        {
            private readonly XmlAttachment m_Attachment;
            public AttachmentTimer(XmlAttachment attachment, TimeSpan delay)
                : base(delay)
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_Attachment = attachment;
            }

            protected override void OnTick()
            {
                this.m_Attachment.Delete();
            }
        }
    }
}