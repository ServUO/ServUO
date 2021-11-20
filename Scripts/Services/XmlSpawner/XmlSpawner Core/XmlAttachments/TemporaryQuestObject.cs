using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	// When this attachment is deleted, the object that it is attached to will be deleted as well.
	// The quest system will automatically delete these attachments after a quest is completed.
	// Specifying an expiration time will also allow you to give objects limited lifetimes.
    public class TemporaryQuestObject : XmlAttachment, ITemporaryQuestAttachment
	{

		private Mobile m_QuestOwner;

        [CommandProperty( AccessLevel.GameMaster )]
		public Mobile QuestOwner
		{
			get {return m_QuestOwner;}
			set {m_QuestOwner = value;}
		}

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
       
        // a serial constructor is REQUIRED
        public TemporaryQuestObject(ASerial serial) : base(serial)
        {
        }

        [Attachable]
        public TemporaryQuestObject(string questname)
        {
            Name = questname;
        }
        
        [Attachable]
        public TemporaryQuestObject(string questname, double expiresin)
        {
            Name = questname;
            Expiration = TimeSpan.FromMinutes(expiresin);

        }

		[Attachable]
		public TemporaryQuestObject(string questname, double expiresin, Mobile questowner)
		{
			Name = questname;
			Expiration = TimeSpan.FromMinutes(expiresin);
			QuestOwner = questowner;

		}

		public override void OnDelete()
		{
			base.OnDelete();

			// delete the object that it is attached to
			if(AttachedTo is Mobile)
			{
				// dont allow deletion of players
				if(!((Mobile)AttachedTo).Player)
				{
                    SafeMobileDelete((Mobile)AttachedTo);
					//((Mobile)AttachedTo).Delete();
				}
			} 
			else
				if(AttachedTo is Item)
			{
                SafeItemDelete((Item)AttachedTo);
				//((Item)AttachedTo).Delete();
			}
		}



        public override void Serialize( GenericWriter writer )
		{
            base.Serialize(writer);

            writer.Write( (int) 0 );

			// version 0
			writer.Write(m_QuestOwner);

        }

        public override void Deserialize(GenericReader reader)
		{
		    base.Deserialize(reader);

            int version = reader.ReadInt();

			// version 0
			m_QuestOwner = reader.ReadMobile();

		}

		public override string OnIdentify(Mobile from)
		{
		    if(from == null || from.AccessLevel == AccessLevel.Player) return null;

            if(Expiration > TimeSpan.Zero)
            {
                return String.Format("{1} expires in {0} mins",Expiration.TotalMinutes, Name);
            } else
            {
                return String.Format("{1}: QuestOwner {0}",QuestOwner, Name);
            }
		}
    }
}
