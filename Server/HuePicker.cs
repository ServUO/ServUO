#region References
using System;

using Server.Network;
#endregion

namespace Server.HuePickers
{
	public class HuePicker
	{
		private static int m_NextSerial = 1;

		public int Serial { get; }
		public int ItemID { get; }

		public HuePicker(int itemID)
		{
			do
			{
				Serial = m_NextSerial++;
			}
			while (Serial == 0);

			ItemID = itemID;
		}

		public virtual void Clip(ref int hue)
		{
			hue = Math.Max(0, Math.Min(1000, hue));
		}

		public virtual void OnResponse(int hue)
		{ }

		public void SendTo(NetState state)
		{
			state.Send(new DisplayHuePicker(this));
			state.AddHuePicker(this);
		}
	}
}
