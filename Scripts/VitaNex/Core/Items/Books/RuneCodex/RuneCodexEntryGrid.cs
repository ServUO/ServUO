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

namespace VitaNex.Items
{
	public class RuneCodexEntryGrid : Grid<RuneCodexEntry>
	{
		public RuneCodexEntryGrid()
			: base(10, 10)
		{ }

		public RuneCodexEntryGrid(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}

		public override void SerializeContent(GenericWriter writer, RuneCodexEntry content, int x, int y)
		{
			base.SerializeContent(writer, content, x, y);

			if (OnSerializeContent == null)
			{
				content.Serialize(writer);
			}
		}

		public override RuneCodexEntry DeserializeContent(GenericReader reader, Type type, int x, int y)
		{
			if (OnDeserializeContent == null)
			{
				return new RuneCodexEntry(reader);
			}

			return base.DeserializeContent(reader, type, x, y);
		}
	}
}