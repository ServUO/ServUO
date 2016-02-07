using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
	public class XmlAddKarma : XmlAttachment
	{
		private int m_DataValue;    // default data

		[CommandProperty( AccessLevel.GameMaster )]
		public int Value { get{ return m_DataValue; } set { m_DataValue = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlAddKarma(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlAddKarma( int value)
		{
			Value = value;
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write(m_DataValue);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_DataValue = reader.ReadInt();
		}
		
		public override void OnAttach()
		{
			base.OnAttach();
		    
			// apply the mod
			if(AttachedTo is PlayerMobile)
			{
				// for players just add it immediately
				((Mobile)AttachedTo).Karma += Value;

				((Mobile)AttachedTo).SendMessage("Receive {0}",OnIdentify((Mobile)AttachedTo));

				// and then remove the attachment
				Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
				//Delete();
			} 
			else
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
		    
			killer.Karma += Value;

			killer.SendMessage("Receive {0}",OnIdentify(killer));
		}


		public override string OnIdentify(Mobile from)
		{

			return String.Format("{0} Karma", Value);

		}
	}
}
