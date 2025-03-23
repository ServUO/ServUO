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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using VitaNex;
using VitaNex.Crypto;
using VitaNex.FX;
using VitaNex.Network;
#endregion

namespace Server
{
	public static class RegionExtUtility
	{
		public class RegionSerial : CryptoHashCode
		{
			public override string Value => base.Value.Replace("-", String.Empty);

			public RegionSerial(Region r)
				: base(
					CryptoHashType.MD5,
					r.GetType().FullName + r.Map.Name + r.Name + r.Area.GetBoundsHashCode() + TimeStamp.UtcNow.Stamp +
					Utility.RandomDouble())
			{ }

			public RegionSerial(GenericReader reader)
				: base(reader)
			{ }
		}

		public class PreviewRegion : Region, IEquatable<Region>
		{
			private static ulong _UID;

			public RegionSerial Serial { get; private set; }

			public PollTimer Timer { get; private set; }
			public EffectInfo[] Effects { get; private set; }

			public DateTime Expire { get; set; }
			public int EffectID { get; set; }
			public int EffectHue { get; set; }
			public EffectRender EffectRender { get; set; }

			public PreviewRegion(Region r)
				: this(r.Name, r.Map, r.Area)
			{
				Serial = GetSerial(r);
			}

			public PreviewRegion(string name, Map map, params Rectangle2D[] area)
				: base(name + " " + _UID++, map, null, area)
			{
				Serial = new RegionSerial(this);
				EnsureDefaults();
			}

			public PreviewRegion(string name, Map map, params Rectangle3D[] area)
				: base(name + " " + _UID++, map, null, area)
			{
				Serial = new RegionSerial(this);
				EnsureDefaults();
			}

			public void EnsureDefaults()
			{
				Expire = DateTime.UtcNow.AddSeconds(300.0);

				EffectID = 1801;
				EffectHue = 85;
				EffectRender = EffectRender.ShadowOutline;
			}

			public void Refresh()
			{
				Expire = DateTime.UtcNow.AddSeconds(300.0);

				Register();
			}

			public override void OnRegister()
			{
				base.OnRegister();

				Expire = DateTime.UtcNow.AddSeconds(300.0);

				if (Effects == null)
				{
					var effects = new EffectInfo[Area.Length][,];

					effects.SetAll(i => new EffectInfo[Area[i].Width, Area[i].Height]);

					for (var index = 0; index < Area.Length; index++)
					{
						var b = Area[index];

						var xSpacing = Math.Max(1, Math.Min(16, b.Width / 8));
						var ySpacing = Math.Max(1, Math.Min(16, b.Height / 8));

						var minX = Math.Min(b.Start.X, b.End.X);
						var maxX = Math.Max(b.Start.X, b.End.X);

						var minY = Math.Min(b.Start.Y, b.End.Y);
						var maxY = Math.Max(b.Start.Y, b.End.Y);

						Parallel.For(
							minX,
							maxX,
							x => Parallel.For(
								minY,
								maxY,
								y =>
								{
									if (x != b.Start.X && x != b.End.X - 1 && x % xSpacing != 0 //
										&& y != b.Start.Y && y != b.End.Y - 1 && y % ySpacing != 0)
									{
										return;
									}

									var idxX = x - minX;
									var idxY = y - minY;

									effects[index][idxX, idxY] = new EffectInfo(
										new Point3D(x, y, 0),
										Map,
										EffectID,
										EffectHue,
										1,
										25,
										EffectRender);
								}));
					}

					Effects = effects.SelectMany(list => list.OfType<EffectInfo>()).ToArray();

					foreach (var e in Effects)
					{
						e.SetSource(e.Source.ToPoint3D(e.Source.GetAverageZ(e.Map)));
					}
				}

				if (Timer == null)
				{
					Timer = PollTimer.FromSeconds(
						1.0,
						() =>
						{
							if (DateTime.UtcNow > Expire)
							{
								Unregister();
								return;
							}

							foreach (var e in Effects)
							{
								e.Send();
							}
						},
						() => Registered);
				}
				else
				{
					Timer.Running = true;
				}

				_Previews.Update(Serial, this);
			}

			public override void OnUnregister()
			{
				base.OnUnregister();

				if (Timer != null)
				{
					Timer.Running = false;
					Timer = null;
				}

				if (Effects != null)
				{
					foreach (var e in Effects)
					{
						e.Dispose();
					}

					Effects.SetAll(i => null);
					Effects = null;
				}

				Expire = DateTime.UtcNow;

				_Previews.Remove(Serial);
			}

			public bool Equals(Region other)
			{
				return !ReferenceEquals(other, null) && Serial.Equals(GetSerial(other));
			}
		}

		private static readonly Dictionary<RegionSerial, Region> _Regions;
		private static readonly Dictionary<RegionSerial, PreviewRegion> _Previews;

		static RegionExtUtility()
		{
			_Regions = new Dictionary<RegionSerial, Region>();
			_Previews = new Dictionary<RegionSerial, PreviewRegion>();
		}

		public static RegionSerial GetSerial(this Region r)
		{
			if (r == null)
			{
				return null;
			}

			if (r is PreviewRegion)
			{
				return ((PreviewRegion)r).Serial;
			}

			lock (_Regions)
			{
				if (_Regions.ContainsValue(r))
				{
					return _Regions.GetKey(r);
				}

				var s = new RegionSerial(r);

				_Regions.Update(s, r);

				//Console.WriteLine("Region Serial: ('{0}', '{1}', '{2}') = {3}", r.GetType().Name, r.Map, r.Name, s.ValueHash);

				return s;
			}
		}

		public static PreviewRegion DisplayPreview(
			string name,
			Map map,
			int hue = 85,
			int effect = 1801,
			EffectRender render = EffectRender.SemiTransparent,
			params Rectangle2D[] bounds)
		{
			return DisplayPreview(new PreviewRegion(name, map, bounds), hue, effect, render);
		}

		public static PreviewRegion DisplayPreview(
			string name,
			Map map,
			int hue = 85,
			int effect = 1801,
			EffectRender render = EffectRender.SemiTransparent,
			params Rectangle3D[] bounds)
		{
			return DisplayPreview(new PreviewRegion(name, map, bounds), hue, effect, render);
		}

		public static PreviewRegion DisplayPreview(
			this Region r,
			int hue = 85,
			int effect = 1801,
			EffectRender render = EffectRender.SemiTransparent)
		{
			if (r == null || r.Area == null || r.Area.Length == 0)
			{
				return null;
			}

			if (hue < 0)
			{
				hue = 0;
			}

			if (effect <= 0)
			{
				effect = 1801;
			}

			PreviewRegion pr;

			if (r is PreviewRegion)
			{
				pr = (PreviewRegion)r;
				pr.EffectID = effect;
				pr.EffectHue = hue;
				pr.EffectRender = render;
				pr.Register();

				return pr;
			}

			var s = GetSerial(r);

			if (s == null)
			{
				return null;
			}

			if (_Previews.TryGetValue(s, out pr) && pr != null && pr.Area.GetBoundsHashCode() != r.Area.GetBoundsHashCode())
			{
				pr.Unregister();
				pr = null;
			}

			if (pr == null)
			{
				pr = new PreviewRegion(r)
				{
					EffectHue = hue,
					EffectID = effect,
					EffectRender = render
				};
			}

			pr.Register();

			return pr;
		}

		public static void ClearPreview(this Region r)
		{
			if (r == null)
			{
				return;
			}

			PreviewRegion pr;

			if (r is PreviewRegion)
			{
				pr = (PreviewRegion)r;
				pr.Unregister();

				return;
			}

			var s = GetSerial(r);

			if (s == null)
			{
				return;
			}

			if (_Previews.TryGetValue(s, out pr) && pr != null)
			{
				pr.Unregister();
			}
		}

		public static bool Contains(this Sector s, Point3D p, Map m)
		{
			return s.Owner == m && s.Contains(p);
		}

		public static bool Contains(this Region r, Point3D p, Map m)
		{
			return r.Map == m && r.Contains(p);
		}

#if ServUOX
		public static bool Contains(this Sector s, Point3D p)
		{
			foreach (var r in s.RegionRects.Values.SelectMany(o => o))
			{
				if (r.Contains(p))
				{
					return true;
				}
			}

			return false;
		}
#else
		public static bool Contains(this RegionRect r, Point3D p, Map m)
		{
			return r.Region.Map == m && r.Contains(p);
		}

		public static bool Contains(this Sector s, Point3D p)
		{
			foreach (var r in s.RegionRects)
			{
				if (r.Contains(p))
				{
					return true;
				}
			}

			return false;
		}

		public static bool Contains(this RegionRect r, Point3D p)
		{
			return r.Rect.Contains(p);
		}
#endif

		public static TRegion Create<TRegion>(params object[] args)
			where TRegion : Region
		{
			return Create(typeof(TRegion), args) as TRegion;
		}

		public static Region Create(Type type, params object[] args)
		{
			return type.CreateInstanceSafe<Region>(args);
		}

		public static TRegion Clone<TRegion>(this TRegion region, params object[] args)
			where TRegion : Region
		{
			if (region == null)
			{
				return null;
			}

			var fields = region.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			var remove = new[]
			{
				"Serial",
				"Name",
				"Map",
				"Parent",
				"Area",
				"Sectors",
				"ChildLevel",
				"Registered"
			};

			foreach (var entry in remove)
            {
				if (!fields.Remove("m_" + entry))
				{
					fields.Remove("_" + entry);
				}
            }

			region.Unregister();

			var reg = Create(region.GetType(), args) as TRegion;

			if (reg != null)
			{
				fields.Serialize(reg);
				reg.Register();
			}

			return reg;
		}

#if ServUOX
		public static bool IsPartOf<TRegion>(Region region)
			where TRegion : Region
		{
			return region != null && region.IsPartOf<TRegion>();
		}

		public static TRegion GetRegion<TRegion>(Region region)
			where TRegion : Region
		{
			return region != null ? region.GetRegion<TRegion>() : null;
		}
#else
		public static bool IsPartOf<TRegion>(this Region region)
			where TRegion : Region
		{
			return region != null && region.IsPartOf(typeof(TRegion));
		}

		public static TRegion GetRegion<TRegion>(this Region region)
			where TRegion : Region
		{
			return region != null ? region.GetRegion(typeof(TRegion)) as TRegion : null;
		}
#endif

		public static TRegion GetRegion<TRegion>(this Mobile m)
			where TRegion : Region
		{
			return m != null ? GetRegion<TRegion>(m.Region) : null;
		}

		public static Region GetRegion(this Mobile m, Type type)
		{
			return m != null && m.Region != null ? m.Region.GetRegion(type) : null;
		}

		public static Region GetRegion(this Mobile m, string name)
		{
			return m != null && m.Region != null ? m.Region.GetRegion(name) : null;
		}

		public static bool InRegion<TRegion>(this Mobile m)
			where TRegion : Region
		{
			return m != null && GetRegion<TRegion>(m.Region) != null;
		}

		public static bool InRegion(this Mobile m, Type type)
		{
			return m != null && GetRegion(m, type) != null;
		}

		public static bool InRegion(this Mobile m, string name)
		{
			return m != null && GetRegion(m, name) != null;
		}

		public static bool InRegion(this Mobile m, Region r)
		{
			return m != null && m.Region != null && m.Region.IsPartOf(r);
		}

		public static Region GetRegion(this Item i)
		{
			if (i == null)
			{
				return null;
			}

			return Region.Find(i.GetWorldLocation(), i.Map);
		}

		public static TRegion GetRegion<TRegion>(this Item i)
			where TRegion : Region
		{
			if (i == null)
			{
				return null;
			}

			var reg = Region.Find(i.GetWorldLocation(), i.Map);

			return reg != null ? GetRegion<TRegion>(reg) : null;
		}

		public static Region GetRegion(this Item i, string name)
		{
			if (i == null)
			{
				return null;
			}

			var reg = Region.Find(i.GetWorldLocation(), i.Map);

			return reg != null ? reg.GetRegion(name) : null;
		}

		public static Region GetRegion(this Item i, Type type)
		{
			if (i == null)
			{
				return null;
			}

			var reg = Region.Find(i.GetWorldLocation(), i.Map);

			return reg != null ? reg.GetRegion(type) : null;
		}

		public static bool InRegion<TRegion>(this Item i)
			where TRegion : Region
		{
			if (i == null)
			{
				return false;
			}

			var reg = Region.Find(i.GetWorldLocation(), i.Map);

			return reg != null && GetRegion<TRegion>(reg) != null;
		}

		public static bool InRegion(this Item i, Type type)
		{
			return i != null && GetRegion(i, type) != null;
		}

		public static bool InRegion(this Item i, string name)
		{
			return i != null && GetRegion(i, name) != null;
		}

		public static bool InRegion(this Item i, Region r)
		{
			if (i == null)
			{
				return false;
			}

			var reg = Region.Find(i.GetWorldLocation(), i.Map);

			return reg != null && reg.IsPartOf(r);
		}
	}
}
