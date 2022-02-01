#region References
using System;
using System.Collections.Generic;

using Server.Items;
using Server.Network;
#endregion

namespace Server
{
	public class RegionRect : IComparable, IComparable<RegionRect>
	{
		private readonly Region m_Region;
		private Rectangle3D m_Rect;

		public Region Region => m_Region;
		public Rectangle3D Rect => m_Rect;

		public RegionRect(Region region, Rectangle3D rect)
		{
			m_Region = region;
			m_Rect = rect;
		}

		public bool Contains(IPoint3D loc)
		{
			return m_Rect.Contains(loc);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is RegionRect regRect)
				return m_Region.CompareTo(regRect.m_Region);

			return -1;
		}

		int IComparable<RegionRect>.CompareTo(RegionRect regRect)
		{
			if (regRect != null)
				return m_Region.CompareTo(regRect.m_Region);

			return -1;
		}
	}

	public class Sector
	{
		private static void Add<T, L>(ref L list, T value) where L : class, ICollection<T>, new()
		{
			if (list == null)
			{
				list = new L();
			}

			list.Add(value);
		}

		private static void Remove<T, L>(ref L list, T value) where L : class, ICollection<T>
		{
			Remove(ref list, value, true);
		}

		private static void Remove<T, L>(ref L list, T value, bool free) where L : class, ICollection<T>
		{
			if (list != null)
			{
				list.Remove(value);

				if (free && list.Count == 0)
				{
					list = null;
				}
			}
		}

		private HashSet<IEntity> m_ActivationBuffer;
		private HashSet<Mobile> m_UpdateBuffer;

		private SortedSet<RegionRect> m_RegionRects;

		public IEnumerable<RegionRect> RegionRects
		{
			get
			{
				if (m_RegionRects != null)
				{
					foreach (var o in m_RegionRects)
					{
						yield return o;
					}
				}
			}
		}

		private HashSet<BaseMulti> m_Multis;

		public IEnumerable<BaseMulti> Multis
		{
			get
			{
				if (m_Multis != null)
				{
					foreach (var o in m_Multis)
					{
						yield return o;
					}
				}
			}
		}

		private HashSet<Mobile> m_Mobiles;

		public IEnumerable<Mobile> Mobiles
		{
			get
			{
				if (m_Mobiles != null)
				{
					foreach (var o in m_Mobiles)
					{
						yield return o;
					}
				}
			}
		}

		private HashSet<Item> m_Items;

		public IEnumerable<Item> Items
		{
			get
			{
				if (m_Items != null)
				{
					foreach (var o in m_Items)
					{
						yield return o;
					}
				}
			}
		}

		private HashSet<NetState> m_Clients;

		public IEnumerable<NetState> Clients
		{
			get
			{
				if (m_Clients != null)
				{
					foreach (var o in m_Clients)
					{
						yield return o;
					}
				}
			}
		}

		private HashSet<Mobile> m_Players;

		public IEnumerable<Mobile> Players
		{
			get
			{
				if (m_Players != null)
				{
					foreach (var o in m_Players)
					{
						yield return o;
					}
				}
			}
		}

		public int MultiCount => m_Multis?.Count ?? 0;
		public int MobileCount => m_Mobiles?.Count ?? 0;
		public int ItemCount => m_Items?.Count ?? 0;
		public int ClientCount => m_Clients?.Count ?? 0;
		public int PlayerCount => m_Players?.Count ?? 0;

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
			Remove(ref m_Clients, oldState, false);
			Add(ref m_Clients, newState);
		}

		public void OnEnter(Item item)
		{
			Add(ref m_Items, item);
		}

		public void OnLeave(Item item)
		{
			Remove(ref m_Items, item);
		}

		public void OnEnter(Mobile mob)
		{
			Add(ref m_Mobiles, mob);

			var ns = mob.NetState;

			if (ns != null)
			{
				Add(ref m_Clients, ns);
			}

			if (mob.Player)
			{
				if (m_Players == null)
				{
					Owner.ActivateSectors(X, Y);
				}

				Add(ref m_Players, mob);
			}
		}

		public void OnLeave(Mobile mob)
		{
			Remove(ref m_Mobiles, mob);

			if (mob.NetState != null)
			{
				Remove(ref m_Clients, mob.NetState);
			}

			if (mob.Player)
			{
				Remove(ref m_Players, mob);

				if (m_Players == null)
				{
					Owner.DeactivateSectors(X, Y);
				}
			}
		}

		public void OnEnter(Region region, Rectangle3D rect)
		{
			Add(ref m_RegionRects, new RegionRect(region, rect));

			UpdateMobileRegions();
		}

		public void OnLeave(Region region)
		{
			if (m_RegionRects != null)
			{
				m_RegionRects.RemoveWhere(r => r.Region == region);

				if (m_RegionRects.Count == 0)
				{
					m_RegionRects = null;
				}
			}

			UpdateMobileRegions();
		}

		private void UpdateMobileRegions()
		{
			if (m_Mobiles != null)
			{
				m_UpdateBuffer.UnionWith(m_Mobiles);
			}

			foreach (var m in m_UpdateBuffer)
			{
				m.UpdateRegion();
			}

			m_UpdateBuffer.Clear();
			m_UpdateBuffer.TrimExcess();
			m_UpdateBuffer = null;
		}

		public void OnMultiEnter(BaseMulti multi)
		{
			Add(ref m_Multis, multi);
		}

		public void OnMultiLeave(BaseMulti multi)
		{
			Remove(ref m_Multis, multi);
		}

		public void Activate()
		{
			if (!Active && Owner != Map.Internal)
			{
				if (m_Items != null)
				{
					m_ActivationBuffer.UnionWith(m_Items);
				}

				if (m_Mobiles != null)
				{
					m_ActivationBuffer.UnionWith(m_Mobiles);
				}

				foreach (var e in m_ActivationBuffer)
				{
					e.OnSectorActivate();
				}

				m_ActivationBuffer.Clear();
				m_ActivationBuffer.TrimExcess();
				m_ActivationBuffer = null;

				m_Active = true;
			}
		}

		public void Deactivate()
		{
			if (Active)
			{
				if (m_Items != null)
				{
					m_ActivationBuffer.UnionWith(m_Items);
				}

				if (m_Mobiles != null)
				{
					m_ActivationBuffer.UnionWith(m_Mobiles);
				}

				foreach (var e in m_ActivationBuffer)
				{
					e.OnSectorDeactivate();
				}

				m_ActivationBuffer.Clear();
				m_ActivationBuffer.TrimExcess();
				m_ActivationBuffer = null;

				m_Active = false;
			}
		}
	}
}
