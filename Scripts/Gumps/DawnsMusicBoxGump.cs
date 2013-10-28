#region Header
// **********
// ServUO - DawnsMusicBoxGump.cs
// **********
#endregion

#region References
using Server.Items;
using Server.Network;
#endregion

namespace Server.Gumps
{
	public class DawnsMusicBoxGump : Gump
	{
		private readonly DawnsMusicBox m_Box;

		public DawnsMusicBoxGump(DawnsMusicBox box)
			: base(60, 36)
		{
			m_Box = box;

			AddPage(0);

			AddBackground(0, 0, 273, 324, 0x13BE);
			AddImageTiled(10, 10, 253, 20, 0xA40);
			AddImageTiled(10, 40, 253, 244, 0xA40);
			AddImageTiled(10, 294, 253, 20, 0xA40);
			AddAlphaRegion(10, 10, 253, 304);
			AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
			AddHtmlLocalized(14, 12, 273, 20, 1075130, 0x7FFF, false, false); // Choose a track to play

			int page = 1;
			int i, y = 49;

			AddPage(page);

			for (i = 0; i < m_Box.Tracks.Count; i++, y += 24)
			{
				DawnsMusicInfo info = DawnsMusicBox.GetInfo(m_Box.Tracks[i]);

				if (i > 0 && i % 10 == 0)
				{
					AddButton(228, 294, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1);

					AddPage(page + 1);
					y = 49;

					AddButton(193, 294, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page);

					page++;
				}

				if (info == null)
				{
					continue;
				}

				AddButton(19, y, 0x845, 0x846, 100 + i, GumpButtonType.Reply, 0);
				AddHtmlLocalized(44, y - 2, 213, 20, info.Name, 0x7FFF, false, false);
			}

			if (i % 10 == 0)
			{
				AddButton(228, 294, 0xFA5, 0xFA6, 0, GumpButtonType.Page, page + 1);

				AddPage(page + 1);
				y = 49;

				AddButton(193, 294, 0xFAE, 0xFAF, 0, GumpButtonType.Page, page);
			}

			AddButton(19, y, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(44, y - 2, 213, 20, 1075207, 0x7FFF, false, false); // Stop Song
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_Box == null || m_Box.Deleted)
			{
				return;
			}

			Mobile m = sender.Mobile;

			if (!m_Box.IsChildOf(m.Backpack) && !m_Box.IsLockedDown)
			{
				m.SendLocalizedMessage(1061856); // You must have the item in your backpack or locked down in order to use it.
			}
			else if (m_Box.IsLockedDown && !m_Box.HasAccces(m))
			{
				m.SendLocalizedMessage(502691); // You must be the owner to use this.
			}
			else if (info.ButtonID == 1)
			{
				m_Box.EndMusic(m);
			}
			else if (info.ButtonID >= 100 && info.ButtonID - 100 < m_Box.Tracks.Count)
			{
				m_Box.PlayMusic(m, m_Box.Tracks[info.ButtonID - 100]);
			}
		}
	}
}