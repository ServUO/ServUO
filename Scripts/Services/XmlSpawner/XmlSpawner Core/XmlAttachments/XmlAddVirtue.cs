using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
    public class XmlAddVirtue : XmlAttachment
    {
        private int m_DataValue;    // default data
        private string m_Virtue;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Value { get{ return m_DataValue; } set { m_DataValue = value; } }
        public string Virtue { get{ return m_Virtue; } set { m_Virtue = value; } }

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
       
        // a serial constructor is REQUIRED
        public XmlAddVirtue(ASerial serial) : base(serial)
        {
        }

        [Attachable]
        public XmlAddVirtue(string virtue, int value)
        {
            Value = value;
            Virtue = virtue;
        }


        public override void Serialize( GenericWriter writer )
		{
            base.Serialize(writer);

            writer.Write( (int) 0 );
            // version 0
            writer.Write(m_DataValue);
            writer.Write(m_Virtue);

        }

        public override void Deserialize(GenericReader reader)
		{
		    base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            m_DataValue = reader.ReadInt();
            m_Virtue = reader.ReadString();
		}
		
		public override void OnAttach()
		{
		    base.OnAttach();
		    
		    // apply the mod
            if(AttachedTo is PlayerMobile)
            {
                // for players just add it immediately
                // lookup the virtue type
                VirtueName g = (VirtueName)0;
				bool valid = true;
				bool gainedPath = false;
                try{
                    g = (VirtueName)Enum.Parse(typeof(VirtueName),Virtue, true);
                } catch{valid = false;}
                
                if(valid)
                {

                    VirtueHelper.Award( (Mobile)AttachedTo, g, Value, ref gainedPath );

                    ((Mobile)AttachedTo).SendMessage("Receive {0}",OnIdentify((Mobile)AttachedTo));

					if(gainedPath)
					{
						((Mobile)AttachedTo).SendMessage("You have gained a path in {0}",Virtue);
					}
                } else 
                {
                    ((Mobile)AttachedTo).SendMessage("{0}: no such Virtue", Virtue);
                }
                // and then remove the attachment
				Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
                //Delete();
            } else
            if(AttachedTo is Item)
            {
                // dont allow item attachments
                Delete();
            }

		}
		
		public override bool HandlesOnKilled { get { return true; } }
		
		public override void OnKilled(Mobile killed, Mobile killer )
		{
		    base.OnKilled(killed, killer);

		    if(killer == null) return;
		    
			VirtueName g = (VirtueName)0;
			bool valid = true;
			bool gainedPath = false;
            try{
                g = (VirtueName)Enum.Parse(typeof(VirtueName),Virtue, true);
            } catch{valid = false;}
            
            if(valid)
            {
    		    // give the killer the Virtue

				VirtueHelper.Award( killer, g, Value, ref gainedPath );

				if(gainedPath)
				{
					killer.SendMessage("You have gained a path in {0}",Virtue);
				}

                killer.SendMessage("Receive {0}",OnIdentify(killer));
            }
		}


		public override string OnIdentify(Mobile from)
		{

            return String.Format("{0} {1} Virtue points", Value, Virtue);

		}
    }
}
