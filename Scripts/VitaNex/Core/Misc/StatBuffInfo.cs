#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Linq;

using Server;
#endregion

namespace VitaNex
{
	public class UniqueStatMod : StatMod
	{
		public static bool ApplyTo(Mobile m, StatType type, string name, int offset, TimeSpan duration)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			RemoveFrom(m, type, name);

			return new UniqueStatMod(type, name, offset, duration).ApplyTo(m);
		}

		public static bool RemoveFrom(Mobile m, StatType type, string name)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (m.StatMods != null)
			{
				return m.StatMods.OfType<UniqueStatMod>().Where(sm => sm.Type.HasFlag(type) && sm.Name == name).Count(mod => mod.RemoveFrom(m)) > 0;
			}

			return false;
		}

		public UniqueStatMod(StatType stat, string name, int value, TimeSpan duration)
			: base(stat, name, value, duration)
		{ }

		public virtual bool ApplyTo(Mobile from)
		{
			if (from == null || from.Deleted)
			{
				return false;
			}

			from.AddStatMod(this);
			return true;
		}

		public virtual bool RemoveFrom(Mobile from)
		{
			if (from == null || from.Deleted)
			{
				return false;
			}

			from.RemoveStatMod(Name);
			return true;
		}
	}

	public sealed class StatBuffInfo : PropertyObject, IEquatable<StatBuffInfo>, IEquatable<StatMod>, ICloneable
	{
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public StatType Type { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public string Name { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Offset { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public TimeSpan Duration { get; set; }

		public StatBuffInfo(StatType type, string name, int offset, TimeSpan duration)
		{
			Type = type;
			Name = name;
			Offset = offset;
			Duration = duration;
		}

		public StatBuffInfo(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{ }

		public override void Reset()
		{ }

		object ICloneable.Clone()
		{
			return Clone();
		}

		public StatBuffInfo Clone()
		{
			return new StatBuffInfo(Type, Name, Offset, Duration);
		}

		public StatMod ToStatMod()
		{
			return new StatMod(Type, Name, Offset, Duration);
		}

		public override string ToString()
		{
			return String.IsNullOrWhiteSpace(Name) ? Name : Type.ToString();
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int)Type;
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as StatMod) || Equals(obj as StatBuffInfo);
		}

		public bool Equals(StatMod mod)
		{
			return mod != null && Type == mod.Type && Name == mod.Name;
		}

		public bool Equals(StatBuffInfo info)
		{
			return info != null && Type == info.Type && Name == info.Name;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteFlag(Type);
					writer.Write(Name);
					writer.Write(Offset);
					writer.Write(Duration);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Type = reader.ReadFlag<StatType>();
					Name = reader.ReadString();
					Offset = reader.ReadInt();
					Duration = reader.ReadTimeSpan();
				}
				break;
			}
		}

		public static bool operator ==(StatBuffInfo l, StatBuffInfo r)
		{
			return l?.Equals(r) ?? r?.Equals(l) ?? true;
		}

		public static bool operator !=(StatBuffInfo l, StatBuffInfo r)
		{
			return !l?.Equals(r) ?? !r?.Equals(l) ?? false;
		}

		public static bool operator ==(StatBuffInfo l, StatMod r)
		{
			return l?.Equals(r) ?? r?.Equals(l) ?? true;
		}

		public static bool operator !=(StatBuffInfo l, StatMod r)
		{
			return !l?.Equals(r) ?? !r?.Equals(l) ?? false;
		}

		public static bool operator ==(StatMod l, StatBuffInfo r)
		{
			return r == l;
		}

		public static bool operator !=(StatMod l, StatBuffInfo r)
		{
			return r != l;
		}

		public static implicit operator StatMod(StatBuffInfo info)
		{
			return new StatMod(info.Type, info.Name, info.Offset, info.Duration);
		}
	}
}
