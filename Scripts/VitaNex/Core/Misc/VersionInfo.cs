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
using System.Globalization;
using System.Linq;

using Server;
#endregion

namespace VitaNex
{
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public class VersionInfoAttribute : Attribute
	{
		public VersionInfo Version { get; set; }

		public string Name { get => Version.Name; set => Version.Name = value; }
		public string Description { get => Version.Description; set => Version.Description = value; }

		public VersionInfoAttribute(string version = "1.0.0.0", string name = "", string description = "")
		{
			Version = version;
			Name = name;
			Description = description;
		}
	}

	public class VersionInfo
		: PropertyObject, IEquatable<VersionInfo>, IComparable<VersionInfo>, IEquatable<Version>, IComparable<Version>
	{
		public static Version DefaultVersion => new Version(1, 0, 0, 0);

		protected Version InternalVersion { get; set; }

		public Version Version => InternalVersion ?? (InternalVersion = DefaultVersion);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual string Value => ToString(4);

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual int Major
		{
			get => Version.Major;
			set => InternalVersion = new Version(value, Minor, Build, Revision);
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual int Minor
		{
			get => Version.Minor;
			set => InternalVersion = new Version(Major, value, Build, Revision);
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual int Build
		{
			get => Version.Build;
			set => InternalVersion = new Version(Major, Minor, value, Revision);
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual int Revision
		{
			get => Version.Revision;
			set => InternalVersion = new Version(Major, Minor, Build, value);
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual string Name { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual string Description { get; set; }

		public VersionInfo()
		{
			InternalVersion = DefaultVersion;
		}

		public VersionInfo(int major = 1, int minor = 0, int build = 0, int revision = 0)
		{
			InternalVersion = new Version(major, minor, build, revision);
		}

		public VersionInfo(string version)
		{
			if (!Version.TryParse(version, out var v))
			{
				v = DefaultVersion;
			}

			InternalVersion = new Version(
				Math.Max(0, v.Major),
				Math.Max(0, v.Minor),
				Math.Max(0, v.Build),
				Math.Max(0, v.Revision));
		}

		public VersionInfo(Version v)
		{
			InternalVersion = new Version(
				Math.Max(0, v.Major),
				Math.Max(0, v.Minor),
				Math.Max(0, v.Build),
				Math.Max(0, v.Revision));
		}

		public VersionInfo(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			InternalVersion = DefaultVersion;
		}

		public override void Reset()
		{
			InternalVersion = DefaultVersion;
		}

		public override string ToString()
		{
			return Version.ToString();
		}

		public virtual string ToString(int fieldCount)
		{
			return Version.ToString(fieldCount);
		}

		public override int GetHashCode()
		{
			return Version.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is Version && Equals((Version)obj)) || (obj is VersionInfo && Equals((VersionInfo)obj));
		}

		public virtual bool Equals(Version other)
		{
			return !ReferenceEquals(other, null) && Version == other;
		}

		public virtual bool Equals(VersionInfo other)
		{
			return !ReferenceEquals(other, null) && Version == other.Version;
		}

		public virtual int CompareTo(Version other)
		{
			return !ReferenceEquals(other, null) ? Version.CompareTo(other) : -1;
		}

		public virtual int CompareTo(VersionInfo other)
		{
			return !ReferenceEquals(other, null) ? Version.CompareTo(other.Version) : -1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 1:
				{
					writer.Write(Name);
					writer.Write(Description);
				}
				goto case 0;
				case 0:
				{
					writer.Write(Version.Major);
					writer.Write(Version.Minor);
					writer.Write(Version.Build);
					writer.Write(Version.Revision);
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
				case 1:
				{
					Name = reader.ReadString();
					Description = reader.ReadString();
				}
				goto case 0;
				case 0:
				{
					int major = reader.ReadInt(), minor = reader.ReadInt(), build = reader.ReadInt(), revision = reader.ReadInt();

					InternalVersion = new Version(Math.Max(0, major), Math.Max(0, minor), Math.Max(0, build), Math.Max(0, revision));
				}
				break;
			}
		}

		public static bool TryParse(string s, out VersionInfo version)
		{
			version = DefaultVersion;

			if (String.IsNullOrWhiteSpace(s))
			{
				return false;
			}

			var value = String.Empty;

			foreach (var c in s.Select(c => c.ToString(CultureInfo.InvariantCulture)))
			{
				if (c == ".")
				{
					if (value.Length > 0)
					{
						value += c;
					}

					continue;
				}

				if (Byte.TryParse(c, out var b))
				{
					value += b;
				}
			}

			if (Version.TryParse(value, out var v))
			{
				version = new Version(Math.Max(0, v.Major), Math.Max(0, v.Minor), Math.Max(0, v.Build), Math.Max(0, v.Revision));
				return true;
			}

			return false;
		}

		public static implicit operator VersionInfo(string version)
		{
			return new VersionInfo(version);
		}

		public static implicit operator string(VersionInfo version)
		{
			return version.Value;
		}

		public static implicit operator VersionInfo(Version version)
		{
			return new VersionInfo(version);
		}

		public static implicit operator Version(VersionInfo a)
		{
			return a.Version;
		}

		public static bool operator ==(VersionInfo l, VersionInfo r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(VersionInfo l, VersionInfo r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}

		public static bool operator <=(VersionInfo v1, VersionInfo v2)
		{
			return (!ReferenceEquals(v1, null) && !ReferenceEquals(v2, null) && v1.Version <= v2.Version);
		}

		public static bool operator >=(VersionInfo v1, VersionInfo v2)
		{
			return (!ReferenceEquals(v1, null) && !ReferenceEquals(v2, null) && v1.Version >= v2.Version);
		}

		public static bool operator <(VersionInfo v1, VersionInfo v2)
		{
			return (!ReferenceEquals(v1, null) && !ReferenceEquals(v2, null) && v1.Version < v2.Version);
		}

		public static bool operator >(VersionInfo v1, VersionInfo v2)
		{
			return (!ReferenceEquals(v1, null) && !ReferenceEquals(v2, null) && v1.Version > v2.Version);
		}
	}
}