#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server
{
	[Parsable]
	public abstract class Poison
	{
		public abstract int LabelNumber { get; }
		public abstract int RealLevel { get; }
		public abstract string Name { get; }
		public abstract int Level { get; }

		public abstract Timer ConstructTimer(Mobile m);

		public override string ToString()
		{
			return Name;
		}

		public static void Register(Poison reg)
		{
			var regName = reg.Name.ToLower();

			for (var i = 0; i < Poisons.Count; i++)
			{
				if (reg.Level == Poisons[i].Level)
				{
					throw new Exception("A poison with that level already exists.");
				}
				else if (regName == Poisons[i].Name.ToLower())
				{
					throw new Exception("A poison with that name already exists.");
				}
			}

			Poisons.Add(reg);
		}

		public static Poison Lesser => GetPoison("Lesser");
		public static Poison Regular => GetPoison("Regular");
		public static Poison Greater => GetPoison("Greater");
		public static Poison Deadly => GetPoison("Deadly");
		public static Poison Lethal => GetPoison("Lethal");

		public static Poison Parasitic => DeadlyParasitic;
		public static Poison DarkGlow => GreaterDarkglow;

		public static Poison LesserDarkglow => GetPoison("LesserDarkglow");
		public static Poison RegularDarkglow => GetPoison("RegularDarkglow");
		public static Poison GreaterDarkglow => GetPoison("GreaterDarkglow");
		public static Poison DeadlyDarkglow => GetPoison("DeadlyDarkglow");
		public static Poison LethalDarkglow => GetPoison("LethalDarkglow");

		public static Poison LesserParasitic => GetPoison("LesserParasitic");
		public static Poison RegularParasitic => GetPoison("RegularParasitic");
		public static Poison GreaterParasitic => GetPoison("GreaterParasitic");
		public static Poison DeadlyParasitic => GetPoison("DeadlyParasitic");
		public static Poison LethalParasitic => GetPoison("LethalParasitic");

		public static List<Poison> Poisons { get; } = new List<Poison>();

		public static Poison Parse(string value)
		{
			Poison p = null;

			if (Int32.TryParse(value, out var plevel))
			{
				p = GetPoison(plevel);
			}

			if (p == null)
			{
				p = GetPoison(value);
			}

			return p;
		}

		public static Poison GetPoison(int level)
		{
			for (var i = 0; i < Poisons.Count; ++i)
			{
				var p = Poisons[i];

				if (p.Level == level)
				{
					return p;
				}
			}

			return null;
		}

		public static Poison GetPoison(string name)
		{
			for (var i = 0; i < Poisons.Count; ++i)
			{
				var p = Poisons[i];

				if (Utility.InsensitiveCompare(p.Name, name) == 0)
				{
					return p;
				}
			}

			return null;
		}

		public static void Serialize(Poison p, GenericWriter writer)
		{
			if (p == null)
			{
				writer.Write((byte)0);
			}
			else
			{
				writer.Write((byte)1);
				writer.Write((byte)p.Level);
			}
		}

		public static Poison Deserialize(GenericReader reader)
		{
			switch (reader.ReadByte())
			{
				case 1: return GetPoison(reader.ReadByte());
			}

			return null;
		}
	}
}