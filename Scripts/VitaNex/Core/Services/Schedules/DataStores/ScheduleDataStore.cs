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
using System.Collections.Generic;
using System.IO;

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Schedules
{
	public class ScheduleDataStore : BinaryDataStore<string, Schedule>
	{
		public ScheduleDataStore(string root, string doc)
			: base(root, doc)
		{ }

		public ScheduleDataStore(DirectoryInfo root, string doc)
			: base(root, doc)
		{ }

		protected override void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteDictionary(
						this,
						(k, v) =>
						{
							writer.Write(k);
							writer.WriteType(
								v,
								t =>
								{
									if (t != null)
									{
										v.Serialize(writer);
									}
								});
						});
				}
				break;
			}
		}

		protected override void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
				{
					reader.ReadDictionary(
						() =>
						{
							var key = reader.ReadString();
							var val = reader.ReadTypeCreate<Schedule>(reader);
							return new KeyValuePair<string, Schedule>(key, val);
						},
						this);
				}
				break;
			}
		}
	}
}