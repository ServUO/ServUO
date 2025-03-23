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
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		public void Teleport(Mobile m, Point3D destLoc, Map destMap)
		{
			if (m == null || m.Deleted || destLoc == Point3D.Zero || destMap == Map.Internal)
			{
				return;
			}

			var oldLoc = m.Location;
			var oldMap = m.Map;

			var pm = m as PlayerMobile;

			if (pm != null && !IsOnline(pm))
			{
				pm.LogoutLocation = destLoc;
				pm.LogoutMap = destMap;

				if (!pm.Alive && pm.Corpse != null && !pm.Corpse.Deleted)
				{
					pm.Corpse.MoveToWorld(destLoc, destMap);
				}

				OnTeleported(pm, oldLoc, oldMap);
				return;
			}

			if (!m.Hidden)
			{
				var fx = EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration);

				Effects.SendLocationParticles(fx, 0x3728, 10, 10, 5023);
			}

			m.MoveToWorld(destLoc, destMap);
			m.Send(VNScreenLightFlash.Instance);
			SendSound(Options.Sounds.Teleport);

			if (!m.Hidden)
			{
				var fx = EffectItem.Create(destLoc, destMap, EffectItem.DefaultDuration);

				Effects.SendLocationParticles(fx, 0x3728, 10, 10, 5023);
			}

			if (m.Location != destLoc || m.Map != destMap)
			{
				return;
			}

			if (!m.Alive && m.Corpse != null && !m.Corpse.Deleted)
			{
				m.Corpse.MoveToWorld(m.Location, m.Map);
			}

			OnTeleported(m, oldLoc, oldMap);
		}

		public virtual void TeleportToEjectLocation(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted && !Options.Locations.Eject.Zero && !Options.Locations.Eject.Internal)
			{
				Teleport(pm, Options.Locations.Eject, Options.Locations.Eject);
			}
		}

		public virtual void TeleportToSpectateLocation(PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted && Options.Locations.SpectateJoin != Point3D.Zero && Options.Locations.Map != null &&
				Options.Locations.Map != Map.Internal)
			{
				Teleport(pm, Options.Locations.SpectateJoin, Options.Locations.Map);
			}
		}

		public virtual void TeleportToHomeBase(PvPTeam team, PlayerMobile pm)
		{
			if (pm != null && !pm.Deleted && team != null && !team.Deleted)
			{
				Teleport(pm, team.HomeBase, Options.Locations.Map);
			}
		}

		public virtual void TeleportToSpawnPoint(PvPTeam team, PlayerMobile pm)
		{
			if (pm == null || pm.Deleted || team == null || team.Deleted)
			{
				return;
			}

			var p = team.SpawnPoint;

			if (team.RespawnRangeMin > 0 && team.RespawnRangeMax > 0)
			{
				p = p.GetRandomPoint3D(team.RespawnRangeMin, team.RespawnRangeMax);
			}

			Teleport(pm, p, Options.Locations.Map);
		}

		protected virtual void OnTeleported(Mobile m, Point3D oldLocation, Map oldMap)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			m.UpdateRegion();

			if (m is PlayerMobile)
			{
				OnTeleported((PlayerMobile)m, oldLocation, oldMap);
			}

			m.Delta(MobileDelta.Noto);
			m.ProcessDelta();
		}

		protected virtual void OnTeleported(PlayerMobile pm, Point3D oldLocation, Map oldMap)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			var old = Region.Find(oldLocation, oldMap);

			if (!old.IsPartOf(BattleRegion))
			{
				if (pm.InRegion(BattleRegion))
				{
					Negate(pm);
				}
			}
			else
			{
				if (!pm.InRegion(BattleRegion))
				{
					Negate(pm);
				}
			}
		}

		public virtual bool CheckMounted(Mobile m, bool dismount)
		{
			if (m == null || m.Deleted || !m.Mounted || m.Mount == null)
			{
				return false;
			}

			if (dismount)
			{
				CheckDismount(m);
			}

			return true;
		}

		public virtual bool CheckDismount(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (!DebugMode && m.AccessLevel >= AccessLevel.Counselor)
			{
				return false;
			}

			if (State == PvPBattleState.Internal || Hidden)
			{
				return false;
			}

			if (m.Flying && Options.Rules.CanFly)
			{
				return false;
			}

			if (m.Mounted)
			{
				if (m.Mount is EtherealMount && Options.Rules.CanMountEthereal)
				{
					return false;
				}

				if (Options.Rules.CanMount)
				{
					return false;
				}
			}

			Dismount(m);
			return true;
		}

		public virtual void Dismount(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (m.Flying)
			{
				m.Flying = false;
				OnDismounted(m, null);
			}
			else if (m.Mount != null)
			{
				var mount = m.Mount;
				mount.Rider = null;
				OnDismounted(m, mount);
			}
		}

		public virtual void OnDismounted(Mobile m, IMount mount)
		{
			if (m != null && !m.Deleted)
			{
				m.SendMessage(mount != null ? "You have been dismounted." : "You have been wing clipped.");
			}
		}

		public bool AllowSpawn()
		{
			if (CheckAllowSpawn())
			{
				OnAllowSpawnAccept();
				return true;
			}

			OnAllowSpawnDeny();
			return false;
		}

		public virtual bool CheckAllowSpawn()
		{
			return Options.Rules.AllowSpawn;
		}

		protected virtual void OnAllowSpawnAccept()
		{ }

		protected virtual void OnAllowSpawnDeny()
		{ }

		public bool CanMoveThrough(Mobile m, IEntity e)
		{
			if (CheckCanMoveThrough(m, e))
			{
				OnCanMoveThroughAccept(m, e);
				return true;
			}

			OnCanMoveThroughDeny(m, e);
			return false;
		}

		public virtual bool CheckCanMoveThrough(Mobile m, IEntity e)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (!DebugMode && m.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

#if NEWENTITY
			return e == null || e.Deleted || Options.Rules.CanMoveThrough;
#else
			return e == null || (e is Mobile && ((Mobile)e).Deleted) || (e is Item && ((Item)e).Deleted) ||
				   Options.Rules.CanMoveThrough;
#endif
		}

		protected virtual void OnCanMoveThroughAccept(Mobile m, IEntity e)
		{ }

		protected virtual void OnCanMoveThroughDeny(Mobile m, IEntity e)
		{ }

		public bool AllowHousing(Mobile m, Point3D p)
		{
			if (CheckAllowHousing(m, p))
			{
				OnAllowHousingAccept(m, p);
				return true;
			}

			OnAllowHousingDeny(m, p);
			return false;
		}

		public virtual bool CheckAllowHousing(Mobile m, Point3D p)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (!DebugMode && m.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			return Options.Rules.AllowHousing;
		}

		protected virtual void OnAllowHousingAccept(Mobile m, Point3D p)
		{ }

		protected virtual void OnAllowHousingDeny(Mobile m, Point3D p)
		{
			if (m != null && !m.Deleted)
			{
				m.SendMessage("You can not place structures here at this time.");
			}
		}

		public bool CanUseStuckMenu(Mobile m)
		{
			if (CheckCanUseStuckMenu(m))
			{
				OnCanUseStuckMenuAccept(m);
				return true;
			}

			OnCanUseStuckMenuDeny(m);
			return false;
		}

		public virtual bool CheckCanUseStuckMenu(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (!DebugMode && m.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			return Options.Rules.CanUseStuckMenu;
		}

		protected virtual void OnCanUseStuckMenuAccept(Mobile m)
		{ }

		protected virtual void OnCanUseStuckMenuDeny(Mobile m)
		{
			if (m != null && !m.Deleted)
			{
				m.SendMessage("You can not use the stuck menu at this time.");
			}
		}

		public bool OnBeforeDeath(Mobile m)
		{
			if (CheckDeath(m))
			{
				OnDeathAccept(m);
				return true;
			}

			OnDeathDeny(m);
			return false;
		}

		public virtual bool CheckDeath(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (!DebugMode && m.AccessLevel >= AccessLevel.Counselor)
			{
				return false;
			}

			return Options.Rules.CanDie;
		}

		protected virtual void OnDeathAccept(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			var pm = m as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (IsParticipant(pm, out var team) && team != null && !team.Deleted)
			{
				team.OnMemberDeath(pm);
			}
		}

		protected virtual void OnDeathDeny(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			var pm = m as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (IsParticipant(pm, out var team) && team != null && !team.Deleted)
			{
				team.OnMemberDeath(pm);
			}
		}

		public virtual void OnDeath(Mobile m)
		{ }

		public bool OnSkillUse(Mobile user, int skill)
		{
			if (CheckSkillUse(user, skill))
			{
				OnSkillUseAccept(user, skill);
				return true;
			}

			OnSkillUseDeny(user, skill);
			return false;
		}

		public virtual bool CheckSkillUse(Mobile user, int skill)
		{
			if (user == null || user.Deleted || skill < 0)
			{
				return false;
			}

			if (!DebugMode && user.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			return !Options.Restrictions.Skills.IsRestricted(skill);
		}

		protected virtual void OnSkillUseAccept(Mobile user, int skill)
		{ }

		protected virtual void OnSkillUseDeny(Mobile user, int skill)
		{
			if (user != null && !user.Deleted && skill >= 0)
			{
				user.SendMessage("You can not use that skill at this time.");
			}
		}

		public bool OnBeginSpellCast(Mobile caster, ISpell spell)
		{
			if (CheckSpellCast(caster, spell))
			{
				OnSpellCastAccept(caster, spell);
				return true;
			}

			OnSpellCastDeny(caster, spell);
			return false;
		}

		public virtual bool CheckSpellCast(Mobile caster, ISpell spell)
		{
			if (caster == null || caster.Deleted || spell == null)
			{
				return false;
			}

			if (!DebugMode && caster.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			if (!(spell is Spell))
			{
				return true;
			}

			if (!Options.Rules.CanFly)
			{
				var t = spell.GetType();

				if (t.Name.ContainsAny("FlySpell", "FlightSpell"))
				{
					return false;
				}
			}

			return !Options.Restrictions.Spells.IsRestricted((Spell)spell);
		}

		protected virtual void OnSpellCastAccept(Mobile caster, ISpell spell)
		{ }

		protected virtual void OnSpellCastDeny(Mobile caster, ISpell spell)
		{
			if (caster != null && !caster.Deleted && spell != null)
			{
				caster.SendMessage("You can not use that spell at this time.");
			}
		}

		public virtual bool CheckDamage(Mobile damaged, ref int damage)
		{
			if (damaged == null || damaged.Deleted)
			{
				return false;
			}

			if (!DebugMode && damaged.AccessLevel >= AccessLevel.Counselor)
			{
				return false;
			}

			return Options.Rules.CanBeDamaged;
		}

		public virtual void OnDamage(Mobile damager, Mobile damaged, int damage)
		{
			if (damage <= 0)
			{
				return;
			}

			PlayerMobile pm;
			PvPTeam t;

			if (damager != null && !damager.Deleted && damager is PlayerMobile)
			{
				pm = (PlayerMobile)damager;

				if (IsParticipant(pm, out t))
				{
					var d = damage;

					UpdateStatistics(t, pm, o => o.DamageDone += d);
				}
			}

			if (damaged != null && !damaged.Deleted && damaged is PlayerMobile)
			{
				pm = (PlayerMobile)damaged;

				if (IsParticipant(pm, out t))
				{
					var d = damage;

					UpdateStatistics(t, pm, o => o.DamageTaken += d);
				}
			}
		}

		public virtual bool CheckHeal(Mobile healed, ref int heal)
		{
			if (healed == null || healed.Deleted)
			{
				return false;
			}

			if (!DebugMode && healed.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			return Options.Rules.CanHeal;
		}

		public virtual void OnHeal(Mobile healer, Mobile healed, int heal)
		{
			if (heal <= 0)
			{
				return;
			}

			PlayerMobile pm;
			PvPTeam t;

			if (healer != null && !healer.Deleted && healer is PlayerMobile)
			{
				pm = (PlayerMobile)healer;

				if (IsParticipant(pm, out t))
				{
					var h = heal;

					UpdateStatistics(t, pm, o => o.HealingDone += h);
				}
			}

			if (healed != null && !healed.Deleted && healed is PlayerMobile)
			{
				pm = (PlayerMobile)healed;

				if (IsParticipant(pm, out t))
				{
					var h = heal;

					UpdateStatistics(t, pm, o => o.HealingTaken += h);
				}
			}
		}

		public bool OnResurrect(Mobile m)
		{
			if (CheckResurrect(m))
			{
				OnResurrectAccept(m);
				return true;
			}

			OnResurrectDeny(m);
			return false;
		}

		public virtual bool CheckResurrect(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (!DebugMode && m.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			return Options.Rules.CanResurrect;
		}

		protected virtual void OnResurrectAccept(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			var pm = m as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (IsParticipant(pm, out var team) && team != null && !team.Deleted)
			{
				team.OnMemberResurrected(pm);
			}
		}

		protected virtual void OnResurrectDeny(Mobile m)
		{
			if (m != null && !m.Deleted)
			{
				m.SendMessage("You can not be resurrected at this time.");
			}
		}

		public void OnSpeech(SpeechEventArgs args)
		{
			if (args.Mobile is PlayerMobile)
			{
				var pm = (PlayerMobile)args.Mobile;

				if (HandleSubCommand(pm, args.Speech))
				{
					args.Handled = true;
					args.Blocked = true;
					return;
				}
			}

			if (CheckSpeech(args))
			{
				OnSpeechAccept(args);
				return;
			}

			args.Handled = true;
			args.Blocked = true;

			OnSpeechDeny(args);
		}

		public virtual bool CheckSpeech(SpeechEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
			{
				return false;
			}

			if (!DebugMode && e.Mobile.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			return Options.Rules.AllowSpeech;
		}

		protected virtual void OnSpeechAccept(SpeechEventArgs args)
		{ }

		protected virtual void OnSpeechDeny(SpeechEventArgs args)
		{
			if (args.Mobile != null && !args.Mobile.Deleted)
			{
				args.Mobile.SendMessage("You can not talk at this time.");
			}
		}

		public virtual void OnSpellCast(Mobile m, ISpell s)
		{ }

		public virtual void OnAggressed(Mobile aggressor, Mobile aggressed, bool criminal)
		{ }

		public virtual void OnBeneficialAction(Mobile helper, Mobile target)
		{ }

		public virtual bool OnCombatantChange(Mobile m, Mobile oldMob, Mobile newMob)
		{
			return m != null && !m.Deleted;
		}

		public virtual void OnCriminalAction(Mobile m, bool message)
		{ }

		public virtual void OnDidHarmful(Mobile harmer, Mobile harmed)
		{ }

		public virtual bool OnDoubleClick(Mobile m, object o)
		{
			if (o is Item)
			{
				return OnDoubleClick(m, (Item)o);
			}

			if (o is Mobile)
			{
				return OnDoubleClick(m, (Mobile)o);
			}

			return m != null && !m.Deleted && o != null;
		}

		public virtual bool OnDoubleClick(Mobile m, Item item)
		{
			return m != null && !m.Deleted && item != null && !item.Deleted && CanUseItem(m, item, true);
		}

		public virtual bool OnDoubleClick(Mobile m, Mobile target)
		{
			if (target is BaseCreature)
			{
				return OnDoubleClick(m, (BaseCreature)target);
			}

			return m != null && !m.Deleted && target != null && !target.Deleted && CanUseMobile(m, target, true);
		}

		public virtual bool OnDoubleClick(Mobile m, BaseCreature target)
		{
			return m != null && !m.Deleted && target != null && !target.Deleted && CanUseMobile(m, target, true);
		}

		public virtual bool OnSingleClick(Mobile m, object o)
		{
			if (o is Item)
			{
				return OnSingleClick(m, (Item)o);
			}

			if (o is Mobile)
			{
				return OnSingleClick(m, (Mobile)o);
			}

			return m != null && !m.Deleted && o != null;
		}

		public virtual bool OnSingleClick(Mobile m, Item item)
		{
			return m != null && !m.Deleted && item != null && !item.Deleted && CanUseItem(m, item, false);
		}

		public virtual bool OnSingleClick(Mobile m, Mobile target)
		{
			if (target is BaseCreature)
			{
				return OnSingleClick(m, (BaseCreature)target);
			}

			return m != null && !m.Deleted && target != null && !target.Deleted && CanUseMobile(m, target, false);
		}

		public virtual bool OnSingleClick(Mobile m, BaseCreature target)
		{
			return m != null && !m.Deleted && target != null && !target.Deleted && CanUseMobile(m, target, false);
		}

		public virtual void OnGotBeneficialAction(Mobile helper, Mobile target)
		{ }

		public virtual void OnGotHarmful(Mobile harmer, Mobile harmed)
		{ }

		public virtual bool OnTarget(Mobile m, Target target, object o)
		{
			return m != null && !m.Deleted && target != null && o != null;
		}
	}
}
