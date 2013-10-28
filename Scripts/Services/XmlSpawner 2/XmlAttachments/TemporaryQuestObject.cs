using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    // When this attachment is deleted, the object that it is attached to will be deleted as well.
    // The quest system will automatically delete these attachments after a quest is completed.
    // Specifying an expiration time will also allow you to give objects limited lifetimes.
    public class TemporaryQuestObject : XmlAttachment, ITemporaryQuestAttachment
    {
        private Mobile m_QuestOwner;
        // a serial constructor is REQUIRED
        public TemporaryQuestObject(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public TemporaryQuestObject(string questname)
        {
            this.Name = questname;
        }

        [Attachable]
        public TemporaryQuestObject(string questname, double expiresin)
        {
            this.Name = questname;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        [Attachable]
        public TemporaryQuestObject(string questname, double expiresin, Mobile questowner)
        {
            this.Name = questname;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
            this.QuestOwner = questowner;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile QuestOwner
        {
            get
            {
                return this.m_QuestOwner;
            }
            set
            {
                this.m_QuestOwner = value;
            }
        }
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void OnDelete()
        {
            base.OnDelete();

            // delete the object that it is attached to
            if (this.AttachedTo is Mobile)
            {
                // dont allow deletion of players
                if (!((Mobile)this.AttachedTo).Player)
                {
                    this.SafeMobileDelete((Mobile)this.AttachedTo);
                    //((Mobile)AttachedTo).Delete();
                }
            }
            else if (this.AttachedTo is Item)
            {
                this.SafeItemDelete((Item)this.AttachedTo);
                //((Item)AttachedTo).Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            // version 0
            writer.Write(this.m_QuestOwner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            // version 0
            this.m_QuestOwner = reader.ReadMobile();
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{1} expires in {0} mins", this.Expiration.TotalMinutes, this.Name);
            }
            else
            {
                return String.Format("{1}: QuestOwner {0}", this.QuestOwner, this.Name);
            }
        }
    }
}