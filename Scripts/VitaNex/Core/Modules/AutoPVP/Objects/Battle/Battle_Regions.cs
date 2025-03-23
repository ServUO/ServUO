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
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		private PvPBattleRegion _BattleRegion;
		private PvPSpectateRegion _SpectateRegion;

		private bool _FloorItemDelete = true;
		private int _LightLevel = 100;

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleRegion BattleRegion
		{
			get => _BattleRegion;
			set
			{
				if (_BattleRegion == value)
				{
					return;
				}

				_BattleRegion = value;

				if (!Deserializing)
				{
					InvalidateBattleRegion();
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPSpectateRegion SpectateRegion
		{
			get => _SpectateRegion;
			set
			{
				if (_SpectateRegion == value)
				{
					return;
				}

				_SpectateRegion = value;

				if (!Deserializing)
				{
					InvalidateSpectateRegion();
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual bool FloorItemDelete
		{
			get => _BattleRegion == null ? _FloorItemDelete : (_FloorItemDelete = _BattleRegion.FloorItemDelete);
			set => _FloorItemDelete = _BattleRegion == null ? value : (_BattleRegion.FloorItemDelete = value);
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual int LightLevel { get => _LightLevel; set => _LightLevel = Math.Max(0, Math.Min(100, value)); }

		[CommandProperty(AutoPvP.Access)]
		public virtual string BattleRegionName => String.Format("{0} ({1})", Name, Serial);

		[CommandProperty(AutoPvP.Access)]
		public virtual string SpectateRegionName => String.Format("{0} (Safe) ({1})", Name, Serial);

		public virtual void InvalidateRegions()
		{
			if (Deserializing)
			{
				return;
			}

			InvalidateBattleRegion();
			InvalidateSpectateRegion();
		}

		public virtual void InvalidateBattleRegion()
		{
			if (Deserializing)
			{
				return;
			}

			if (_BattleRegion != null)
			{
				if (_BattleRegion.Map == Map &&
					_BattleRegion.Area.GetBoundsHashCode() == Options.Locations.BattleBounds.GetBoundsHashCode())
				{
					return;
				}

				_BattleRegion.Unregister();
			}

			if (Options.Locations.BattleFixedPoint == Point3D.Zero)
			{
				_BattleRegion = null;
				return;
			}

			_BattleRegion = _BattleRegion != null ? _BattleRegion.Clone(this) : RegionExtUtility.Create<PvPBattleRegion>(this);

			if (_BattleRegion == null)
			{
				return;
			}

			_BattleRegion.GoLocation = Options.Locations.BattleFixedPoint;
			_BattleRegion.Register();
		}

		public virtual void InvalidateSpectateRegion()
		{
			if (Deserializing)
			{
				return;
			}

			if (_SpectateRegion != null)
			{
				if (_SpectateRegion.Map == Map && _SpectateRegion.Area.GetBoundsHashCode() ==
					Options.Locations.SpectateBounds.GetBoundsHashCode())
				{
					return;
				}

				_SpectateRegion.Unregister();
			}

			if (Options.Locations.SpectateFixedPoint == Point3D.Zero)
			{
				_SpectateRegion = null;
				return;
			}

			_SpectateRegion = _SpectateRegion != null
				? _SpectateRegion.Clone(this)
				: RegionExtUtility.Create<PvPSpectateRegion>(this);

			if (_SpectateRegion == null)
			{
				return;
			}

			_SpectateRegion.GoLocation = Options.Locations.SpectateFixedPoint;
			_SpectateRegion.Register();
		}

		public virtual int NotorietyHandler(Mobile source, Mobile target, out bool handled)
		{
			handled = false;

			if (IsInternal || Hidden)
			{
				return BattleNotoriety.Bubble;
			}

			if (source == null || source.Deleted || target == null || target.Deleted)
			{
				return BattleNotoriety.Bubble;
			}

			handled = true;

			if (NotoUtility.Resolve(source, target, out PlayerMobile x, out PlayerMobile y))
			{
				var noto = NotorietyHandler(x, y, out handled);

				if (handled || noto != BattleNotoriety.Bubble)
				{
					return noto;
				}
			}

			var xrs = source.InRegion(SpectateRegion);
			var xrb = source.InRegion(BattleRegion);

			var yrs = target.InRegion(SpectateRegion);
			var yrb = target.InRegion(BattleRegion);

			if (xrs || xrb || yrs || yrb)
			{
				return Notoriety.Invulnerable;
			}

			handled = false;

			return BattleNotoriety.Bubble;
		}

		protected virtual int NotorietyHandler(PlayerMobile source, PlayerMobile target, out bool handled)
		{
			handled = false;

			if (IsInternal || Hidden)
			{
				return BattleNotoriety.Bubble;
			}

			if (source == null || source.Deleted || target == null || target.Deleted)
			{
				return BattleNotoriety.Bubble;
			}

			if (IsParticipant(source, out var teamA) && IsParticipant(target, out var teamB))
			{
				handled = true;

				if (!IsRunning)
				{
					return Notoriety.Invulnerable;
				}

				if (teamA == teamB)
				{
					if (CanDamageOwnTeam(source, target))
					{
						return Notoriety.Enemy;
					}

					return Notoriety.Ally;
				}

				if (CanDamageEnemyTeam(source, target))
				{
					return Notoriety.Enemy;
				}

				return Notoriety.Invulnerable;
			}

			return BattleNotoriety.Bubble;
		}

		public virtual bool AcceptsSpawnsFrom(Region region)
		{
			return AllowSpawn() && region != null && (region.IsPartOf(BattleRegion) || region.IsPartOf(SpectateRegion));
		}

		public virtual void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			personal = personal != LightLevel ? LightLevel : personal;
		}

		public virtual void OnLocationChanged(Mobile m, Point3D oldLocation)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (m.Region != null && (m.Region.IsPartOf(SpectateRegion) || m.Region.IsPartOf(BattleRegion)))
			{
				CheckDismount(m);
				InvalidateStray(m);
			}

			var pm = m as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (IsParticipant(pm, out var team) && team != null && !team.Deleted)
			{
				team.UpdateActivity(pm);
			}
		}

		public virtual bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
		{
			return true;
		}

		public virtual void OnEnter(PvPRegion region, Mobile m)
		{
			if (region == null || m == null || m.Deleted)
			{
				return;
			}

			if (region.IsPartOf(BattleRegion) && m.InRegion(BattleRegion))
			{
				var pm = m as PlayerMobile;

				if (pm != null)
				{
					pm.SendMessage("You have entered {0}", Name);
				}

				AutoPvP.InvokeEnterBattle(this, region, m);
			}
			else if (region.IsPartOf(SpectateRegion) && m.InRegion(SpectateRegion))
			{
				var pm = m as PlayerMobile;

				if (pm != null)
				{
					pm.SendMessage("You have entered {0} spectator area.", Name);

					if (!IsSpectator(pm))
					{
						AddSpectator(pm, false);
					}
				}

				AutoPvP.InvokeEnterBattle(this, region, m);
			}

			m.Delta(MobileDelta.Noto);
		}

		public virtual void OnExit(PvPRegion region, Mobile m)
		{
			if (region == null || m == null || m.Deleted)
			{
				return;
			}

			if (region.IsPartOf(BattleRegion) && !m.InRegion(BattleRegion))
			{
				var pm = m as PlayerMobile;

				if (pm != null)
				{
					if (IsParticipant(pm))
					{
						Quit(pm, false);
					}

					pm.SendMessage("You have left {0}", Name);
				}

				AutoPvP.InvokeExitBattle(this, region, m);
			}
			else if (region.IsPartOf(SpectateRegion) && !m.InRegion(SpectateRegion))
			{
				var pm = m as PlayerMobile;

				if (pm != null)
				{
					pm.SendMessage("You have left {0} spectator area", Name);

					if (IsSpectator(pm))
					{
						RemoveSpectator(pm, false);
					}
				}

				AutoPvP.InvokeExitBattle(this, region, m);
			}

			m.Delta(MobileDelta.Noto);
		}

		public bool AllowBeneficial(Mobile m, Mobile target, out bool handled)
		{
			if (CheckAllowBeneficial(m, target, out handled))
			{
				if (handled)
				{
					OnAllowBeneficialAccept(m, target);
				}

				return true;
			}

			if (handled)
			{
				OnAllowBeneficialDeny(m, target);
			}

			return false;
		}

		public virtual bool CheckAllowBeneficial(Mobile m, Mobile target, out bool handled)
		{
			handled = false;

			if (m == null || m.Deleted || target == null || target.Deleted)
			{
				return false;
			}

			if (Deleted || State == PvPBattleState.Internal || Hidden)
			{
				return false;
			}

			handled = true;

			if (NotoUtility.Resolve(m, target, out PlayerMobile x, out PlayerMobile y))
			{
				if (IsParticipant(x, out var teamA) && IsParticipant(y, out var teamB))
				{
					if (State != PvPBattleState.Running)
					{
						return true;
					}

					if (!Options.Rules.AllowBeneficial)
					{
						return false;
					}

					if (teamA == teamB)
					{
						if (!CanHealOwnTeam(x, y))
						{
							return false;
						}
					}
					else if (!CanHealEnemyTeam(x, y))
					{
						return false;
					}
				}
			}

			var xrs = m.InRegion(SpectateRegion);
			var xrb = m.InRegion(BattleRegion);

			var yrs = target.InRegion(SpectateRegion);
			var yrb = target.InRegion(BattleRegion);

			if (xrs || xrb || yrs || yrb)
			{
				if (xrs && yrs)
				{
					return true;
				}

				if (xrb && yrb)
				{
					return true;
				}

				return false;
			}

			handled = false;

			return false;
		}

		protected virtual void OnAllowBeneficialAccept(Mobile m, Mobile target)
		{ }

		protected virtual void OnAllowBeneficialDeny(Mobile m, Mobile target)
		{
			if (m != null && !m.Deleted && target != null && !target.Deleted && m != target)
			{
				m.SendMessage("You can not perform beneficial actions on your target.");
			}
		}

		public bool AllowHarmful(Mobile m, Mobile target, out bool handled)
		{
			if (CheckAllowHarmful(m, target, out handled))
			{
				if (handled)
				{
					OnAllowHarmfulAccept(m, target);
				}

				return true;
			}

			if (handled)
			{
				OnAllowHarmfulDeny(m, target);
			}

			return false;
		}

		public virtual bool CheckAllowHarmful(Mobile m, Mobile target, out bool handled)
		{
			handled = false;

			if (m == null || m.Deleted || target == null || target.Deleted)
			{
				return false;
			}

			if (Deleted || State == PvPBattleState.Internal || Hidden)
			{
				return false;
			}

			handled = true;

			if (NotoUtility.Resolve(m, target, out PlayerMobile x, out PlayerMobile y))
			{
				if (IsParticipant(x, out var teamA) && IsParticipant(y, out var teamB))
				{
					if (!Options.Rules.AllowHarmful)
					{
						return false;
					}

					if (State != PvPBattleState.Running)
					{
						return false;
					}

					if (teamA == teamB)
					{
						if (!CanDamageOwnTeam(x, y))
						{
							return false;
						}
					}
					else if (!CanDamageEnemyTeam(x, y))
					{
						return false;
					}
				}
			}

			var xrs = m.InRegion(SpectateRegion);
			var xrb = m.InRegion(BattleRegion);

			var yrs = target.InRegion(SpectateRegion);
			var yrb = target.InRegion(BattleRegion);

			if (xrs || xrb || yrs || yrb)
			{
				if (xrs && yrs)
				{
					return true;
				}

				if (xrb && yrb)
				{
					return true;
				}

				return false;
			}

			handled = false;

			return false;
		}

		protected virtual void OnAllowHarmfulAccept(Mobile m, Mobile target)
		{ }

		protected virtual void OnAllowHarmfulDeny(Mobile m, Mobile target)
		{
			if (m != null && !m.Deleted && target != null && !target.Deleted && m != target)
			{
				m.SendMessage("You can not perform harmful actions on your target.");
			}
		}
	}
}