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

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AntiAdverts
{
	public sealed class AntiAdvertsReport : IEquatable<AntiAdvertsReport>
	{
		public DateTime Date { get; private set; }
		public Mobile Mobile { get; private set; }

		public string Report { get; set; }
		public string Speech { get; set; }
		public bool Jailed { get; set; }
		public bool Banned { get; set; }
		public bool Viewed { get; set; }

		public AntiAdvertsReport(
			DateTime date,
			Mobile m,
			string report,
			string speech,
			bool jailed,
			bool banned,
			bool viewed = false)
		{
			Date = date;
			Mobile = m;
			Speech = speech;
			Report = report;
			Viewed = viewed;
			Jailed = jailed;
			Banned = banned;
		}

		public AntiAdvertsReport(GenericReader reader)
		{
			Deserialize(reader);
		}

		public override string ToString()
		{
			return String.Format(
				"[{0}] {1}: {2}",
				Date.ToSimpleString("t@h:m@ m-d"),
				Mobile == null ? "-null-" : Mobile.RawName,
				Report);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Mobile != null ? Mobile.Serial.Value : 0);
				hashCode = (hashCode * 397) ^ Date.GetHashCode();
				hashCode = (hashCode * 397) ^ (Speech != null ? Speech.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Report != null ? Report.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Jailed.GetHashCode();
				hashCode = (hashCode * 397) ^ Banned.GetHashCode();
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is AntiAdvertsReport && Equals((AntiAdvertsReport)obj);
		}

		public bool Equals(AntiAdvertsReport other)
		{
			return !ReferenceEquals(other, null) && Mobile == other.Mobile && Date == other.Date && Jailed == other.Jailed &&
				   Banned == other.Banned && Speech == other.Speech && Report == other.Report;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.Write(Date);
			writer.Write(Mobile);
			writer.Write(Speech);
			writer.Write(Report);
			writer.Write(Viewed);
			writer.Write(Jailed);
			writer.Write(Banned);
		}

		public void Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			Date = reader.ReadDateTime();
			Mobile = reader.ReadMobile<PlayerMobile>();
			Speech = reader.ReadString();
			Report = reader.ReadString();
			Viewed = reader.ReadBool();
			Jailed = reader.ReadBool();
			Banned = reader.ReadBool();
		}

		public static bool operator ==(AntiAdvertsReport l, AntiAdvertsReport r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(AntiAdvertsReport l, AntiAdvertsReport r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}