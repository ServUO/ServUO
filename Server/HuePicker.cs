#region References
using System;

using Server.Network;
#endregion

namespace Server.HuePickers
{
	public class HuePicker
	{
		private static int m_NextSerial = 1;

		private readonly int m_Serial;
		private readonly int m_ItemID;

		public int Serial => m_Serial;

		public int ItemID => m_ItemID;

		public HuePicker(int itemID)
		{
			do
			{
				m_Serial = m_NextSerial++;
			}
			while (m_Serial == 0);

			m_ItemID = itemID;
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
