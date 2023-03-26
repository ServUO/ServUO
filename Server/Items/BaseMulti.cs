namespace Server.Items
{
	public class BaseMulti : Item
	{
		[Hue, CommandProperty(AccessLevel.Counselor)]
		public virtual int MultiHue => Hue;

		[CommandProperty(AccessLevel.GameMaster)]
		public override int ItemID
		{
			get => base.ItemID;
			set
			{
				if (base.ItemID != value)
				{
					var facet = Parent == null ? Map : null;

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

		public override int LabelNumber
		{
			get
			{
				var mcl = Components;

				if (mcl.List.Length > 0)
				{
					int id = mcl.List[0].m_ItemID;

					return id < 0x4000 ? 1020000 + id : 1078872 + id;
				}

				return base.LabelNumber;
			}
		}

		public virtual bool AllowsRelativeDrop => false;

		public virtual MultiComponentList Components => MultiData.GetComponents(ItemID);

		[Constructable]
		public BaseMulti(int itemID)
			: base(itemID)
		{
			Movable = false;
		}

		public BaseMulti(Serial serial)
			: base(serial)
		{ }

		public override int GetUpdateRange(Mobile m)
		{
			var min = base.GetUpdateRange(m);
			var max = Core.GlobalRadarRange - 1;

			var w = Components.Width;
			var h = Components.Height - 1;
			var v = min + ((w > h ? w : h) / 2);

			if (v > max)
				v = max;
			else if (v < min)
				v = min;

			return v;
		}

		public override int GetPacketFlags()
		{
			var f = base.GetPacketFlags();

			if (!ForceShowProperties)
			{
				f &= ~0x20;
			}

			return f;
		}

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
			var mcl = Components;

			x -= X + mcl.Min.m_X;
			y -= Y + mcl.Min.m_Y;

			return x >= 0 && x < mcl.Width && y >= 0 && y < mcl.Height && mcl.Tiles[x][y].Length > 0;
		}

		public virtual bool Contains(Mobile m)
		{
			return m.Map == Map && Contains(m.X, m.Y);
		}

		public virtual bool Contains(Item item)
		{
			return item.Map == Map && Contains(item.X, item.Y);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();
		}
	}
}