#region References
using System;
using System.Collections.Generic;

using Server.Items;
using Server.Network;
#endregion

namespace Server
{
	public class Sector
	{
		private const int m_SliceCapacity = 1 << 8;

		private static readonly Queue<Action> m_SliceQueue = new Queue<Action>();

		internal static void Slice()
		{
			if (m_SliceQueue.Count == 0)
			{
				return;
			}
			
			while (m_SliceQueue.Count > 0)
			{
				m_SliceQueue.Dequeue()();

				if (m_SliceQueue.Count == m_SliceCapacity)
				{
					m_SliceQueue.TrimExcess();
				}
			}
		}

		public SortedDictionary<Region, HashSet<Rectangle3D>> RegionRects { get; } = new SortedDictionary<Region, HashSet<Rectangle3D>>();

		public HashSet<BaseMulti> Multis { get; } = new HashSet<BaseMulti>();
		public HashSet<Mobile> Mobiles { get; } = new HashSet<Mobile>();
		public HashSet<Item> Items { get; } = new HashSet<Item>();
		public HashSet<NetState> Clients { get; } = new HashSet<NetState>();
		public HashSet<Mobile> Players { get; } = new HashSet<Mobile>();

		public int MultiCount => Multis.Count;
		public int MobileCount => Mobiles.Count;
		public int ItemCount => Items.Count;
		public int ClientCount => Clients.Count;
		public int PlayerCount => Players.Count;

		private bool m_Active;

		public bool Active => m_Active && Owner != Map.Internal;

		public int X { get; }
		public int Y { get; }

		public Map Owner { get; }

		public Sector(int x, int y, Map owner)
		{
			X = x;
			Y = y;

			Owner = owner;
		}

		public void OnClientChange(NetState oldState, NetState newState)
		{
			if (oldState != null)
			{
				Clients.Remove(oldState);
			}

			if (newState != null)
			{
				Clients.Add(newState);
			}
		}

		public void OnEnter(Item item)
		{
			if (item != null)
			{
				Items.Add(item);
			}
		}

		public void OnLeave(Item item)
		{
			if (item != null)
			{
				Items.Remove(item);
			}
		}

		public void OnEnter(Mobile mob)
		{
			if (mob != null)
			{
				Mobiles.Add(mob);

				if (mob.NetState != null)
				{
					Clients.Add(mob.NetState);
				}

				if (mob.Player)
				{
					if (Players.Count == 0)
					{
						Owner.ActivateSectors(X, Y);
					}

					Players.Add(mob);
				}
			}
		}

		public void OnLeave(Mobile mob)
		{
			if (mob != null)
			{
				Mobiles.Remove(mob);

				if (mob.NetState != null)
				{
					Clients.Remove(mob.NetState);
				}

				if (mob.Player && Players != null)
				{
					Players.Remove(mob);

					if (Players.Count == 0)
					{
						Owner.DeactivateSectors(X, Y);
					}
				}
			}
		}

		public void OnEnter(Region region, Rectangle3D rect)
		{
			if (region != null)
			{
				if (!RegionRects.TryGetValue(region, out var rects))
				{
					RegionRects[region] = rects = new HashSet<Rectangle3D>();
				}

				if (rects.Add(rect))
				{
					UpdateMobileRegions();
				}
			}
		}

		public void OnLeave(Region region)
		{
			if (region != null)
			{
				if (RegionRects.Remove(region))
				{
					UpdateMobileRegions();
				}
			}
		}

		private void UpdateMobileRegions()
		{
			foreach (var m in Mobiles)
			{
				m_SliceQueue.Enqueue(m.UpdateRegion);
			}
		}

		public void OnMultiEnter(BaseMulti multi)
		{
			if (multi != null)
			{
				Multis.Add(multi);
			}
		}

		public void OnMultiLeave(BaseMulti multi)
		{
			if (multi != null)
			{
				Multis.Remove(multi);
			}
		}

		public void Activate()
		{
			if (!m_Active && Owner != Map.Internal)
			{
				foreach (var o in Items)
				{
					m_SliceQueue.Enqueue(o.OnSectorActivate);
				}

				foreach (var m in Mobiles)
				{
					m_SliceQueue.Enqueue(m.OnSectorActivate);
				}

				m_Active = true;
			}
		}

		public void Deactivate()
		{
			if (m_Active && Owner != Map.Internal)
			{
				foreach (var o in Items)
				{
					m_SliceQueue.Enqueue(o.OnSectorDeactivate);
				}

				foreach (var m in Mobiles)
				{
					m_SliceQueue.Enqueue(m.OnSectorDeactivate);
				}

				m_Active = false;
			}
		}
	}
}
