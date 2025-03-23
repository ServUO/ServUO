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

using VitaNex.FX;
using VitaNex.Geometry;
using VitaNex.Network;
#endregion

namespace VitaNex.Items
{
	public static class Fireworks
	{
		public static void DoStarsEffect(
			this FireworkStars fx,
			IPoint3D p,
			Map map,
			int radius,
			int sound,
			int[] stars,
			int[] hues)
		{
			if (fx == FireworkStars.None || stars == null || stars.Length == 0)
			{
				return;
			}

			radius = Math.Max(0, Math.Min(10, radius));

			if (hues == null || hues.Length == 0)
			{
				hues = new int[9];

				hues.SetAll(i => Utility.RandomBrightHue());
			}

			switch (fx)
			{
				case FireworkStars.Peony:
				{
					var shape = new Sphere3D(p, radius, true);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(4, 6),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
				case FireworkStars.Chrysanthemum:
				{
					var shape = new Disc3D(p, radius, false);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(4, 6),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
				case FireworkStars.Dahlia:
				{
					var shape = new Cylinder3D(p, radius, true, false);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(6, 8),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
				case FireworkStars.Willow:
				{
					var shape = new Disc3D(p, radius, true);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(6, 8),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								if (Utility.RandomDouble() < 0.66)
								{
									return;
								}

								var zL = b.Z;
								var zR = b.GetWorldTop(map).Z;

								if (zL <= zR || zL < p.Z)
								{
									return;
								}

								var zDiff = zL - zR;

								if (zDiff < 30)
								{
									return;
								}

								var t = b.Clone3D(0, 0, -(zDiff / 2));

								new MovingEffectInfo(b, t, map, e.EffectID, e.Hue, Math.Max(1, e.Speed / 2), e.Render).Send();

								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
				case FireworkStars.BloomFlower:
				{
					var shape = new Ring3D(p, radius);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(6, 8),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								if (Utility.RandomDouble() < 0.66)
								{
									return;
								}

								var t = b.Clone3D(Utility.RandomMinMax(-3, 3), Utility.RandomMinMax(-3, 3), Utility.RandomMinMax(-10, 10));

								new MovingEffectInfo(b, t, map, e.EffectID, e.Hue, e.Speed, e.Render).Send();

								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
				case FireworkStars.Ring:
				{
					var shape = new Ring3D(p, radius);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(6, 8),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								var zL = b.Z;
								var zR = b.GetWorldTop(map).Z;

								if (zL <= zR || zL < p.Z)
								{
									return;
								}

								var zDiff = zL - zR;

								if (zDiff < 30)
								{
									return;
								}

								var t = b.Clone3D(0, 0, -(zDiff / 2));

								new MovingEffectInfo(b, t, map, e.EffectID, e.Hue, 2, e.Render).Send();

								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
				case FireworkStars.Crossette:
				{
					var shape = new Plane3D(p, radius, true);

					foreach (var b in shape)
					{
						new MovingEffectInfo(
							p,
							b,
							map,
							stars.GetRandom(),
							hues.GetRandom(),
							Utility.RandomMinMax(6, 8),
							EffectRender.LightenMore).MovingImpact(
							e =>
							{
								if (Utility.RandomDouble() < 0.33)
								{
									return;
								}

								var t = b.Clone3D(Utility.RandomMinMax(-5, 5), Utility.RandomMinMax(-5, 5));

								new MovingEffectInfo(b, t, map, e.EffectID, e.Hue, e.Speed, e.Render).Send();

								if (sound > 0 && Utility.RandomDouble() <= 0.25)
								{
									Effects.PlaySound(b, map, sound);
								}
							});
					}

					shape.Clear();
				}
				break;
			}
		}
	}
}