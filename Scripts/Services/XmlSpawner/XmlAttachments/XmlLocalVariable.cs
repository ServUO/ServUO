using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlLocalVariable : XmlAttachment
	{
		private string m_DataValue = null;    // default data

		[CommandProperty( AccessLevel.GameMaster )]
		public string Data { get{ return m_DataValue; } set { m_DataValue = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlLocalVariable(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlLocalVariable(string name)
		{
			Name = name;
			Data = String.Empty;
		}

		[Attachable]
		public XmlLocalVariable(string name, string data)
		{
			Name = name;
			Data = data;
		}

		[Attachable]
		public XmlLocalVariable(string name, string data, double expiresin)
		{
			Name = name;
			Data = data;
			Expiration = TimeSpan.FromMinutes(expiresin);

		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write((string)m_DataValue);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_DataValue = reader.ReadString();
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			if(Expiration > TimeSpan.Zero)
			{
				return String.Format("{2} = {0} : expires in {1} mins",Data,Expiration.TotalMinutes, Name);
			} 
			else
			{
				return String.Format("{1} = {0}",Data, Name);
			}
		}
	}
}
