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

using Server;
using Server.Mobiles;
using Server.Regions;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		public List<PlayerMobile> Spectators { get; private set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool SpectateAllowed { get; set; }

		public IEnumerable<PlayerMobile> GetSpectators()
		{
			return Spectators;
		}

		public void AddSpectator(PlayerMobile pm, bool teleport)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			if (!CanSpectate(pm))
			{
				OnSpectateReject(pm);
				return;
			}

			Spectators.Add(pm);

			if (teleport)
			{
				RecordBounce(pm);

				Teleport(pm, Options.Locations.SpectateJoin, Options.Locations.Map);
			}

			OnSpectatorAdded(pm);
		}

		public void RemoveSpectator(PlayerMobile pm, bool teleport)
		{
			if (!IsSpectator(pm))
			{
				return;
			}

			Spectators.Remove(pm);

			if (teleport)
			{
				var bounce = BounceInfo.GetValue(pm);

				if (bounce != null && !bounce.InternalOrZero)
				{
					Teleport(pm, bounce, bounce);

					BounceInfo.Remove(pm);
				}
				else
				{
					Teleport(pm, Options.Locations.Eject, Options.Locations.Eject);
				}
			}

			OnSpectatorRemoved(pm);
		}

		public virtual bool CanSpectate(PlayerMobile pm)
		{
			return SpectateAllowed && State != PvPBattleState.Internal && pm != null && !pm.Deleted && pm.Alive &&
				   (pm.Region == null || !pm.Region.IsPartOf<Jail>()) && IsOnline(pm) && !InCombat(pm) && !InOtherBattle(pm);
		}

		public bool IsSpectator(PlayerMobile pm)
		{
			return Spectators.Contains(pm);
		}

		protected virtual void OnSpectatorAdded(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				pm.SendMessage("You have been granted a front-row seat.");
			}
		}

		protected virtual void OnSpectatorRemoved(PlayerMobile pm)
		{ }

		protected virtual void OnSpectateReject(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted && IsSpectator(pm))
			{
				pm.SendMessage("You can not spectate {0} at this time.", Name);
			}
		}
	}
}