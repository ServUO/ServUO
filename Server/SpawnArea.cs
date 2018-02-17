#region Header
//   Vorspire    _,-'/-'/  SpawnArea.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2018  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Server
{
	public delegate bool SpawnValidator(Map map, int x, int y, int z);

	public sealed class SpawnArea : ICollection<Point3D>
	{
		private static readonly Bitmap _EmptyImage;

		private static readonly TileFlag[] _EmptyFilters;
		private static readonly TileFlag[] _AllFilters;

		private static readonly Dictionary<int, SpawnArea> _Cache;

		public const ushort PixelColor = 0xFC1F;

		public const int Stride = 16;

		static SpawnArea()
		{
			_EmptyImage = new Bitmap(1, 1, PixelFormat.Format16bppRgb555);

			_EmptyFilters = new TileFlag[0];

			_AllFilters = Enum.GetValues(typeof(TileFlag)).Cast<TileFlag>().Where(f => f != TileFlag.None).ToArray();

			_Cache = new Dictionary<int, SpawnArea>();
		}

		public static SpawnArea Instantiate(Region region, TileFlag filter, SpawnValidator validator, bool cache)
		{
			var name = region.Name;

			if (region.IsDefault || String.IsNullOrWhiteSpace(name))
			{
				name = "Default";
			}

			var filters = GetFilters(filter);

			var hash = GetHashCode(region.Map, name, filters, validator);

			SpawnArea o;

			if (!_Cache.TryGetValue(hash, out o) || o == null)
			{
				o = new SpawnArea(region.Map, name, filters, validator);

				if (cache)
				{
					_Cache[hash] = o;
				}

				o.Invalidate();
			}

			return o;
		}

		private static IEnumerable<Rectangle3D> Slice(Rectangle3D rect)
		{
			if (rect.Width <= Stride && rect.Height <= Stride)
			{
				yield return rect;
				yield break;
			}

			int x, y, z = Math.Min(rect.Start.Z, rect.End.Z);
			int ow, oh, od = rect.Depth;

			x = rect.Start.X;

			while (x < rect.End.X)
			{
				ow = Math.Min(Stride, rect.End.X - x);
				
				y = rect.Start.Y;

				while (y < rect.End.Y)
				{
					oh = Math.Min(Stride, rect.End.Y - y);
					
					yield return new Rectangle3D(x, y, z, ow, oh, od);

					y += oh;
				}

				x += ow;
			}
		}

		private static TileFlag[] GetFilters(TileFlag filter)
		{
			if (filter == TileFlag.None)
			{
				return _EmptyFilters;
			}

			return _AllFilters.Where(f => f != TileFlag.None && filter.HasFlag(f)).ToArray();
		}

		private static int GetHashCode(int x, int y)
		{
			unchecked
			{
				var hash = x + y;

				hash = (hash * 397) ^ x;
				hash = (hash * 397) ^ y;

				return hash;
			}
		}

		private static int GetHashCode(Map facet, string region, IEnumerable<TileFlag> filters, SpawnValidator validator)
		{
			unchecked
			{
				var hash = region.Length;

				hash = region.Aggregate(hash, (v, c) => unchecked((v * 397) ^ Convert.ToInt32(c)));

				hash = (hash * 397) ^ facet.MapID;
				hash = (hash * 397) ^ facet.MapIndex;

				var filter = TileFlag.None;

				foreach (var f in filters)
				{
					filter |= f;
				}

				if (filter != TileFlag.None)
				{
					hash = (hash * 397) ^ (int)(((long)filter >> 0) & 0x7FFFFFFF);
					hash = (hash * 397) ^ (int)(((long)filter >> 32) & 0x7FFFFFFF);
				}

				if (validator != null)
				{
					hash = (hash * 397) ^ validator.GetHashCode();
				}

				return hash;
			}
		}

		private Bitmap _Image;

		private Rectangle3D _Bounds;

		private readonly Dictionary<int, Point3D> _Points;

		public SpawnValidator Validator { get; private set; }

		public TileFlag[] Filters { get; private set; }

		public Map Facet { get; private set; }

		public string Region { get; private set; }

		public Point2D Center { get; private set; }

		public Rectangle3D Bounds { get { return _Bounds; } }

		public int Count { get { return _Points.Count; } }

		public Bitmap Image { get { return GetImage(); } }

		bool ICollection<Point3D>.IsReadOnly { get { return true; } }

		private SpawnArea(Map facet, string region, TileFlag[] filters, SpawnValidator validator)
		{
			_Points = new Dictionary<int, Point3D>();

			Facet = facet;
			Region = region;
			Filters = filters;
			Validator = validator;
		}

		public Bitmap GetImage()
		{
			if (Facet == null)
			{
				return _EmptyImage;
			}

			lock (this)
			{
				if (_Image != null)
				{
					return _Image;
				}

				Ultima.Map umap;

				switch (Facet.MapID)
				{
					case 0:
						umap = Ultima.Map.Felucca;
						break;
					case 1:
						umap = Ultima.Map.Trammel;
						break;
					case 2:
						umap = Ultima.Map.Ilshenar;
						break;
					case 3:
						umap = Ultima.Map.Malas;
						break;
					case 4:
						umap = Ultima.Map.Tokuno;
						break;
					case 5:
						umap = Ultima.Map.TerMur;
						break;
					default:
						return _Image = _EmptyImage;
				}

				var map = new Bitmap(_Bounds.Width, _Bounds.Height, PixelFormat.Format16bppRgb555);

				var b = new Rectangle(_Bounds.Start.X >> 3, _Bounds.Start.Y >> 3, _Bounds.Width >> 3, _Bounds.Height >> 3);

				umap.GetImage(b.X, b.Y, b.Width, b.Height, map, true);

				b = new Rectangle(Point.Empty, map.Size);

				var data = map.LockBits(b, ImageLockMode.ReadWrite, map.PixelFormat);

				b = new Rectangle(_Bounds.Start.X, _Bounds.Start.Y, _Bounds.Width, _Bounds.Height);

				Parallel.ForEach(_Points.Values, o => SetPixel(o.X - b.X, o.Y - b.Y, data));

				map.UnlockBits(data);

				return _Image = map;
			}
		}

		private static unsafe void SetPixel(int x, int y, BitmapData data)
		{
			var index = (y * data.Stride) + (x * 2);
			var pixel = (byte*)data.Scan0.ToPointer();

			pixel[index + 0] = (PixelColor >> 0) & 0xFF;
			pixel[index + 1] = (PixelColor >> 8) & 0xFF;
		}

		public bool Contains(int x, int y)
		{
			return _Points.ContainsKey(GetHashCode(x, y));
		}

		public bool Contains(IPoint2D p)
		{
			return _Points.ContainsKey(GetHashCode(p.X, p.Y));
		}

		public Point3D GetRandom()
		{
			if (Facet == null || Facet == Map.Internal || Count == 0)
			{
				return Point3D.Zero;
			}

			if (Count <= 1024)
			{
				return _Points.Values.ElementAt(Utility.Random(Count));
			}

			int x, y;

			do
			{
				x = Utility.RandomMinMax(_Bounds.Start.X, _Bounds.End.X);
				y = Utility.RandomMinMax(_Bounds.Start.Y, _Bounds.End.Y);
			}
			while (!Contains(x, y));

			var z = Facet.GetAverageZ(x, y);

			if (Validator == null || Validator(Facet, x, y, z))
			{
				return new Point3D(x, y, z);
			}

			return GetRandom();
		}

		public void Invalidate()
		{
			_Image = null;

			_Points.Clear();

			if (Facet == null || Facet == Map.Internal)
			{
				return;
			}

			Region region;

			if (String.IsNullOrWhiteSpace(Region) || Region == "Default")
			{
				region = Facet.DefaultRegion;
			}
			else if (!Facet.Regions.TryGetValue(Region, out region))
			{
				return;
			}

			if (region == null || (!region.IsDefault && (region.Area == null || region.Area.Length == 0)))
			{
				return;
			}

			if (region.IsDefault)
			{
				var fw = Facet.MapID <= 1 ? 5119 : Facet.Width;
				var fh = Facet.MapID <= 1 ? 4095 : Facet.Height;
				var fd = Server.Region.MaxZ - Server.Region.MinZ;

				_Bounds = new Rectangle3D(0, 0, Server.Region.MinZ, fw, fh, fd);

				Parallel.ForEach(Slice(_Bounds), Compute);
			}
			else
			{
				int x1 = Int16.MaxValue, y1 = Int16.MaxValue, z1 = SByte.MaxValue;
				int x2 = Int16.MinValue, y2 = Int16.MinValue, z2 = SByte.MinValue;

				foreach (var o in region.Area)
				{
					x1 = Math.Min(x1, o.Start.X);
					y1 = Math.Min(y1, o.Start.Y);
					z1 = Math.Min(z1, o.Start.Z);

					x2 = Math.Max(x2, o.End.X);
					y2 = Math.Max(y2, o.End.Y);
					z2 = Math.Max(z2, o.End.Z);
				}

				_Bounds = new Rectangle3D(x1, y1, z1, x2 - x1, y2 - y1, z2 - z1);

				Parallel.ForEach(region.Area.SelectMany(Slice), Compute);
			}

			Center = new Point2D(_Bounds.Start.X + (_Bounds.Width / 2), _Bounds.Start.Y + (_Bounds.Height / 2));
		}

		private void Compute(Rectangle3D area)
		{
			// Check all corners to skip large bodies of water.
			if (Filters.Contains(TileFlag.Wet))
			{
				var land1 = Facet.Tiles.GetLandTile(area.Start.X, area.Start.Y); // TL
				var land2 = Facet.Tiles.GetLandTile(area.End.X, area.Start.Y); // TR
				var land3 = Facet.Tiles.GetLandTile(area.Start.X, area.End.Y); // BL
				var land4 = Facet.Tiles.GetLandTile(area.End.X, area.End.Y); // BR

				if ((land1.Ignored || TileData.LandTable[land1.ID].Flags.HasFlag(TileFlag.Wet)) &&
					(land2.Ignored || TileData.LandTable[land2.ID].Flags.HasFlag(TileFlag.Wet)) &&
					(land3.Ignored || TileData.LandTable[land3.ID].Flags.HasFlag(TileFlag.Wet)) &&
					(land4.Ignored || TileData.LandTable[land4.ID].Flags.HasFlag(TileFlag.Wet)))
				{
					return;
				}
			}

			int x, y, z, h;

			for (x = area.Start.X; x < area.End.X; x++)
			{
				for (y = area.Start.Y; y < area.End.Y; y++)
				{
					h = GetHashCode(x, y);

					if (_Points.ContainsKey(h))
					{
						continue;
					}

					z = Facet.Tiles.GetLandTile(x, y).Z;//.GetAverageZ(x, y);

					if (!CanSpawn(x, y, z))
					{
						continue;
					}

					if (Filters.Length > 0)
					{
						var land = Facet.Tiles.GetLandTile(x, y);

						if (land.Ignored)
						{
							continue;
						}

						var flags = TileData.LandTable[land.ID].Flags;

						if (Filters.Any(f => flags.HasFlag(f)))
						{
							continue;
						}

						var valid = true;

						foreach (var tile in Facet.Tiles.GetStaticTiles(x, y))
						{
							flags = TileData.ItemTable[tile.ID].Flags;

							if (Filters.Any(f => flags.HasFlag(f)))
							{
								valid = false;
								break;
							}
						}

						if (!valid)
						{
							continue;
						}
					}

					if (Validator != null && !Validator(Facet, x, y, z))
					{
						continue;
					}

					lock (_Points)
					{
						_Points[h] = new Point3D(x, y, z);
					}
				}
			}
		}

		private bool CanSpawn(int x, int y, int z)
		{
			return Facet.CanFit(x, y, z, Server.Region.MaxZ - z, true, false, true);
		}

		public override int GetHashCode()
		{
			return GetHashCode(Facet, Region, Filters, Validator);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Point3D> GetEnumerator()
		{
			return _Points.Values.GetEnumerator();
		}

		void ICollection<Point3D>.Clear()
		{
			_Points.Clear();
		}

		void ICollection<Point3D>.Add(Point3D p)
		{
			_Points[GetHashCode(p.X, p.Y)] = p;
		}

		bool ICollection<Point3D>.Remove(Point3D p)
		{
			return _Points.Remove(GetHashCode(p.X, p.Y));
		}

		bool ICollection<Point3D>.Contains(Point3D p)
		{
			return _Points.ContainsKey(GetHashCode(p.X, p.Y));
		}

		void ICollection<Point3D>.CopyTo(Point3D[] array, int index)
		{
			_Points.Values.CopyTo(array, index);
		}
	}
}