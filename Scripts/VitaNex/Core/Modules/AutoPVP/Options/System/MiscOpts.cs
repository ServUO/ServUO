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

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPMiscOptions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual bool UseCategories { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool DeserterAssoc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual TimeSpan DeserterLockout { get; set; }

		public AutoPvPMiscOptions()
		{
			UseCategories = true;

			DeserterAssoc = true;
			DeserterLockout = TimeSpan.FromMinutes(30);
		}

		public AutoPvPMiscOptions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Misc Options";
		}

		public override void Clear()
		{
			UseCategories = false;

			DeserterAssoc = false;
			DeserterLockout = TimeSpan.Zero;
		}

		public override void Reset()
		{
			UseCategories = true;

			DeserterAssoc = true;
			DeserterLockout = TimeSpan.FromMinutes(30);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.Write(DeserterAssoc);
					writer.Write(DeserterLockout);
				}
				goto case 0;
				case 0:
					writer.Write(UseCategories);
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
					DeserterAssoc = reader.ReadBool();
					DeserterLockout = reader.ReadTimeSpan();
				}
				goto case 0;
				case 0:
					UseCategories = reader.ReadBool();
					break;
			}
		}
	}
}