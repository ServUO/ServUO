#region Header
// **********
// ServUO - LastEditedBy.cs
// **********
#endregion

#region References
using System;

using CustomsFramework;
#endregion

namespace Server
{
	public class LastEditedBy
	{
		private Mobile _Mobile;
		private DateTime _Time;

		public LastEditedBy(Mobile mobile)
		{
			_Mobile = mobile;
			_Time = DateTime.UtcNow;
		}

		public LastEditedBy(GenericReader reader)
		{
			Deserialize(reader);
		}

		[CommandProperty(AccessLevel.Decorator)]
		public Mobile Mobile { get { return _Mobile; } set { _Mobile = value; } }

		[CommandProperty(AccessLevel.Decorator)]
		public DateTime Time { get { return _Time; } set { _Time = value; } }

		public void Serialize(GenericWriter writer)
		{
			writer.WriteVersion(0);

			// Version 0
			writer.Write(_Mobile);
			writer.Write(_Time);
		}

		private void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						_Mobile = reader.ReadMobile();
						_Time = reader.ReadDateTime();
						break;
					}
			}
		}
	}
}