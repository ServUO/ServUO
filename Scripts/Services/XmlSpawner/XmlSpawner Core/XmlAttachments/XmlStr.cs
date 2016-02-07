using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlStr : XmlAttachment
	{
		private TimeSpan m_Duration = TimeSpan.FromSeconds(30.0);       // default 30 sec duration
		private int m_Value = 10;       // default value of 10

		[CommandProperty(AccessLevel.GameMaster)]
		public int Value { get { return m_Value; } set { m_Value = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlStr(ASerial serial)
			: base(serial)
		{
		}

		[Attachable]
		public XmlStr()
		{
		}

		[Attachable]
		public XmlStr(int value)
		{
			m_Value = value;
		}

		[Attachable]
		public XmlStr(int value, double duration)
		{
			m_Value = value;
			m_Duration = TimeSpan.FromSeconds(duration);
		}

		public override void OnAttach()
		{
			base.OnAttach();

			// apply the mod
			if (AttachedTo is Mobile)
			{
				((Mobile)AttachedTo).AddStatMod(new StatMod(StatType.Str, "XmlStr" + Name, m_Value, m_Duration));
			}
			// and then remove the attachment
			Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));

			//Delete();
		}
	}
}
