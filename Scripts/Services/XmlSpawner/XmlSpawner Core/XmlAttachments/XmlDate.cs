using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlDate : XmlAttachment
	{
		private DateTime m_DataValue;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime Date { get{ return m_DataValue; } set { m_DataValue = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlDate(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlDate(string name)
		{
			Name = name;
			Date = DateTime.UtcNow;
		}
        
		[Attachable]
		public XmlDate(string name, double expiresin)
		{
			Name = name;
			Date = DateTime.UtcNow;
			Expiration = TimeSpan.FromMinutes(expiresin);

		}

		[Attachable]
		public XmlDate(string name, DateTime value, double expiresin)
		{
			Name = name;
			Date = value;
			Expiration = TimeSpan.FromMinutes(expiresin);

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
			m_DataValue = reader.ReadDateTime();
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			if(Expiration > TimeSpan.Zero)
			{
				return String.Format("{2}: Date {0} expires in {1} mins",Date,Expiration.TotalMinutes, Name);
			} 
			else
			{
				return String.Format("{1}: Date {0}",Date, Name);
			}
		}
	}
}
