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
	public class PvPBattleOptions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleBroadcasts Broadcasts { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleLocations Locations { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleRestrictions Restrictions { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPRewards Rewards { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleRules Rules { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleSounds Sounds { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleSuddenDeath SuddenDeath { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleTiming Timing { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleWeather Weather { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleMissions Missions { get; set; }

		public PvPBattleOptions()
		{
			Broadcasts = new PvPBattleBroadcasts();
			Locations = new PvPBattleLocations();
			Restrictions = new PvPBattleRestrictions();
			Rewards = new PvPRewards();
			Rules = new PvPBattleRules();
			Sounds = new PvPBattleSounds();
			SuddenDeath = new PvPBattleSuddenDeath();
			Timing = new PvPBattleTiming();
			Weather = new PvPBattleWeather();
			Missions = new PvPBattleMissions();
		}

		public PvPBattleOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Broadcasts.Clear();
			Locations.Clear();
			Restrictions.Clear();
			Rewards.Clear();
			Rules.Clear();
			Sounds.Clear();
			SuddenDeath.Clear();
			Timing.Clear();
			Weather.Clear();
			Missions.Clear();
		}

		public override void Reset()
		{
			Broadcasts.Reset();
			Locations.Reset();
			Restrictions.Reset();
			Rewards.Reset();
			Rules.Reset();
			Sounds.Reset();
			SuddenDeath.Reset();
			Timing.Reset();
			Weather.Reset();
			Missions.Reset();
		}

		public override string ToString()
		{
			return "Advanced Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.WriteBlock(w => w.WriteType(Missions, t => Missions.Serialize(w)));
					goto case 0;
				case 0:
				{
					writer.WriteBlock(w => w.WriteType(Broadcasts, t => Broadcasts.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Locations, t => Locations.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Restrictions, t => Restrictions.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Rewards, t => Rewards.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Rules, t => Rules.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Sounds, t => Sounds.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(SuddenDeath, t => SuddenDeath.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Timing, t => Timing.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Weather, t => Weather.Serialize(w)));
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
				case 1:
					Missions = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleMissions>(r)) ?? new PvPBattleMissions();
					goto case 0;
				case 0:
				{
					Broadcasts = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleBroadcasts>(r)) ?? new PvPBattleBroadcasts();
					Locations = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleLocations>(r)) ?? new PvPBattleLocations();
					Restrictions = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleRestrictions>(r)) ?? new PvPBattleRestrictions();
					Rewards = reader.ReadBlock(r => r.ReadTypeCreate<PvPRewards>(r)) ?? new PvPRewards();
					Rules = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleRules>(r)) ?? new PvPBattleRules();
					Sounds = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleSounds>(r)) ?? new PvPBattleSounds();
					SuddenDeath = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleSuddenDeath>(r)) ?? new PvPBattleSuddenDeath();
					Timing = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleTiming>(r)) ?? new PvPBattleTiming();
					Weather = reader.ReadBlock(r => r.ReadTypeCreate<PvPBattleWeather>(r)) ?? new PvPBattleWeather();
				}
				break;
			}

			if (version < 1)
			{
				Missions = new PvPBattleMissions();
			}
		}
	}
}