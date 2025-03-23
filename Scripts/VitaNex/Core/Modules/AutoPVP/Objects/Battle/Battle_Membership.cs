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

using Server;
using Server.Mobiles;
using Server.Regions;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual TimeSpan LogoutDelay { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual TimeSpan IdleThreshold { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool IdleKick { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool InviteWhileRunning { get; set; }

		public virtual Dictionary<PlayerMobile, MapPoint> BounceInfo { get; private set; }

		public virtual bool InOtherBattle(PlayerMobile pm)
		{
			if (pm == null)
			{
				return false;
			}

			var battle = AutoPvP.FindBattle(pm);

			return battle != null && battle != this && battle.IsParticipant(pm);
		}

		public bool IsAliveParticipant(PlayerMobile pm)
		{
			if (pm == null)
			{
				return false;
			}

			return IsParticipant(pm, out var team) && !team.IsDead(pm);
		}

		public bool IsDeadParticipant(PlayerMobile pm)
		{
			if (pm == null)
			{
				return false;
			}

			return IsParticipant(pm, out var team) && team.IsDead(pm);
		}

		public bool IsParticipant(PlayerMobile pm)
		{
			if (pm == null)
			{
				return false;
			}

			return IsParticipant(pm, out var team);
		}

		public bool IsParticipant(PlayerMobile pm, out PvPTeam team)
		{
			if (pm == null)
			{
				team = null;

				return false;
			}

			team = FindTeam(pm);

			return team != null;
		}

		public virtual TimeSpan GetLogoutDelay(Mobile m)
		{
			return LogoutDelay;
		}

		public IEnumerable<PlayerMobile> GetParticipants()
		{
			return Teams.Where(t => t != null && !t.Deleted).SelectMany(t => t.Where(p => p != null && !p.Deleted));
		}

		public virtual bool CanSendInvites()
		{
			return !Hidden && (IsPreparing || (IsRunning && InviteWhileRunning)) && CurrentCapacity < MaxCapacity;
		}

		public virtual bool CanSendInvite(PlayerMobile pm)
		{
			return pm != null && !pm.Deleted && pm.Alive && !pm.InRegion<Jail>() && pm.DesignContext == null && IsOnline(pm) &&
				   !InCombat(pm) && IsQueued(pm) && !IsParticipant(pm) && !InOtherBattle(pm) && !AutoPvP.IsDeserter(pm);
		}

		public virtual void SendInvites()
		{
			foreach (var pm in Queue.Keys)
			{
				var invites = SuperGump.GetInstances<PvPInviteGump>(pm);

				if (CanSendInvite(pm))
				{
					var sendNew = invites.All(invite => !invite.IsOpen || invite.Battle != this);

					if (sendNew)
					{
						OnSendInvite(pm, new PvPInviteGump(pm, this));
					}
				}
				else
				{
					foreach (var invite in invites.Where(invite => invite.IsOpen && invite.Battle == this))
					{
						invite.Close();
					}
				}
			}
		}

		protected virtual void OnSendInvite(PlayerMobile pm, PvPInviteGump invite)
		{
			if (pm == null || pm.Deleted || !pm.Alive || invite == null || invite.IsDisposed)
			{
				return;
			}

			invite.Send();

			SendSound(pm, Options.Sounds.InviteSend);
		}

		public void AcceptInvite(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || !pm.Alive)
			{
				return;
			}

			if (CanSendInvites() && CanSendInvite(pm))
			{
				OnInviteAccept(pm);
			}
			else
			{
				OnInviteRejected(pm);
			}
		}

		public void DeclineInvite(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				OnInviteDecline(pm);
			}
		}

		protected virtual void OnInviteAccept(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || !IsOnline(pm) || IsInternal || Hidden)
			{
				return;
			}

			if (!IsQueued(pm))
			{
				pm.SendMessage("You are not queued for {0}", Name);
				return;
			}

			if (IsParticipant(pm))
			{
				pm.SendMessage("You are already participating in {0}", Name);
				return;
			}

			if (InOtherBattle(pm))
			{
				pm.SendMessage("You cannot join {0} while you are in another battle.", Name);
				return;
			}

			if (IsFull)
			{
				pm.SendMessage("The battle is full, you will be sent an invite if someone leaves.");
				return;
			}

			var team = Queue[pm];

			if (team == null || team.Deleted) // Assume AutoAssign is true
			{
				if (Teams.Count == 1)
				{
					team = Teams[0];
				}
				else if (AutoAssign) // Make sure AutoAssign is true
				{
					team = GetAutoAssignTeam(pm);
				}
			}

			if (team == null || team.Deleted) // Fallback to most empty, or random team
			{
				team = GetMostEmptyTeam();

				if (team == null || team.Deleted)
				{
					team = GetRandomTeam();
				}
			}

			if (team == null || team.Deleted)
			{
				pm.SendMessage("The team you've chosen seems to have vanished in the void, sorry about that.");
				Queue.Remove(pm);
				return;
			}

			if (team.IsFull)
			{
				pm.SendMessage("The team you've chosen is full, you will be sent an invite if someone leaves.");
				return;
			}

			Queue.Remove(pm);

			RecordBounce(pm);

			team.AddMember(pm, true);

			SendSound(pm, Options.Sounds.InviteAccept);
		}

		protected virtual void OnInviteDecline(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			pm.SendMessage("You decide not to join {0}", Name);
			SendSound(pm, Options.Sounds.InviteCancel);

			if (IsQueued(pm))
			{
				Dequeue(pm);
			}
		}

		protected virtual void OnInviteRejected(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			pm.SendMessage("You can not join {0} at this time.", Name);
			SendSound(pm, Options.Sounds.InviteCancel);
		}

		public void RecordBounce(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || pm.InRegion<PvPBattleRegion>())
			{
				return;
			}

			var bounce = pm.ToMapPoint();

			if (bounce != null && !bounce.InternalOrZero)
			{
				BounceInfo[pm] = bounce;
			}
		}

		public void Quit(Mobile m, bool teleport)
		{
			var pm = m as PlayerMobile;

			if (pm != null)
			{
				if (IsParticipant(pm, out var team))
				{
					UpdateStatistics(team, pm, o => ++o.Losses);

					if (IsRunning)
					{
						OnDeserted(team, pm);
					}
				}
			}

			Eject(m, teleport);
		}

		public void Eject(Mobile m, bool teleport)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (m is PlayerMobile)
			{
				Eject((PlayerMobile)m, teleport);
			}
			else if (m is BaseCreature)
			{
				Eject((BaseCreature)m, teleport);
			}
		}

		public void Eject(PlayerMobile pm, bool teleport)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			if (IsParticipant(pm, out var team))
			{
				if (IsRunning || IsEnded)
				{
					UpdateStatistics(team, pm, o => ++o.Battles);
				}

				team.RemoveMember(pm, false);
			}
			else if (IsSpectator(pm))
			{
				RemoveSpectator(pm, false);
			}

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
					Teleport(pm, Options.Locations.Eject, Options.Locations.Eject.Map);
				}
			}
		}

		public void Eject(BaseCreature bc, bool teleportOrStable)
		{
			if (bc == null || bc.Deleted)
			{
				return;
			}

			var pet = bc.IsControlled<PlayerMobile>();

			if (!teleportOrStable)
			{
				if (!pet)
				{
					bc.Delete();
				}

				return;
			}

			if (!pet || !bc.Stable())
			{
				Teleport(bc, Options.Locations.Eject, Options.Locations.Eject);
			}
		}

		protected virtual void OnEjected(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (m is PlayerMobile)
			{
				OnEjected((PlayerMobile)m);
			}
			else if (m is BaseCreature)
			{
				OnEjected((BaseCreature)m);
			}
		}

		protected virtual void OnEjected(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted)
			{
				pm.SendMessage("You have been ejected from the battle.");
			}
		}

		protected virtual void OnEjected(BaseCreature bc)
		{ }

		protected virtual void OnDeserted(PvPTeam team, PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			pm.SendMessage(0x22, "You have deserted {0}!", Name);

			AutoPvP.AddDeserter(pm);

			RevokePoints(pm);

			UpdateStatistics(team, pm, o => ++o["Deserted"]);

			WorldBroadcast("{0} has deserted {1}!", pm.RawName, Name);
		}

		public virtual void InvalidateStray(Mobile m)
		{
			if (IsInternal || Hidden || m == null || m.Deleted)
			{
				return;
			}

			if (m is PlayerMobile)
			{
				InvalidateStrayPlayer((PlayerMobile)m);
			}
			else if (m is BaseCreature)
			{
				var bc = (BaseCreature)m;

				if (bc.IsControlled())
				{
					InvalidateStrayPet(bc);
				}
				else
				{
					InvalidateStraySpawn(bc);
				}
			}
		}

		public virtual void InvalidateStrayPlayer(PlayerMobile player)
		{
			if (IsInternal || Hidden || player == null || player.Deleted)
			{
				return;
			}

			if (player.InRegion(SpectateRegion))
			{
				if (IsParticipant(player))
				{
					Eject(player, false);
				}

				if (!IsSpectator(player))
				{
					AddSpectator(player, false);
				}
			}
			else if (player.InRegion(BattleRegion))
			{
				if (IsParticipant(player))
				{
					if (IsSpectator(player))
					{
						RemoveSpectator(player, false);
					}

					Queue.Remove(player);

					if (DebugMode || player.AccessLevel < AccessLevel.Counselor)
					{
						if (player.Flying && !Options.Rules.CanFly)
						{
							player.Flying = false;
						}

						if (player.Mounted)
						{
							var canMount = Options.Rules.CanMount;

							if (player.Mount is EtherealMount)
							{
								canMount = Options.Rules.CanMountEthereal;
							}

							if (!canMount)
							{
								player.Dismount();
							}
						}
					}
				}
				else if (!IsSpectator(player))
				{
					if ((DebugMode || player.AccessLevel < AccessLevel.Counselor) && SpectateAllowed)
					{
						AddSpectator(player, true);
					}
					else if (DebugMode || player.AccessLevel < AccessLevel.Counselor)
					{
						Eject(player, true);
					}
				}
			}
		}

		public virtual void InvalidateStraySpawn(BaseCreature mob)
		{
			if (IsInternal || Hidden || mob == null || mob.Deleted || !mob.InRegion(BattleRegion))
			{
				return;
			}

			if (!AllowSpawn())
			{
				Eject(mob, mob.IsControlled<PlayerMobile>());
			}
		}

		public virtual void InvalidateStrayPet(BaseCreature pet)
		{
			if (IsInternal || Hidden || pet == null || pet.Deleted || !pet.InRegion(BattleRegion))
			{
				return;
			}

			if (!Options.Rules.AllowPets)
			{
				var master = pet.GetMaster<PlayerMobile>();

				if (master == null || DebugMode || master.AccessLevel < AccessLevel.Counselor)
				{
					Eject(pet, master != null);
				}
			}
		}

		public virtual void InvalidateGates()
		{
			InvalidateSpectateGate();
			InvalidateTeamGates();
		}

		public virtual void InvalidateSpectateGate()
		{
			if (!SpectateAllowed || SpectateRegion == null || IsInternal || Options.Locations.SpectateGate.InternalOrZero)
			{
				if (Gate != null)
				{
					Gate.Delete();
					Gate = null;
				}

				return;
			}

			if (Gate == null || Gate.Deleted)
			{
				Gate = new PvPSpectatorGate(this);

				if (!Options.Locations.SpectateGate.MoveToWorld(Gate))
				{
					Gate.MoveToWorld(Options.Locations.SpectateGate, Options.Locations.SpectateGate);
				}
			}

			if (Gate.Battle == null)
			{
				Gate.Battle = this;
			}
		}

		public virtual void InvalidateTeamGates()
		{
			ForEachTeam(t => t.InvalidateGate());
		}
	}
}