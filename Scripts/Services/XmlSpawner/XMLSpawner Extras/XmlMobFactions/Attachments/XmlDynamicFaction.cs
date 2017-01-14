using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using System.Text;

namespace Server.Engines.XmlSpawner2
{
	public class XmlDynamicFaction : XmlAttachment
	{
		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlDynamicFaction(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlDynamicFaction(string name)
		{
			Name = name;
		}
		
		public static bool MatchFaction(Mobile o, string name)
		{
			if(o == null || name == null) return false;

			// look for any attachments on the object that match this one
			List<XmlAttachment> list = XmlAttach.FindAttachments(o, typeof(XmlDynamicFaction));
			
			if(list != null && list.Count > 0)
			{
				foreach( XmlAttachment a in list)
				{
					if(a.Name.ToLower() == name.ToLower()) return true;
				}
			}
			return false;
		}

		public static string Title(Mobile o)
		{
			if(o == null) return null;
			
			StringBuilder title = new StringBuilder();

			// look for any attachments on the object that match this one
			List<XmlAttachment> list = XmlAttach.FindAttachments(o, typeof(XmlDynamicFaction));
			
			if(list != null && list.Count > 0)
			{
				foreach( XmlAttachment a in list)
				{
					title.AppendFormat(" [{0}]", a.Name);
				}
			}
			return title.ToString();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
		}

		public override string OnIdentify(Mobile from)
		{
			return String.Format("Member of the '{0}' faction",Name);
		}
	}
}
