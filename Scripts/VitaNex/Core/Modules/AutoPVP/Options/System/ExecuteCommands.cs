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
using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPExecuteCommands : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual bool SaveEnabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Save
		{
			get => SaveEnabled;
			set
			{
				if (value)
				{
					Execute("SAVE");
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual bool LoadEnabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Load
		{
			get => LoadEnabled;
			set
			{
				if (value)
				{
					Execute("LOAD");
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual bool SyncEnabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Sync
		{
			get => SyncEnabled;
			set
			{
				if (value)
				{
					Execute("SYNC");
				}
			}
		}

		public AutoPvPExecuteCommands()
		{
			SaveEnabled = LoadEnabled = SyncEnabled = true;
		}

		public AutoPvPExecuteCommands(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Command Interface";
		}

		public override void Clear()
		{
			SaveEnabled = LoadEnabled = SyncEnabled = false;
		}

		public override void Reset()
		{
			SaveEnabled = LoadEnabled = SyncEnabled = true;
		}

		public virtual void Execute(string cmd)
		{
			switch (cmd)
			{
				case "SAVE":
				{
					if (SaveEnabled)
					{
						AutoPvP.Save();
					}
				}
				break;
				case "LOAD":
				{
					if (LoadEnabled)
					{
						AutoPvP.Load();
					}
				}
				break;
				case "SYNC":
				{
					if (SyncEnabled)
					{
						AutoPvP.Sync();
					}
				}
				break;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(SaveEnabled);
					writer.Write(LoadEnabled);
					writer.Write(SyncEnabled);
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
					SaveEnabled = reader.ReadBool();
					LoadEnabled = reader.ReadBool();
					SyncEnabled = reader.ReadBool();
				}
				break;
			}
		}
	}
}