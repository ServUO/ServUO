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
using System.Collections;
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public sealed class TournamentRecords : PropertyObject, IEnumerable<TournamentRecord>
	{
		[CommandProperty(AutoPvP.Access)]
		public List<TournamentRecord> Entries { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public int Count => Entries.Count;

		[CommandProperty(AutoPvP.Access)]
		public DateTime DateBegin => Count > 0 ? Entries.Lowest(o => o.Time.Ticks).Time : DateTime.MinValue;

		[CommandProperty(AutoPvP.Access)]
		public DateTime DateEnd => Count > 0 ? Entries.Highest(o => o.Time.Ticks).Time : DateTime.MaxValue;

		public TournamentRecords()
		{
			Entries = new List<TournamentRecord>();
		}

		public TournamentRecords(GenericReader reader)
			: base(reader)
		{ }

		public void Record(string format, params object[] args)
		{
			Record(String.Format(format, args));
		}

		public void Record(string value)
		{
			Entries.Add(new TournamentRecord(value));
		}

		public IEnumerator<TournamentRecord> GetEnumerator()
		{
			return Entries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteList(Entries, (w, o) => o.Serialize(w));
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Entries = reader.ReadList(r => new TournamentRecord(r), Entries);
		}
	}
}