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
	public enum PvPBattleWorldBroadcastMode
	{
		Disabled,
		Broadcast,
		TownCrier,
		Notify
	}

	[PropertyObject]
	public class PvPBattleWorldBroadcasts : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public PvPBattleWorldBroadcastMode Mode { get; set; }

		[Hue, CommandProperty(AutoPvP.Access)]
		public int MessageHue { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool OpenNotify { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool StartNotify { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool EndNotify { get; set; }

		public PvPBattleWorldBroadcasts()
		{
			OpenNotify = true;
			StartNotify = true;
			EndNotify = true;
			MessageHue = 85;
			Mode = PvPBattleWorldBroadcastMode.Broadcast;
		}

		public PvPBattleWorldBroadcasts(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "World Broadcasts";
		}

		public override void Clear()
		{
			OpenNotify = false;
			StartNotify = false;
			EndNotify = false;
			MessageHue = 0;
			Mode = PvPBattleWorldBroadcastMode.Disabled;
		}

		public override void Reset()
		{
			OpenNotify = true;
			StartNotify = true;
			EndNotify = true;
			MessageHue = 85;
			Mode = PvPBattleWorldBroadcastMode.Broadcast;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteFlag(Mode);
					writer.Write(MessageHue);
					writer.Write(OpenNotify);
					writer.Write(StartNotify);
					writer.Write(EndNotify);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
				{
					Mode = reader.ReadFlag<PvPBattleWorldBroadcastMode>();
					MessageHue = reader.ReadInt();
					OpenNotify = reader.ReadBool();
					StartNotify = reader.ReadBool();
					EndNotify = reader.ReadBool();
				}
				break;
			}
		}
	}
}