using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Engines.CannedEvil
{
	public class GoldShower
	{
		public static void DoForChamp(Point3D center, Map map)
		{
			Do(center, map, ChampionSystem.GoldShowerPiles, ChampionSystem.GoldShowerMinAmount, ChampionSystem.GoldShowerMaxAmount);
		}
		public static void DoForHarrower(Point3D center, Map map)
		{
			Do(center, map, ChampionSystem.HarrowerGoldShowerPiles, ChampionSystem.HarrowerGoldShowerMinAmount, ChampionSystem.HarrowerGoldShowerMaxAmount);
		}
		public static void Do(Point3D center, Map map, int piles, int minAmount, int maxAmount)
		{
			new GoodiesTimer(center, map, piles, minAmount, maxAmount).Start();
		}

		private class GoodiesTimer : Timer
		{
			private readonly Map m_Map;
			private readonly Point3D m_Location;
			private readonly int m_PilesMax;
			private int m_PilesDone = 0;
			private readonly int m_MinAmount;
			private readonly int m_MaxAmount;
			public GoodiesTimer(Point3D center, Map map, int piles, int minAmount, int maxAmount)
				: base(TimeSpan.FromSeconds(0.25d), TimeSpan.FromSeconds(0.25d))
			{
				m_Location = center;
				m_Map = map;
				m_PilesMax = piles;
				m_MinAmount = minAmount;
				m_MaxAmount = maxAmount;
			}

			protected override void OnTick()
			{
				if(m_PilesDone >= m_PilesMax)
				{
					Stop();
					return;
				}

				Point3D p = FindGoldLocation(m_Map, m_Location, m_PilesMax / 8);
				Gold g = new Gold(m_MinAmount, m_MaxAmount);
				g.MoveToWorld(p, this.m_Map);

				switch (Utility.Random(3))
				{
					case 0: // Fire column
						Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
						Effects.PlaySound(g, g.Map, 0x208);
						break;
					case 1: // Explosion
						Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
						Effects.PlaySound(g, g.Map, 0x307);
						break;
					case 2: // Ball of fire
						Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);
						break;
				}
				++m_PilesDone;
			}

			private static Point3D FindGoldLocation(Map map, Point3D center, int range)
			{
				int cx = center.X;
				int cy = center.Y;

				for (int i = 0; i < 20; ++i)
				{
					int x = cx + Utility.Random(range * 2) - range;
					int y = cy + Utility.Random(range * 2) - range;
					if ((cx - x) * (cx - x) + (cy - y) * (cy - y) > range * range)
						continue;

					int z = map.GetAverageZ(x, y);
					if (!map.CanFit(x, y, z, 6, false, false))
						continue;

					int topZ = z;
					foreach (Item item in map.GetItemsInRange(new Point3D(x, y, z), 0))
					{
						topZ = Math.Max(topZ, item.Z + item.ItemData.CalcHeight);
					}
					return new Point3D(x, y, topZ);
				}
				return center;
			}
		}
	}
}
