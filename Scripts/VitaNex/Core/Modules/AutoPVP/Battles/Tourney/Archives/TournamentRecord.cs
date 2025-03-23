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
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class TournamentRecord : PropertyObject
	{
		[CommandProperty(AutoPvP.Access, true)]
		public DateTime Time { get; private set; }

		[CommandProperty(AutoPvP.Access, true)]
		public string Value { get; private set; }

		public TournamentRecord(string value)
		{
			Time = DateTime.UtcNow;
			Value = value ?? String.Empty;
		}

		public TournamentRecord(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Time);
			writer.Write(Value);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Time = reader.ReadDateTime();
			Value = reader.ReadString();
		}
	}
}