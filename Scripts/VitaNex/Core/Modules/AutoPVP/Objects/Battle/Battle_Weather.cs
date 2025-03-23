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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Linq;
using System.Threading.Tasks;

using Server;
using Server.Movement;

using VitaNex.FX;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public abstract partial class PvPBattle
	{
		private static readonly PvPBattleWeatherDirection[] _WeatherDirections = ((PvPBattleWeatherDirection)0)
			.GetValues<PvPBattleWeatherDirection>()
			.Not(d => d == PvPBattleWeatherDirection.None || d == PvPBattleWeatherDirection.Random)
			.ToArray();

		public virtual bool CanUseWeather()
		{
			if (Options.Locations.BattleBounds == null || Options.Locations.BattleBounds.Count == 0 ||
				Options.Locations.Map == Map.Internal)
			{
				return false;
			}

			if (Options.Weather.Density <= 0 || Options.Weather.EffectID <= 0 || Options.Weather.EffectSpeed <= 0)
			{
				return false;
			}

			if (Options.Weather.Force)
			{
				return true;
			}

			if (!Options.Weather.Enabled || State == PvPBattleState.Internal)
			{
				return false;
			}

#if ServUOX
			if (BattleRegion != null && BattleRegion.MobileCount == 0)
#else
			if (BattleRegion != null && BattleRegion.GetMobileCount() == 0)
#endif
			{
				return false;
			}

			if (!Options.SuddenDeath.Enabled || !Options.SuddenDeath.Active)
			{
				return false;
			}

			if (DateTime.UtcNow - Options.SuddenDeath.StartedWhen < Options.SuddenDeath.Delay)
			{
				return false;
			}

			return true;
		}

		public virtual Point3D GetWeatherStartPoint(Point3D loc, PvPBattleWeatherDirection direction)
		{
			if (!Options.Weather.Enabled || Options.Locations.Map == null || Options.Locations.Map == Map.Internal ||
				loc == Point3D.Zero)
			{
				return loc;
			}

			switch (direction)
			{
				case PvPBattleWeatherDirection.None:
					return loc.Clone3D(0, 0, 80);
				case PvPBattleWeatherDirection.Random:
					direction = _WeatherDirections.GetRandom(PvPBattleWeatherDirection.None);
					break;
			}

			int x = 0, y = 0;

			Movement.Offset((Direction)direction, ref x, ref y);

			var rand = Utility.RandomMinMax(4, 8);

			return loc.Clone3D(x * rand, y * rand, 80);
		}

		protected virtual void WeatherCycle()
		{
			if (!CanUseWeather())
			{
				return;
			}

			Point3D start, end;

			Parallel.ForEach(
				Options.Locations.BattleBounds,
				b =>
				{
					end = b.Start.Clone2D(Utility.Random(b.Width), Utility.Random(b.Height)).GetWorldTop(Options.Locations.Map);
					start = GetWeatherStartPoint(end, Options.Weather.Direction);

					new MovingEffectInfo(
						start,
						end,
						Options.Locations.Map,
						Options.Weather.EffectID,
						Options.Weather.EffectHue,
						Options.Weather.EffectSpeed,
						Options.Weather.EffectRender,
						TimeSpan.FromMilliseconds(Utility.Random(500))).MovingImpact(WeatherImpactHandler);
				});
		}

		protected virtual void WeatherImpactHandler(MovingEffectInfo info)
		{
			if (info == null || Deleted || !Options.Weather.Impacts || Options.Weather.ImpactEffectID <= 0 ||
				Options.Weather.ImpactEffectSpeed <= 0 || Options.Weather.ImpactEffectDuration <= 0)
			{
				return;
			}

			OnWeatherImpact(info);

			if (Options.Weather.ImpactEffectID <= 0)
			{
				return;
			}

			var effect = new EffectInfo(
				info.Target,
				info.Map,
				Options.Weather.ImpactEffectID,
				Options.Weather.ImpactEffectHue,
				Options.Weather.ImpactEffectSpeed,
				Options.Weather.ImpactEffectDuration,
				Options.Weather.ImpactEffectRender);

			effect.Send();

			OnWeatherImpact(effect);
		}

		protected virtual void OnWeatherImpact(MovingEffectInfo info)
		{ }

		protected virtual void OnWeatherImpact(EffectInfo info)
		{
			if (info == null || Deleted || !Options.Weather.Impacts)
			{
				return;
			}

			if (Options.Weather.ImpactEffectSound > 0)
			{
				Effects.PlaySound(info.Source, info.Map, Options.Weather.ImpactEffectSound);
			}

			if (!Options.SuddenDeath.Enabled || !Options.SuddenDeath.Active || !Options.SuddenDeath.Damages)
			{
				return;
			}

			foreach (var pm in info.Source.GetPlayersInRange(info.Map, Options.SuddenDeath.DamageRange))
			{
				Options.SuddenDeath.Damage(pm);
			}
		}
	}
}