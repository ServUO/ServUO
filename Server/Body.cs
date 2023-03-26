#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public enum BodyType : byte
	{
		Empty,
		Monster,
		Sea,
		Animal,
		Human,
		Equipment
	}

	[PropertyObject]
	public struct Body : IComparable<Body>, IEquatable<Body>, IEquatable<BodyType>, IEquatable<int>
	{
		private static readonly char[] m_Splitters = { '\t', ' ' };

		public const int MinValue = 0;
		public const int MaxValue = 4095;

		public static Body[] Bodies { get; } = new Body[MaxValue + 1];
		public static BodyType[] Types { get; } = new BodyType[MaxValue + 1];

		public static bool[] Gargoyles { get; } = new bool[MaxValue + 1];
		public static bool[] Ghosts { get; } = new bool[MaxValue + 1];
		public static bool[] Males { get; } = new bool[MaxValue + 1];
		public static bool[] Females { get; } = new bool[MaxValue + 1];

		static Body()
		{
			Load();
		}

		public static void Clear()
		{
			Array.Clear(Bodies, 0, Bodies.Length);
			Array.Clear(Types, 0, Types.Length);

			Array.Clear(Gargoyles, 0, Gargoyles.Length);
			Array.Clear(Ghosts, 0, Ghosts.Length);
			Array.Clear(Males, 0, Males.Length);
			Array.Clear(Females, 0, Females.Length);
		}

		public static void Load()
		{
			var path = Core.FindDataFile("mobtypes.txt");

			if (!File.Exists(path))
			{
				Utility.WriteWarning($"File not found: {path}");
				return;
			}

			Clear();

			foreach (var line in File.ReadLines(path))
			{
				var entry = line.Trim();

				if (entry.Length == 0 || entry.StartsWith("#"))
				{
					continue;
				}

				var split = entry.Split(m_Splitters, StringSplitOptions.RemoveEmptyEntries);

				if (split.Length > 4)
				{
					var subsplit = new string[4];

					Array.Copy(split, subsplit, subsplit.Length);

					split = subsplit;
				}

				if (split.Length < 2)
				{
					Utility.WriteWarning($"Invalid body entry: {entry}");
					continue;
				}

				if (!Int32.TryParse(split[0], out var body) || body < 0 || body >= Types.Length)
				{
					Utility.WriteWarning($"Invalid body entry: {entry}");
					continue;
				}

				var space = split[1].IndexOf('_');

				if (space > 0)
				{
					split[1] = split[1].Substring(0, space);
				}

				if (!Enum.TryParse(split[1], true, out BodyType type))
				{
					Utility.WriteWarning($"Invalid body entry: {entry}");
					continue;
				}

				Types[body] = type;
				Bodies[body] = body;
			}

			foreach (var body in Config.GetArray<int>("Animations.GargoyleBodies", true))
			{
				Gargoyles[body] = true;
			}

			foreach (var body in Config.GetArray<int>("Animations.GhostBodies", true))
			{
				Ghosts[body] = true;
			}

			foreach (var body in Config.GetArray<int>("Animations.MaleBodies", true))
			{
				Males[body] = true;
			}

			foreach (var body in Config.GetArray<int>("Animations.FemaleBodies", true))
			{
				Females[body] = true;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int BodyID { get; }

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsValid => BodyID >= MinValue && BodyID <= MaxValue;

		[CommandProperty(AccessLevel.Counselor)]
		public BodyType Type => IsValid ? Types[BodyID] : BodyType.Empty;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsEmpty => IsValid && Type == BodyType.Empty;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsMonster => IsValid && Type == BodyType.Monster;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsSea => IsValid && Type == BodyType.Sea;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsAnimal => IsValid && Type == BodyType.Animal;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsHuman => IsValid && Type == BodyType.Human && !IsGhost;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsEquipment => IsValid && Type == BodyType.Equipment;

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsGargoyle => IsValid && Gargoyles[BodyID];

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsMale => IsValid && Males[BodyID];

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsFemale => IsValid && Females[BodyID];

		[CommandProperty(AccessLevel.Counselor)]
		public bool IsGhost => IsValid && Ghosts[BodyID];

		public Body(int bodyID)
		{
			BodyID = bodyID;
		}

		public override string ToString()
		{
			return $"0x{BodyID:X}";
		}

		public override int GetHashCode()
		{
			return BodyID;
		}

		public override bool Equals(object o)
		{
			if (o is Body b)
			{
				return Equals(b);
			}

			if (o is BodyType t)
			{
				return Equals(t);
			}

			if (o is int i)
			{
				return Equals(i);
			}

			return false;
		}

		public bool Equals(Body other)
		{
			return BodyID.Equals(other.BodyID);
		}

		public bool Equals(BodyType other)
		{
			return Type.Equals(other);
		}

		public bool Equals(int other)
		{
			return BodyID.Equals(other);
		}

		public int CompareTo(Body other)
		{
			return BodyID.CompareTo(other.BodyID);
		}

		public static bool operator ==(Body left, Body right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Body left, Body right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(Body left, Body right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator <=(Body left, Body right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >(Body left, Body right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator >=(Body left, Body right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static implicit operator int(Body a)
		{
			return a.BodyID;
		}

		public static implicit operator Body(int a)
		{
			return new Body(a);
		}
	}
}
