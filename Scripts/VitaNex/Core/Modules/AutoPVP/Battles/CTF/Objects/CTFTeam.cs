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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class CTFTeam : PvPTeam
	{
		private static readonly TimeSpan _OneSecond = TimeSpan.FromSeconds(1.0);

		private CTFPodium _FlagPodium;
		private bool _SolidHueOverride = true;

		public Dictionary<PlayerMobile, int> Attackers { get; private set; }
		public Dictionary<PlayerMobile, int> Defenders { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public CTFBattle CTFBattle => Battle as CTFBattle;

		[CommandProperty(AutoPvP.Access, true)]
		public CTFFlag Flag { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool SolidHueOverride
		{
			get => _SolidHueOverride;
			set
			{
				_SolidHueOverride = value;
				InvalidateSolidHueOverride();
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual TimeSpan FlagRespawnDelay { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual CTFPodium FlagPodium
		{
			get => _FlagPodium;
			set
			{
				_FlagPodium = value;
				InvalidateFlagPodium();
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public override Point3D HomeBase
		{
			get => base.HomeBase;
			set
			{
				base.HomeBase = value;
				InvalidateFlagPodium();
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public override Point3D SpawnPoint
		{
			get => base.SpawnPoint;
			set
			{
				base.SpawnPoint = value;
				InvalidateFlagPodium();
			}
		}

		protected override void EnsureConstructDefaults()
		{
			base.EnsureConstructDefaults();

			Attackers = new Dictionary<PlayerMobile, int>();
			Defenders = new Dictionary<PlayerMobile, int>();
		}

		public CTFTeam(PvPBattle battle, string name = "Team", int minCapacity = 0, int maxCapacity = 1, int color = 12)
			: base(battle, name, minCapacity, maxCapacity, color)
		{
			FlagRespawnDelay = TimeSpan.FromSeconds(10.0);

			RespawnOnDeath = true;
			KickOnDeath = false;
		}

		public CTFTeam(PvPBattle battle, GenericReader reader)
			: base(battle, reader)
		{ }

		protected override void OnMicroSync()
		{
			base.OnMicroSync();

			if (Flag == null || Flag.Deleted)
			{
				Flag = null;
				return;
			}

			if (Battle == null || Battle.Deleted || Battle.Hidden || Battle.IsInternal)
			{
				Flag.Delete();
				Flag = null;
				return;
			}

			Flag.InvalidateCarrier();

			if (Flag.IsAtPodium())
			{
				return;
			}

			Flag.UpdateDamageIncrease();
			Flag.CheckReset();
		}

		public override void Reset()
		{
			base.Reset();

			if (Flag != null)
			{
				Flag.Delete();
				Flag = null;
			}

			if (Attackers != null)
			{
				Attackers.Clear();
			}
			else
			{
				Attackers = new Dictionary<PlayerMobile, int>();
			}

			if (Defenders != null)
			{
				Defenders.Clear();
			}
			else
			{
				Defenders = new Dictionary<PlayerMobile, int>();
			}
		}

		public virtual void InvalidateFlagPodium()
		{
			if (Deserializing || SpawnPoint == Point3D.Zero || Battle.Options.Locations.Map == null ||
				Battle.Options.Locations.Map == Map.Internal)
			{
				return;
			}

			if (FlagPodium == null || FlagPodium.Deleted)
			{
				FlagPodium = new CTFPodium(this);
			}
			else
			{
				FlagPodium.Hue = Color;
				FlagPodium.Name = Name;
			}

			FlagPodium.MoveToWorld(SpawnPoint, Battle.Options.Locations.Map);
		}

		public virtual void InvalidateSolidHueOverride()
		{
			if (!Deserializing)
			{
				ForEachMember(InvalidateSolidHueOverride);
			}
		}

		public virtual void InvalidateSolidHueOverride(PlayerMobile pm)
		{
			if (!Deserializing && pm != null && !pm.Deleted && IsMember(pm) && pm.InRegion(Battle.BattleRegion))
			{
				if ((Battle.IsPreparing || Battle.IsRunning) && _SolidHueOverride)
				{
					pm.SolidHueOverride = Color;
				}
				else
				{
					pm.SolidHueOverride = -1;
				}
			}
		}

		public override void OnMemberAdded(PlayerMobile pm)
		{
			base.OnMemberAdded(pm);

			InvalidateSolidHueOverride(pm);
		}

		public override void OnMemberRemoved(PlayerMobile pm)
		{
			base.OnMemberRemoved(pm);

			if (Battle != null && !Battle.Deleted)
			{
				Battle.ForEachTeam<CTFTeam>(
					t =>
					{
						if (t.Flag != null && !t.Flag.Deleted && t.Flag.Carrier == pm)
						{
							t.Flag.Carrier = null;
						}
					});
			}

			InvalidateSolidHueOverride(pm);
		}

		public override void OnBattleOpened()
		{
			base.OnBattleOpened();

			InvalidateFlagPodium();
			InvalidateSolidHueOverride();
		}

		public override void OnBattlePreparing()
		{
			base.OnBattlePreparing();

			InvalidateFlagPodium();
			InvalidateSolidHueOverride();
		}

		public override void OnBattleStarted()
		{
			base.OnBattleStarted();

			InvalidateFlagPodium();
			InvalidateSolidHueOverride();

			if (Flag == null || Flag.Deleted)
			{
				RespawnFlag();
			}
			else
			{
				Flag.Reset();
			}
		}

		public override void OnBattleEnded()
		{
			base.OnBattleEnded();

			InvalidateFlagPodium();
			InvalidateSolidHueOverride();
		}

		public override void OnMemberDeath(PlayerMobile pm)
		{
			if (Battle != null && !Battle.Deleted)
			{
				Battle.ForEachTeam<CTFTeam>(
					t =>
					{
						if (t.Flag != null && !t.Flag.Deleted && t.Flag.Carrier == pm)
						{
							t.Flag.Carrier = null;
						}
					});
			}

			InvalidateSolidHueOverride(pm);

			base.OnMemberDeath(pm);
		}

		public virtual void SpawnFlag()
		{
			if (!Battle.IsRunning)
			{
				if (Flag != null && !Flag.Deleted)
				{
					Flag.Delete();
				}

				Flag = null;
				return;
			}

			if (Flag == null || Flag.Deleted)
			{
				Flag = new CTFFlag(this);
			}

			Flag.Carrier = null;
			Flag.Reset();
		}

		public virtual void RespawnFlag()
		{
			Timer.DelayCall(FlagRespawnDelay, SpawnFlag);
		}

		public virtual void OnFlagDropped(PlayerMobile attacker, CTFTeam enemyTeam)
		{
			if (attacker == null || enemyTeam == null || Flag == null || Flag.Deleted || CTFBattle == null || CTFBattle.Deleted)
			{
				return;
			}

			Broadcast("[{0}]: {1} has dropped your flag!", enemyTeam.Name, attacker.RawName);

			CTFBattle.OnFlagDropped(Flag, attacker, enemyTeam);
		}

		public virtual void OnFlagCaptured(PlayerMobile attacker, CTFTeam enemyTeam)
		{
			if (attacker == null || enemyTeam == null || Flag == null || Flag.Deleted || CTFBattle == null || CTFBattle.Deleted)
			{
				return;
			}

			if (enemyTeam.Attackers.ContainsKey(attacker))
			{
				enemyTeam.Attackers[attacker]++;
			}
			else
			{
				enemyTeam.Attackers.Add(attacker, 1);
			}

			Broadcast("[{0}]: {1} has captured your flag!", enemyTeam.Name, attacker.RawName);

			CTFBattle.OnFlagCaptured(Flag, attacker, enemyTeam);

			RespawnFlag();
		}

		public virtual void OnFlagStolen(PlayerMobile attacker, CTFTeam enemyTeam)
		{
			if (attacker == null || enemyTeam == null || Flag == null || Flag.Deleted || CTFBattle == null || CTFBattle.Deleted)
			{
				return;
			}

			Broadcast("[{0}]: {1} has stolen your flag!", enemyTeam.Name, attacker.RawName);

			CTFBattle.OnFlagStolen(Flag, attacker, enemyTeam);
		}

		public virtual void OnFlagReturned(PlayerMobile defender)
		{
			if (defender == null || Flag == null || Flag.Deleted || CTFBattle == null || CTFBattle.Deleted)
			{
				return;
			}

			if (Defenders.ContainsKey(defender))
			{
				Defenders[defender]++;
			}
			else
			{
				Defenders.Add(defender, 1);
			}

			Broadcast("[{0}]: {1} has returned your flag!", Name, defender.RawName);

			CTFBattle.OnFlagReturned(Flag, defender);
		}

		public long GetTotalFlagsCaptured()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o["Flags Captured"]);
		}

		public long GetTotalFlagsDropped()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o["Flags Dropped"]);
		}

		public long GetTotalFlagsStolen()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o["Flags Stolen"]);
		}

		public long GetTotalFlagsReturned()
		{
			return Statistics.Values.Aggregate(0L, (v, o) => v + o["Flags Returned"]);
		}

		protected override void OnDeleted()
		{
			if (Flag != null)
			{
				Flag.Delete();
				Flag = null;
			}

			if (FlagPodium != null)
			{
				FlagPodium.Delete();
				FlagPodium = null;
			}

			base.OnDeleted();
		}

		public override void GetHtmlStatistics(Mobile viewer, StringBuilder html)
		{
			base.GetHtmlStatistics(viewer, html);

			html.AppendLine("----------");
			html.AppendLine("Flags Captured: {0:#,0}", GetTotalFlagsCaptured());
			html.AppendLine("Flags Dropped: {0:#,0}", GetTotalFlagsDropped());
			html.AppendLine("Flags Stolen: {0:#,0}", GetTotalFlagsStolen());
			html.AppendLine("Flags Returned: {0:#,0}", GetTotalFlagsReturned());

			html.AppendLine();

			html.Append(String.Empty.WrapUOHtmlColor(System.Drawing.Color.LawnGreen, false));
			html.AppendLine("Defenders:");
			html.AppendLine();

			string t;

			var i = 0;

			foreach (var o in Defenders.OrderBy(kv => kv.Value))
			{
				t = String.Format("{0:#,0}: {1} ({2:#,0} Returns)", ++i, o.Key.RawName, o.Value);
				t = t.WrapUOHtmlColor(viewer.GetNotorietyColor(o.Key), SuperGump.DefaultHtmlColor);

				html.AppendLine(t);
			}

			html.AppendLine();

			html.Append(String.Empty.WrapUOHtmlColor(System.Drawing.Color.OrangeRed, false));
			html.AppendLine("Attackers:");
			html.AppendLine();

			i = 0;

			foreach (var o in Attackers.OrderBy(kv => kv.Value))
			{
				t = String.Format("{0:#,0}: {1} ({2:#,0} Captures)", ++i, o.Key.RawName, o.Value);
				t = t.WrapUOHtmlColor(viewer.GetNotorietyColor(o.Key), SuperGump.DefaultHtmlColor);

				html.AppendLine(t);
			}

			html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Flag);
					writer.Write(FlagPodium);
					writer.Write(-1); // Caps

					writer.Write(FlagRespawnDelay);
					writer.Write(SolidHueOverride);

					writer.WriteDictionary(
						Attackers,
						(w, m, c) =>
						{
							w.Write(m);
							w.Write(c);
						});

					writer.WriteDictionary(
						Defenders,
						(w, m, c) =>
						{
							w.Write(m);
							w.Write(c);
						});
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
					Flag = reader.ReadItem<CTFFlag>();

					if (Flag != null)
					{
						Flag.Team = this;
					}

					FlagPodium = reader.ReadItem<CTFPodium>();

					if (FlagPodium != null)
					{
						FlagPodium.Team = this;
					}

					reader.ReadInt(); // Caps

					FlagRespawnDelay = reader.ReadTimeSpan();
					SolidHueOverride = reader.ReadBool();

					reader.ReadDictionary(
						r => new KeyValuePair<PlayerMobile, int>(r.ReadMobile<PlayerMobile>(), r.ReadInt()),
						Attackers);

					reader.ReadDictionary(
						r => new KeyValuePair<PlayerMobile, int>(r.ReadMobile<PlayerMobile>(), r.ReadInt()),
						Defenders);
				}
				break;
			}
		}
	}
}