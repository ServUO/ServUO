using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Items;

namespace Server.Services.ChampionSystem
{
	public class TestRadiusCommand
	{
		private static int m_Radius;
		private static List<Item> m_Markers = new List<Item>();

		public static void Initialize()
		{
			CommandSystem.Register("TestRadius", AccessLevel.GameMaster, new CommandEventHandler(TestRadius_OnCommand));
		}

		private static void TestRadius_OnCommand(CommandEventArgs e)
		{
			End();

			if (String.IsNullOrEmpty(e.ArgString))
				return;

			if(!int.TryParse(e.ArgString, out m_Radius))
			{
				e.Mobile.SendMessage("Usage: TestRadius <Int32>");
			}

			Map map = e.Mobile.Map;
			int cx = e.Mobile.X;
			int cy = e.Mobile.Y;
			int minCompare = (m_Radius - 1) * (m_Radius - 1);
			int maxCompare = m_Radius * m_Radius;

			/*
			for(int y = e.Mobile.Y - m_Radius; y <= e.Mobile.Y + m_Radius; ++y)
			{
				AddMarker(e.Mobile.X - m_Radius, y, map, false);
				AddMarker(e.Mobile.X + m_Radius, y, map, false);
			}
			for (int x = e.Mobile.X - m_Radius; x < e.Mobile.X + m_Radius; ++x)
			{
				AddMarker(x, e.Mobile.Y - m_Radius, map, true);
				AddMarker(x, e.Mobile.Y + m_Radius, map, true);
			}
			*/
			for (int y = cy - m_Radius; y <= cy + m_Radius; ++y)
			{
				for (int x = cx - m_Radius; x <= cx + m_Radius; ++x)
				{
					int d = (cx - x) * (cx - x) + (cy - y) * (cy - y);
					if (d > minCompare && d <= maxCompare)
					{
						AddMarker(x, y, map, true);
						AddMarker(x, y, map, false);
					}
				}
			}
		}

		private static void AddMarker(int x, int y, Map map, bool north)
		{
			int z = map.GetAverageZ(x, y);
			Item item = new Item(north ? 0x3975 : 0x3987);
			item.Visible = false;
			item.MoveToWorld(new Point3D(x, y, z), map);
			m_Markers.Add(item);
		}

		private static void End()
		{
			foreach (Item item in m_Markers)
			{
				item.Delete();
			}
			m_Markers.Clear();
		}
	}
}
