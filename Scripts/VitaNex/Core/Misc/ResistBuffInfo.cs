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
	public class UniqueResistMod : ResistanceMod
	{
		public static bool ApplyTo(Mobile m, ResistanceType type, string name, int offset)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			RemoveFrom(m, type, name);

			return new UniqueResistMod(type, name, offset).ApplyTo(m);
		}

		public static bool RemoveFrom(Mobile m, ResistanceType type, string name)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (m.ResistanceMods != null)
			{
				return m.ResistanceMods.OfType<UniqueResistMod>().Where(rm => rm.Type == type && rm.Name == name).Count(mod => mod.RemoveFrom(m)) > 0;
			}

			return false;
		}

		public string Name { get; set; }

		public UniqueResistMod(ResistanceType res, string name, int value)
			: base(res, value)
		{
			Name = name;
		}

		public virtual bool ApplyTo(Mobile from)
		{
			if (from == null || from.Deleted)
			{
				return false;
			}

			from.AddResistanceMod(this);
			return true;
		}

		public virtual bool RemoveFrom(Mobile from)
		{
			if (from == null || from.Deleted)
			{
				return false;
			}

			from.RemoveResistanceMod(this);
			return true;
		}
	}

	public sealed class ResistBuffInfo : PropertyObject, IEquatable<ResistBuffInfo>, IEquatable<ResistanceMod>, ICloneable
	{
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public ResistanceType Type { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Offset { get; set; }

		public ResistBuffInfo(ResistanceType type, int offset)
		{
			Type = type;
			Offset = offset;
		}

		public ResistBuffInfo(GenericReader reader)
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

		public ResistBuffInfo Clone()
		{
			return new ResistBuffInfo(Type, Offset);
		}

		public ResistanceMod ToResistMod()
		{
			return new ResistanceMod(Type, Offset);
		}

		public override string ToString()
		{
			return Type.ToString();
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)Type * 397) ^ Offset;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ResistanceMod) || Equals(obj as ResistBuffInfo);
		}

		public bool Equals(ResistanceMod mod)
		{
			return mod != null && Type == mod.Type && Offset == mod.Offset;
		}

		public bool Equals(ResistBuffInfo info)
		{
			return info != null && Type == info.Type && Offset == info.Offset;
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
					writer.Write(Offset);
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
					Type = reader.ReadFlag<ResistanceType>();
					Offset = reader.ReadInt();
				}
				break;
			}
		}

		public static bool operator ==(ResistBuffInfo l, ResistBuffInfo r)
		{
			return l?.Equals(r) ?? r?.Equals(l) ?? true;
		}

		public static bool operator !=(ResistBuffInfo l, ResistBuffInfo r)
		{
			return !l?.Equals(r) ?? !r?.Equals(l) ?? false;
		}

		public static bool operator ==(ResistBuffInfo l, ResistanceMod r)
		{
			return l?.Equals(r) ?? r?.Equals(l) ?? true;
		}

		public static bool operator !=(ResistBuffInfo l, ResistanceMod r)
		{
			return !l?.Equals(r) ?? !r?.Equals(l) ?? false;
		}

		public static bool operator ==(ResistanceMod l, ResistBuffInfo r)
		{
			return r == l;
		}

		public static bool operator !=(ResistanceMod l, ResistBuffInfo r)
		{
			return r != l;
		}

		public static implicit operator ResistanceMod(ResistBuffInfo info)
		{
			return new ResistanceMod(info.Type, info.Offset);
		}
	}
}
