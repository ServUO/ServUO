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
using Server.Items;

using VitaNex.Crypto;
using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public sealed class PvPSerial : CryptoHashCode
	{
		public static CryptoHashType Algorithm = CryptoHashType.MD5;

		[CommandProperty(AutoPvP.Access)]
		public override string Value => base.Value.Replace("-", String.Empty);

		public PvPSerial()
			: this(TimeStamp.UtcNow + "+" + Utility.RandomDouble())
		{ }

		public PvPSerial(string seed)
			: base(Algorithm, seed)
		{ }

		public PvPSerial(GenericReader reader)
			: base(reader)
		{ }
	}

	public abstract partial class PvPBattle
	{
		[CommandProperty(AutoPvP.Access, true)]
		public PvPSerial Serial { get; private set; }

		protected bool Deserialized { get; private set; }
		protected bool Deserializing { get; private set; }

		private PvPBattle(bool deserializing)
		{
			Deserialized = deserializing;

			EnsureConstructDefaults();
		}

		public PvPBattle(GenericReader reader)
			: this(true)
		{
			Deserializing = true;

			Deserialize(reader);

			Deserializing = false;
		}

		public virtual void SerializeRegion(GenericWriter w, PvPRegion r)
		{
			if (r != null)
			{
				r.Serialize(w);
			}
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(10);

			if (version > 5)
			{
				writer.WriteBlock(
					w =>
					{
						if (version > 6)
						{
							Serial.Serialize(w);
						}
						else
						{
							w.WriteType(Serial, t => Serial.Serialize(w));
						}
					});
			}

			switch (version)
			{
				case 10:
					writer.Write(Shortcut);
					goto case 9;
				case 9:
					writer.Write(RewardTeam);
					goto case 8;
				case 8:
					writer.Write(RequireCapacity);
					goto case 7;
				case 7:
				case 6:
				case 5:
					writer.Write(Hidden);
					goto case 4;
				case 4:
					writer.Write(_FloorItemDelete);
					goto case 3;
				case 3:
				case 2:
					writer.Write(Gate);
					goto case 1;
				case 1:
				{
					writer.Write(Category);
					writer.Write(Ranked);
					writer.Write(InviteWhileRunning);
				}
				goto case 0;
				case 0:
				{
					if (version < 6)
					{
						writer.WriteBlock(w => w.WriteType(Serial, t => Serial.Serialize(w)));
					}

					writer.Write(DebugMode);
					writer.WriteFlag(_State);
					writer.Write(_Name);
					writer.Write(Description);
					writer.Write(AutoAssign);
					writer.Write(UseTeamColors);
					writer.Write(false); // IgnoreCapacity
					writer.Write(_SubCommandPrefix);
					writer.Write(QueueAllowed);
					writer.Write(SpectateAllowed);
					writer.Write(KillPoints);
					writer.Write(PointsBase);
					writer.Write(0.0); // PointsRankFactor
					writer.Write(IdleKick);
					writer.Write(IdleThreshold);
					writer.WriteFlag(LastState);
					writer.Write(LastStateChange);
					writer.Write(_LightLevel);
					writer.Write(LogoutDelay);

					writer.WriteItemList(Doors, true);

					writer.WriteBlock(w => w.WriteType(Options, t => Options.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Schedule, t => Schedule.Serialize(w)));

					writer.WriteBlock(w => w.WriteType(BattleRegion, t => SerializeRegion(w, BattleRegion)));
					writer.WriteBlock(w => w.WriteType(SpectateRegion, t => SerializeRegion(w, SpectateRegion)));

					writer.WriteBlockList(Teams, (w, team) => w.WriteType(team, t => team.Serialize(w)));
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var v = reader.GetVersion();

			if (v > 5)
			{
				Serial = reader.ReadBlock(r => v > 6 ? new PvPSerial(r) : r.ReadTypeCreate<PvPSerial>(r)) ?? new PvPSerial();
			}

			switch (v)
			{
				case 10:
					Shortcut = reader.ReadString();
					goto case 9;
				case 9:
					RewardTeam = reader.ReadBool();
					goto case 8;
				case 8:
					RequireCapacity = reader.ReadBool();
					goto case 7;
				case 7:
				case 6:
				case 5:
					Hidden = reader.ReadBool();
					goto case 4;
				case 4:
					_FloorItemDelete = reader.ReadBool();
					goto case 3;
				case 3:
				case 2:
				{
					Gate = reader.ReadItem<PvPSpectatorGate>();

					if (Gate != null)
					{
						Gate.Battle = this;
					}
				}
				goto case 1;
				case 1:
				{
					Category = reader.ReadString();
					Ranked = reader.ReadBool();
					InviteWhileRunning = reader.ReadBool();
				}
				goto case 0;
				case 0:
				{
					if (v < 6)
					{
						Serial = reader.ReadBlock(r => r.ReadTypeCreate<PvPSerial>(r)) ?? new PvPSerial();
					}

					DebugMode = reader.ReadBool();
					_State = reader.ReadFlag<PvPBattleState>();
					_Name = reader.ReadString();
					Description = reader.ReadString();
					AutoAssign = reader.ReadBool();
					UseTeamColors = reader.ReadBool();
					reader.ReadBool(); // IgnoreCapacity
					_SubCommandPrefix = reader.ReadChar();
					QueueAllowed = reader.ReadBool();
					SpectateAllowed = reader.ReadBool();
					KillPoints = v < 3 ? (reader.ReadBool() ? 1 : 0) : reader.ReadInt();
					PointsBase = reader.ReadInt();
					reader.ReadDouble(); //PointsRankFactor
					IdleKick = reader.ReadBool();
					IdleThreshold = reader.ReadTimeSpan();
					LastState = reader.ReadFlag<PvPBattleState>();
					LastStateChange = reader.ReadDateTime();
					_LightLevel = reader.ReadInt();
					LogoutDelay = reader.ReadTimeSpan();

					Doors.AddRange(reader.ReadStrongItemList<BaseDoor>());

					Options = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleOptions>(r)) ?? new PvPBattleOptions();

					if (Schedule != null && Schedule.Running)
					{
						Schedule.Stop();
					}

					Schedule = reader.ReadBlock(r => r.ReadTypeCreate<Schedule>(r)) ?? new Schedule(Name, false);

					if (Schedule.Name != Name && Name != null)
					{
						Schedule.Name = Name;
					}

					BattleRegion = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleRegion>(this, r));
					SpectateRegion = reader.ReadBlock(r => r.ReadTypeCreate<PvPSpectateRegion>(this, r));

					Teams = reader.ReadBlockList(r => r.ReadTypeCreate<PvPTeam>(this, r), Teams);
				}
				break;
			}
		}
	}
}