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
	public sealed class TournamentArchive : PropertyObject, IEnumerable<TournamentMatch>
	{
		[CommandProperty(AutoPvP.Access)]
		public List<TournamentMatch> Matches { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public int Count => Matches.Count;

		[CommandProperty(AutoPvP.Access)]
		public string Name { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public DateTime Date { get; private set; }

		public TournamentArchive(string name, DateTime date)
		{
			Name = name;
			Date = date;

			Matches = new List<TournamentMatch>();
		}

		public TournamentArchive(GenericReader reader)
			: base(reader)
		{ }

		public IEnumerator<TournamentMatch> GetEnumerator()
		{
			return Matches.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Name);
			writer.Write(Date);

			writer.WriteList(Matches, (w, o) => o.Serialize(w));
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Name = reader.ReadString();
			Date = reader.ReadDateTime();

			Matches = reader.ReadList(r => new TournamentMatch(r), Matches);
		}
	}
}