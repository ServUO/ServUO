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

using VitaNex.FX;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class CTFFlag : Item
	{
		private static readonly TimeSpan SplitSecond = TimeSpan.FromMilliseconds(250.0);

		private static readonly TimeSpan HalfSecond = TimeSpan.FromSeconds(0.5);
		private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1.0);
		private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5.0);
		private static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10.0);
		private static readonly TimeSpan TwentySeconds = TimeSpan.FromSeconds(20.0);

		private DateTime _NextMultiUpdate = DateTime.UtcNow;
		private DateTime _NextFlagReturn = DateTime.UtcNow;

		[CommandProperty(AutoPvP.Access)]
		public virtual CTFTeam Team { get; set; }

		private PlayerMobile _Carrier;

		[CommandProperty(AutoPvP.Access)]
		public virtual PlayerMobile Carrier
		{
			get => _Carrier;
			set
			{
				if (_Carrier == null || _Carrier == value)
				{
					return;
				}

				if (value == null)
				{
					Drop(_Carrier);
				}
				else
				{
					Drop(_Carrier);
					Steal(value);
				}
			}
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual double DamageInc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual DateTime NextAssault { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual DateTime NextEffect { get; set; }

		public override bool HandlesOnMovement => RootParent == null;
		public override bool Nontransferable => RootParent != null;
		public override bool IsVirtualItem => RootParent != null;

		public CTFFlag(CTFTeam team)
			: base(8351)
		{
			Team = team;

			Name = Team.Name;
			Hue = Team.Color;
			Weight = 10.1;
			Movable = false;
			NextAssault = NextEffect = DateTime.UtcNow;
		}

		public CTFFlag(Serial serial)
			: base(serial)
		{ }

		public CTFTeam GetTeam(PlayerMobile pm)
		{
			return Team.Battle.FindTeam<CTFTeam>(pm);
		}

		public void Reset()
		{
			if (Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted)
			{
				Delete();
				return;
			}

			if (Team.FlagPodium != null && !Team.FlagPodium.Deleted)
			{
				MoveToWorld(Team.FlagPodium.Location.Clone3D(0, 0, Team.FlagPodium.ItemData.Height + 5), Team.FlagPodium.Map);
				DamageInc = 0;
			}
			else
			{
				MoveToWorld(Team.HomeBase, Team.Battle.Options.Locations.Map);
				DamageInc = 0;
			}

			NextAssault = DateTime.UtcNow;
		}

		public void Drop(PlayerMobile attacker)
		{
			if (Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted)
			{
				Delete();
				return;
			}

			if (attacker == null || attacker.Deleted || attacker != _Carrier)
			{
				return;
			}

			var enemy = GetTeam(attacker);

			if (enemy != null)
			{
				attacker.SolidHueOverride = enemy.Color;
			}

			NextAssault = DateTime.UtcNow + FiveSeconds;

			_Carrier = null;

			_NextFlagReturn = DateTime.UtcNow + TwentySeconds;

			MoveToWorld(attacker.Location, attacker.Map);

			Team.OnFlagDropped(attacker, enemy);

			InvalidateCarrier();
		}

		public void Steal(PlayerMobile attacker)
		{
			if (Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted)
			{
				Delete();
				return;
			}

			if (attacker == null || attacker.Deleted || attacker == _Carrier || attacker.Backpack == null || attacker.Backpack.Deleted)
			{
				return;
			}

			if (NextAssault > DateTime.UtcNow)
			{
				attacker.SendMessage(54, "This flag cannot be picked up for another {0} seconds.", (NextAssault - DateTime.UtcNow).TotalSeconds);
				return;
			}

			if (attacker.HasItem<CTFFlag>())
			{
				attacker.SendMessage("You may only carry one flag at any given time!");
				return;
			}

			if (!attacker.Backpack.TryDropItem(attacker, this, true))
			{
				return;
			}

			var enemy = GetTeam(attacker);

			attacker.SolidHueOverride = 2498;

			_Carrier = attacker;

			Team.OnFlagStolen(attacker, enemy);

			InvalidateCarrier();
		}

		public void Capture(PlayerMobile attacker)
		{
			if (Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted)
			{
				Delete();
				return;
			}

			if (attacker == null || attacker.Deleted)
			{
				return;
			}

			var enemy = GetTeam(attacker);

			if (enemy != null)
			{
				attacker.SolidHueOverride = enemy.Color;
			}

			Team.OnFlagCaptured(attacker, enemy);

			InvalidateCarrier();

			Delete();
		}

		public void Return(PlayerMobile defender)
		{
			if (Team == null || Team.Deleted)
			{
				Delete();
				return;
			}

			if (defender == null || defender.Deleted)
			{
				return;
			}

			if (Team.FlagPodium != null && !Team.FlagPodium.Deleted)
			{
				MoveToWorld(Team.FlagPodium.Location.Clone3D(0, 0, Team.FlagPodium.ItemData.Height + 5), Team.FlagPodium.Map);
			}
			else
			{
				MoveToWorld(Team.HomeBase, Team.Battle.Options.Locations.Map);
			}

			NextAssault = DateTime.UtcNow;

			DamageInc = 0;

			Team.OnFlagReturned(defender);

			InvalidateCarrier();
		}

		public bool IsAtPodium()
		{
			return Carrier == null && (this.InRange2D(Team.FlagPodium, 0) || this.InRange2D(Team.HomeBase, 0));
		}

		public void UpdateDamageIncrease()
		{
			if (DateTime.UtcNow < _NextMultiUpdate)
			{
				return;
			}

			if (IsAtPodium())
			{
				DamageInc = 0;
			}
			else if (Carrier != null && DamageInc < Team.CTFBattle.FlagDamageIncMax)
			{
				DamageInc += Team.CTFBattle.FlagDamageInc;
				_NextMultiUpdate = DateTime.UtcNow + TenSeconds;
			}
		}

		public void CheckReset()
		{
			if (DateTime.UtcNow < _NextFlagReturn || Carrier != null || IsAtPodium())
			{
				return;
			}

			Team.CTFBattle.OnFlagTimeout(this);

			Reset();
		}

#if NEWPARENT
		public override void OnParentDeleted(IEntity parent)
#else
		public override void OnParentDeleted(object parent)
#endif
		{
			Drop(parent as PlayerMobile);

			base.OnParentDeleted(parent);
		}

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			Drop(parent as PlayerMobile);

			return base.OnParentDeath(parent);
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted ||
				!this.CheckDoubleClick(m, true, false, 1) || !(m is PlayerMobile))
			{
				return;
			}

			var pm = (PlayerMobile)m;
			var battle = Team.CTFBattle;

			if (battle == null || !battle.IsParticipant(pm))
			{
				pm.SendMessage("You must be a participant to perform that action.");
				return;
			}

			if (Team.IsMember(pm))
			{
				if (Team.FlagPodium != null && !Team.FlagPodium.Deleted)
				{
					if (!this.InRange3D(Team.FlagPodium, 1, -10, 10))
					{
						Return(pm);
					}
				}
				else if (!this.InRange3D(Team.HomeBase, 1, -10, 10))
				{
					Return(pm);
				}
			}
			else if (Carrier != pm)
			{
				Steal(pm);
			}
			else
			{
				Drop(pm);
			}
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			if (Deleted || Team == null || Team.Deleted || Team.Battle == null || Team.Battle.Deleted || !(m is PlayerMobile) ||
				!this.CheckDoubleClick(m, false, false, 1))
			{
				return;
			}

			var pm = (PlayerMobile)m;
			var battle = Team.CTFBattle;

			if (battle == null || !battle.IsParticipant(pm))
			{
				return;
			}

			if (Team.IsMember(pm))
			{
				if (Team.FlagPodium != null && !Team.FlagPodium.Deleted)
				{
					if (!this.InRange3D(Team.FlagPodium, 1, -10, 10))
					{
						Return(pm);
					}
				}
				else if (!this.InRange3D(Team.HomeBase, 1, -10, 10))
				{
					Return(pm);
				}
			}
			else if (Carrier != pm)
			{
				Steal(pm);
			}
		}

		public void InvalidateCarrier()
		{
			if (Deleted)
			{
				_Carrier = null;
				return;
			}

			if (Team == null || Team.Deleted)
			{
				Delete();
				return;
			}

			InvalidateCarryEffect();
		}

		public void InvalidateCarryEffect()
		{
			if (Deleted || Carrier == null || Carrier.Deleted || !IsChildOf(Carrier.Backpack))
			{
				_Carrier = null;
				return;
			}

			if (DateTime.UtcNow < NextEffect)
			{
				return;
			}

			new EffectInfo(Carrier.Clone3D(0, 0, 22), Carrier.Map, ItemID, Team.Color).Send();

			NextEffect = DateTime.UtcNow + SplitSecond;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_Carrier);
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
					_Carrier = reader.ReadMobile<PlayerMobile>();
					break;
			}
		}
	}
}