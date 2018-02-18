#region Header
// **********
// ServUO - BaseMulti.cs
// **********
#endregion

#region References
using Server.Network;
using System;
#endregion

namespace Server.Items
{
	public class BaseMulti : Item
	{
		[Constructable]
		public BaseMulti(int itemID)
			: base(itemID)
		{
			Movable = false;
		}

		public BaseMulti(Serial serial)
			: base(serial)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public override int ItemID
		{
			get { return base.ItemID; }
			set
			{
				if (base.ItemID != value)
				{
					Map facet = (Parent == null ? Map : null);

					if (facet != null)
					{
						facet.OnLeave(this);
					}

					base.ItemID = value;

					if (facet != null)
					{
						facet.OnEnter(this);
					}
				}
			}
		}

		[Obsolete("Replace with calls to OnLeave and OnEnter surrounding component invalidation.", true)]
		public virtual void RefreshComponents()
		{
			if (Parent == null)
			{
				Map facet = Map;

				if (facet != null)
				{
					facet.OnLeave(this);
					facet.OnEnter(this);
				}
			}
		}

        public override int LabelNumber
		{
			get
			{
				MultiComponentList mcl = Components;

				if (mcl.List.Length > 0)
				{
					int id = mcl.List[0].m_ItemID;

					if (id < 0x4000)
					{
						return 1020000 + id;
					}
					else
					{
						return 1078872 + id;
					}
				}

				return base.LabelNumber;
			}
		}

		public virtual bool AllowsRelativeDrop { get { return false; } }
	
		public override int GetUpdateRange(Mobile m)
		{
			// Multis should update at maximum range given their center tile must be in the update range.
			// This will prevent castles, keeps and the minax fortress from suddenly appearaing out of nowhere.
			// (Any return value higher than the global max will cause the multi to not load at all)
			return Core.GlobalMaxUpdateRange;
		}

		public virtual MultiComponentList Components { get { return MultiData.GetComponents(ItemID); } }

		public virtual bool Contains(Point2D p)
		{
			return Contains(p.m_X, p.m_Y);
		}

		public virtual bool Contains(Point3D p)
		{
			return Contains(p.m_X, p.m_Y);
		}

		public virtual bool Contains(IPoint3D p)
		{
			return Contains(p.X, p.Y);
		}

		public virtual bool Contains(int x, int y)
		{
			MultiComponentList mcl = Components;

			x -= X + mcl.Min.m_X;
			y -= Y + mcl.Min.m_Y;

			return x >= 0 && x < mcl.Width && y >= 0 && y < mcl.Height && mcl.Tiles[x][y].Length > 0;
		}

		public bool Contains(Mobile m)
		{
			if (m.Map == Map)
			{
				return Contains(m.X, m.Y);
			}
			else
			{
				return false;
			}
		}

		public bool Contains(Item item)
		{
			if (item.Map == Map)
			{
				return Contains(item.X, item.Y);
			}
			else
			{
				return false;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if (version == 0)
			{
				if (ItemID >= 0x4000)
				{
					ItemID -= 0x4000;
				}
			}
		}
	}
}