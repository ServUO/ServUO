#region Header
// **********
// ServUO - Utilities.cs
// **********
#endregion

#region References
using System;
using System.IO;

using Server;
using Server.Items;
#endregion

namespace CustomsFramework
{
	public enum SaveStrategyTypes
	{
		StandardSaveStrategy,
		DualSaveStrategy,
		DynamicSaveStrategy,
		ParallelSaveStrategy
	}

	public enum OldClientResponse
	{
		Ignore,
		Warn,
		Annoy,
		LenientKick,
		Kick
	}

	public static class Utilities
	{
		public static int WriteVersion(this GenericWriter writer, int version)
		{
			writer.Write(version);

			return version;
		}

		public static int ReaderVersion(this GenericReader reader, int version)
		{
			return reader.ReadInt();
		}

		public static void CheckFileStructure(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		// TODO: Make this factor-in GeneralSettings AccessLevel options?
		public static bool IsPlayer(this Mobile from)
		{
			return from.AccessLevel <= AccessLevel.VIP;
		}

		// TODO: Make this factor-in GeneralSettings AccessLevel options?
		public static bool IsStaff(this Mobile from)
		{
			return from.AccessLevel >= AccessLevel.Counselor;
		}

		// TODO: Make this factor-in GeneralSettings AccessLevel options?
		public static bool IsOwner(this Mobile from)
		{
			return from.AccessLevel >= AccessLevel.CoOwner;
		}

		public static bool IsDigit(this string text)
		{
			int value;
			return IsDigit(text, out value);
		}

		public static bool IsDigit(this string text, out int value)
		{
			return Int32.TryParse(text, out value);
		}

		public static SaveStrategy GetSaveStrategy(this SaveStrategyTypes saveStrategyTypes)
		{
			switch (saveStrategyTypes)
			{
				case SaveStrategyTypes.StandardSaveStrategy:
					return new StandardSaveStrategy();
				case SaveStrategyTypes.DualSaveStrategy:
					return new DualSaveStrategy();
				case SaveStrategyTypes.DynamicSaveStrategy:
					return new DynamicSaveStrategy();
				case SaveStrategyTypes.ParallelSaveStrategy:
					return new ParallelSaveStrategy(Core.ProcessorCount);
				default:
					return new StandardSaveStrategy();
			}
		}

		public static SaveStrategyTypes GetSaveType(this SaveStrategy saveStrategy)
		{
			if (saveStrategy is DualSaveStrategy)
			{
				return SaveStrategyTypes.StandardSaveStrategy;
			}

			if (saveStrategy is StandardSaveStrategy)
			{
				return SaveStrategyTypes.StandardSaveStrategy;
			}

			if (saveStrategy is DynamicSaveStrategy)
			{
				return SaveStrategyTypes.DynamicSaveStrategy;
			}

			if (saveStrategy is ParallelSaveStrategy)
			{
				return SaveStrategyTypes.ParallelSaveStrategy;
			}

			return SaveStrategyTypes.StandardSaveStrategy;
		}

		public static void PlaceItemIn(this Container container, Item item, Point3D location)
		{
			container.AddItem(item);
			item.Location = location;
		}

		public static void PlaceItemIn(this Container container, Item item, int x = 0, int y = 0, int z = 0)
		{
			PlaceItemIn(container, item, new Point3D(x, y, z));
		}

		public static Item BlessItem(this Item item)
		{
			item.LootType = LootType.Blessed;

			return item;
		}

		public static Item MakeNewbie(this Item item)
		{
			if (!Core.AOS)
			{
				item.LootType = LootType.Newbied;
			}

			return item;
		}

		public static void DumpToConsole(params object[] elements)
		{
			Console.WriteLine();

			foreach (var element in elements)
			{
				Console.WriteLine(ObjectDumper.Dump(element));
				Console.WriteLine();
			}
		}
	}
}