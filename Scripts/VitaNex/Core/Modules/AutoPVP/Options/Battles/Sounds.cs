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
	public class PvPBattleSounds : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual bool Enabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int InviteSend { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int InviteAccept { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int InviteCancel { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int QueueJoin { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int QueueLeave { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int Teleport { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattleOpened { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlePreparing { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattleStarted { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattleEnded { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int BattleCanceled { get; set; }

		public PvPBattleSounds()
		{
			SetDefaults();
		}

		public PvPBattleSounds(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Battle Sounds";
		}

		public override void Clear()
		{
			Enabled = false;

			InviteSend = -1;
			InviteAccept = -1;
			InviteCancel = -1;

			QueueJoin = -1;
			QueueLeave = -1;

			Teleport = -1;

			BattleOpened = -1;
			BattlePreparing = -1;
			BattleStarted = -1;
			BattleEnded = -1;
			BattleCanceled = -1;
		}

		public override void Reset()
		{
			SetDefaults();
		}

		public virtual void SetDefaults()
		{
			Enabled = true;

			InviteSend = 0xF0;
			InviteAccept = 0x5B5;
			InviteCancel = 0x5B4;

			QueueJoin = 0x664;
			QueueLeave = 0x51C;

			Teleport = 0x029;

			BattleOpened = 0x2E8;
			BattlePreparing = 0x2E8;
			BattleStarted = 0x2E9;
			BattleEnded = 0x2EA;
			BattleCanceled = 0x2EA;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Enabled);

					writer.Write(InviteSend);
					writer.Write(InviteAccept);
					writer.Write(InviteCancel);

					writer.Write(QueueJoin);
					writer.Write(QueueLeave);

					writer.Write(Teleport);

					writer.Write(BattleOpened);
					writer.Write(BattlePreparing);
					writer.Write(BattleStarted);
					writer.Write(BattleEnded);
					writer.Write(BattleCanceled);
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
					Enabled = reader.ReadBool();

					InviteSend = reader.ReadInt();
					InviteAccept = reader.ReadInt();
					InviteCancel = reader.ReadInt();

					QueueJoin = reader.ReadInt();
					QueueLeave = reader.ReadInt();

					Teleport = reader.ReadInt();

					BattleOpened = reader.ReadInt();
					BattlePreparing = reader.ReadInt();
					BattleStarted = reader.ReadInt();
					BattleEnded = reader.ReadInt();
					BattleCanceled = reader.ReadInt();
				}
				break;
			}
		}
	}
}