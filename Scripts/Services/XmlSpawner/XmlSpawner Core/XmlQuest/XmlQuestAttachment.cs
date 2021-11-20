using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.IO;

namespace Server.Engines.XmlSpawner2
{
	public class XmlQuestAttachment : XmlAttachment
	{
		private DateTime m_DataValue;

		public DateTime Date { get { return m_DataValue; } set { m_DataValue = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlQuestAttachment(ASerial serial)
			: base(serial)
		{
		}

		[Attachable]
		public XmlQuestAttachment(string name)
		{
			Name = name;
			Date = DateTime.UtcNow;
		}

		[Attachable]
		public XmlQuestAttachment(string name, double expiresin)
		{
			Name = name;
			Date = DateTime.UtcNow;
			Expiration = TimeSpan.FromMinutes(expiresin);

		}

		[Attachable]
		public XmlQuestAttachment(string name, DateTime value, double expiresin)
		{
			Name = name;
			Date = value;
			Expiration = TimeSpan.FromMinutes(expiresin);

		}


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
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
			if (from.AccessLevel == AccessLevel.Player) return null;

			if (Expiration > TimeSpan.Zero)
			{
				return String.Format("Quest '{2}' Completed {0} expires in {1} mins", Date, Expiration.TotalMinutes, Name);
			}
			else
			{
				return String.Format("Quest '{1}' Completed {0}", Date, Name);
			}
		}
	}
}
