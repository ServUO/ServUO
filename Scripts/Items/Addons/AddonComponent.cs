#region References
using System;
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Network;
#endregion

namespace Server.Items
{
	[Anvil]
	public class AnvilComponent : AddonComponent
	{
		[Constructable]
		public AnvilComponent(int itemID)
			: base(itemID)
		{ }

		public AnvilComponent(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	[Forge]
	public class ForgeComponent : AddonComponent
	{
		[Constructable]
		public ForgeComponent(int itemID)
			: base(itemID)
		{ }

		public ForgeComponent(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class LocalizedAddonComponent : AddonComponent
	{
		private int m_LabelNumber;

		[Constructable]
		public LocalizedAddonComponent(int itemID, int labelNumber)
			: base(itemID)
		{
			m_LabelNumber = labelNumber;
		}

		public LocalizedAddonComponent(Serial serial)
			: base(serial)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public int Number
		{
			get { return m_LabelNumber; }
			set
			{
				m_LabelNumber = value;
				InvalidateProperties();
			}
		}

		public override int LabelNumber { get { return m_LabelNumber; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_LabelNumber);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
				{
					m_LabelNumber = reader.ReadInt();
					break;
				}
			}
		}
	}

	public class AddonComponent : Item, IChopable
	{
		public override bool ForceShowProperties { get { return Addon != null && Addon.ForceShowProperties; } }

		private static readonly LightEntry[] m_Entries =
		{
			new LightEntry(
				LightType.WestSmall,
				1122,
				1123,
				1124,
				1141,
				1142,
				1143,
				1144,
				1145,
				1146,
				2347,
				2359,
				2360,
				2361,
				2362,
				2363,
				2364,
				2387,
				2388,
				2389,
				2390,
				2391,
				2392),
			new LightEntry(
				LightType.NorthSmall,
				1131,
				1133,
				1134,
				1147,
				1148,
				1149,
				1150,
				1151,
				1152,
				2352,
				2373,
				2374,
				2375,
				2376,
				2377,
				2378,
				2401,
				2402,
				2403,
				2404,
				2405,
				2406),
			new LightEntry(LightType.Circle300, 6526, 6538, 6571), new LightEntry(LightType.Circle150, 5703, 6587)
		};

		[Constructable]
		public AddonComponent(int itemID)
			: base(itemID)
		{
			Movable = false;
			ApplyLightTo(this);
		}

		public AddonComponent(Serial serial)
			: base(serial)
		{ }

		[CommandProperty(AccessLevel.GameMaster)]
		public BaseAddon Addon { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D Offset { get; set; }

		[Hue, CommandProperty(AccessLevel.GameMaster)]
		public override int Hue
		{
			get { return base.Hue; }
			set
			{
				base.Hue = value;

				if (Addon != null && Addon.ShareHue)
					Addon.Hue = value;
			}
		}

		public virtual bool NeedsWall { get { return false; } }
		public virtual Point3D WallPosition { get { return Point3D.Zero; } }

		public static void ApplyLightTo(Item item)
		{
			if ((item.ItemData.Flags & TileFlag.LightSource) == 0)
				return; // not a light source

			var itemID = item.ItemID;

			foreach (var entry in m_Entries)
			{
				var toMatch = entry.m_ItemIDs;
				var contains = false;

				for (var j = 0; !contains && j < toMatch.Length; ++j)
					contains = (itemID == toMatch[j]);

				if (contains)
				{
					item.Light = entry.m_Light;
					return;
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (Addon != null)
				Addon.OnComponentUsed(this, from);
		}

		public void OnChop(Mobile from)
		{
			if (Addon != null && from.InRange(GetWorldLocation(), 3))
				Addon.OnChop(from);
			else
				from.SendLocalizedMessage(500446); // That is too far away.
		}

		public override void OnLocationChange(Point3D old)
		{
			if (Addon != null)
				Addon.Location = new Point3D(X - Offset.X, Y - Offset.Y, Z - Offset.Z);
		}

		public override void OnMapChange()
		{
			if (Addon != null)
				Addon.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (Addon != null)
				Addon.Delete();
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			if (Addon != null)
			{
				Addon.GetProperties(list, this);
			}
		}

		public override void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(m, list);

			if (Addon != null)
			{
				Addon.GetContextMenuEntries(m, list);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write(Addon);
			writer.Write(Offset);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
				case 0:
				{
					Addon = reader.ReadItem() as BaseAddon;
					Offset = reader.ReadPoint3D();

                    if (Addon != null)
                    {
                        Addon.OnComponentLoaded(this);
                        ApplyLightTo(this);
                    }
                    else
                    {
                        Delete();
                    }

					break;
				}
			}

			if (version < 1 && Weight == 0)
				Weight = -1;
		}

		private class LightEntry
		{
			public readonly LightType m_Light;
			public readonly int[] m_ItemIDs;

			public LightEntry(LightType light, params int[] itemIDs)
			{
				m_Light = light;
				m_ItemIDs = itemIDs;
			}
		}
	}

	public class InstrumentedAddonComponent : AddonComponent
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public int SuccessSound { get; set; }

		[Constructable]
		public InstrumentedAddonComponent(int itemID, int wellSound)
			: base(itemID)
		{
			SuccessSound = wellSound;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(GetWorldLocation(), 2))
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
			}
			else if (from.BeginAction(typeof(InstrumentedAddonComponent)))
			{
                Timer.DelayCall(TimeSpan.FromMilliseconds(1000), () =>
                {
                    from.EndAction(typeof(InstrumentedAddonComponent));
                });

				from.PlaySound(SuccessSound);
			}
			else
			{
				from.SendLocalizedMessage(500119); // You must wait to perform another action
			}
		}

		public InstrumentedAddonComponent(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);

			writer.Write(SuccessSound);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			SuccessSound = reader.ReadInt();
		}
	}
}