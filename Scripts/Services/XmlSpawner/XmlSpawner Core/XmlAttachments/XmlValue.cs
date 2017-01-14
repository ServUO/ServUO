using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlValue : XmlAttachment
	{
		private int m_DataValue;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Value { get{ return m_DataValue; } set { m_DataValue = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlValue(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlValue(string name, int value)
		{
			Name = name;
			Value = value;
		}

		[Attachable]
		public XmlValue(string name, int value, double expiresin)
		{
			Name = name;
			Value = value;
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
			m_DataValue = reader.ReadInt();
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			if(Expiration > TimeSpan.Zero)
			{
				return String.Format("{2}: Value {0} expires in {1} mins",Value,Expiration.TotalMinutes, Name);
			} 
			else
			{
				return String.Format("{1}: Value {0}",Value, Name);
			}
		}
	}
}
