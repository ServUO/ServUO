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
	public class RuneCodexCategoryGrid : Grid<RuneCodexCategory>
	{
		public RuneCodexCategoryGrid()
			: base(10, 10)
		{
			SetContent(0, 0, new RuneCodexCategory());
		}

		public RuneCodexCategoryGrid(GenericReader reader)
			: base(reader)
		{ }

		public override RuneCodexCategory this[int x, int y]
		{
			get
			{
				if (x == 0 && y == 0 && base[x, y] == null)
				{
					return (base[x, y] = new RuneCodexCategory());
				}

				return base[x, y];
			}
			set
			{
				if (x == 0 && y == 0 && value == null)
				{
					value = new RuneCodexCategory();
				}

				base[x, y] = value;
			}
		}

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

		public override void SerializeContent(GenericWriter writer, RuneCodexCategory content, int x, int y)
		{
			base.SerializeContent(writer, content, x, y);

			if (OnSerializeContent == null)
			{
				content.Serialize(writer);
			}
		}

		public override RuneCodexCategory DeserializeContent(GenericReader reader, Type type, int x, int y)
		{
			if (OnDeserializeContent == null)
			{
				return new RuneCodexCategory(reader);
			}

			return base.DeserializeContent(reader, type, x, y);
		}
	}
}