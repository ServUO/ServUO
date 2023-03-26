using System;
using System.Collections.Generic;

namespace Server.Items
{
	public static class DailyRares
	{
		public static HashSet<Item> Rares { get; } = new HashSet<Item>();

		[ConfigProperty("DailyRares.Enabled")]
		public static bool Enabled
		{
			get => Config.Get("DailyRares.Enabled", true);
			set
			{
				Config.Set("DailyRares.Enabled", value);

				Invalidate();
			}
		}

		public static void Initialize()
		{
			Invalidate();

			EventSink.AfterWorldSave += e => Rares.RemoveWhere(o => o.Deleted || o.Parent != null || o.LastMoved < DateTime.UtcNow);
		}

		public static void Invalidate()
		{
			// rocks
			SetItem<DailyRocks>(Map.Felucca, 2684, 2060, 28);
			SetItem<DailyRocks>(Map.Trammel, 2684, 2060, 28);

			// rock
			SetItem<DailyRock>(Map.Felucca, 5511, 3116, -4);
			SetItem<DailyRock>(Map.Trammel, 5511, 3116, -4);

			// fruit basket
			SetItem<FruitBasket>(Map.Felucca, 2636, 2081, 16);
			SetItem<FruitBasket>(Map.Trammel, 2636, 2081, 16);

			// fruit basket
			SetItem<FruitBasket>(Map.Felucca, 286, 986, 6);
			SetItem<FruitBasket>(Map.Trammel, 286, 986, 6);

			// closed barrel
			SetItem<ClosedBarrel>(Map.Felucca, 5191, 587, 0);
			SetItem<ClosedBarrel>(Map.Trammel, 5191, 587, 0);

			// closed barrel
			SetItem<ClosedBarrel>(Map.Felucca, 5301, 592, 0);
			SetItem<ClosedBarrel>(Map.Trammel, 5301, 592, 0);

			// large candle
			SetItem<CandleLarge>(Map.Felucca, 5575, 1829, 6);
			SetItem<CandleLarge>(Map.Trammel, 5575, 1829, 6);

			// full jars
			SetItem<DailyFullJars>(Map.Felucca, 3656, 2506, 0);

			// hay
			SetItem<DecoHay2>(Map.Felucca, 5998, 3774, 19);

			// broken chair
			SetItem<DailyBrokenChair>(Map.Ilshenar, 148, 946, -29);

			// meat pie
			SetItem<DailyMeatPie>(Map.Malas, 2112, 1311, -44);

			// seaweed
			SetItem<DailySeaweed>(Map.Felucca, 4548, 2400, -5);
			SetItem<DailySeaweed>(Map.Trammel, 4548, 2400, -5);
		}

		public static void SetItem<T>(Map map, int x, int y, int z) where T : Item, new()
		{
			if (map == null || map == Map.Internal)
			{
				return;
			}

			var loc = new Point3D(x, y, z);
			var item = map.FindItem<T>(loc);

			if (Enabled)
			{
				if (item?.Deleted != false)
				{
					item = new T();
				}

				SetItem(item, loc, map);
			}
			else if (item != null)
			{
				item.Delete();

				Rares.Remove(item);
			}
		}

		public static void SetItem(Item item, Point3D p, Map map)
		{
			if (item == null)
			{
				return;
			}

			item.MoveToWorld(p, map);

			item.LastMoved = DateTime.UtcNow.AddDays(1);

			if (item is BaseLight light)
			{
				light.Burning = true;
			}

			Rares.Add(item);
		}
	}

	public interface IDailyRare : ISpawnable
	{
		DateTime LastMoved { get; set; }
	}

	public class DailyRocks : Item, IDailyRare
	{
		[Constructable]
		public DailyRocks()
			: base(0x1367)
		{
		}

		public DailyRocks(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class DailyRock : Item, IDailyRare
	{
		[Constructable]
		public DailyRock()
			: base(0x1368)
		{
		}

		public DailyRock(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class DailyFullJars : Item, IDailyRare
	{
		[Constructable]
		public DailyFullJars()
			: base(0xE48)
		{
		}

		public DailyFullJars(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class DailyBrokenChair : Item, IDailyRare
	{
		[Constructable]
		public DailyBrokenChair()
			: base(0xC19)
		{
		}

		public DailyBrokenChair(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class DailySeaweed : Item, IDailyRare
	{
		[Constructable]
		public DailySeaweed()
			: base(0xDBA)
		{
		}

		public DailySeaweed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class DailyMeatPie : Food, IDailyRare
	{
		public override int LabelNumber => 1060141;  // a tasty meat pie

		[Constructable]
		public DailyMeatPie()
			: base(0x1041)
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 5;
		}

		public DailyMeatPie(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}
