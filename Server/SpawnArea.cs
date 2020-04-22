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

        public const int Stride = 32;

        static SpawnArea()
        {
            _EmptyImage = new Bitmap(1, 1, PixelFormat.Format16bppRgb555);

            _EmptyFilters = new TileFlag[0];

            _AllFilters = Enum.GetValues(typeof(TileFlag)).Cast<TileFlag>().Where(f => f != TileFlag.None).ToArray();

            _Cache = new Dictionary<int, SpawnArea>();
        }

        public static SpawnArea Instantiate(Region region, TileFlag filter, SpawnValidator validator, bool cache)
        {
            string name = region.Name;

            if (region.IsDefault || String.IsNullOrWhiteSpace(name))
            {
                name = "Default";
            }

            TileFlag[] filters = GetFilters(filter);

            int hash = GetHashCode(region.Map, name, filters, validator);

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

        private static int GetHashCode(Map facet, string region, IEnumerable<TileFlag> filters, SpawnValidator validator)
        {
            unchecked
            {
                int hash = region.Length;

                hash = region.Aggregate(hash, (v, c) => unchecked((v * 397) ^ Convert.ToInt32(c)));

                hash = (hash * 397) ^ facet.MapID;
                hash = (hash * 397) ^ facet.MapIndex;

                TileFlag filter = TileFlag.None;

                foreach (TileFlag f in filters)
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

        private readonly HashSet<Point3D> _Points;

        public SpawnValidator Validator { get; private set; }

        public TileFlag[] Filters { get; private set; }

        public Map Facet { get; private set; }

        public string Region { get; private set; }

        public Point2D Center { get; private set; }

        public Rectangle3D Bounds => _Bounds;

        public int Count => _Points.Count;

        public Bitmap Image => GetImage();

        bool ICollection<Point3D>.IsReadOnly => true;

        private SpawnArea(Map facet, string region, TileFlag[] filters, SpawnValidator validator)
        {
            _Points = new HashSet<Point3D>();

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

                Bitmap map = new Bitmap(_Bounds.Width, _Bounds.Height, PixelFormat.Format16bppRgb555);

                Rectangle b = new Rectangle(_Bounds.Start.X >> 3, _Bounds.Start.Y >> 3, _Bounds.Width >> 3, _Bounds.Height >> 3);

                umap.GetImage(b.X, b.Y, b.Width, b.Height, map, true);

                Rectangle l = new Rectangle(Point.Empty, map.Size);

                BitmapData data = map.LockBits(l, ImageLockMode.ReadWrite, map.PixelFormat);

                Parallel.ForEach(_Points, o => SetPixel(o.X - _Bounds.Start.X, o.Y - _Bounds.Start.Y, data));

                map.UnlockBits(data);
                
                return _Image = map;
            }
        }

        private static unsafe void SetPixel(int x, int y, BitmapData data)
        {
            int index = (y * data.Stride) + (x * 2);
            byte* pixel = (byte*)data.Scan0.ToPointer();

            pixel[index + 0] = (PixelColor >> 0) & 0xFF;
            pixel[index + 1] = (PixelColor >> 8) & 0xFF;
        }

        public bool Contains(int x, int y)
        {
            return Contains(x, y, Facet.Tiles.GetLandTile(x, y).Z);
        }

        public bool Contains(int x, int y, int z)
        {
            return Contains(new Point3D(x, y, z));
        }

        public bool Contains(IPoint3D p)
        {
            return Contains(new Point3D(p));
        }

        public bool Contains(Point3D p)
        {
            return _Points.Contains(p);
        }

        public Point3D GetRandom()
        {
            if (Facet == null || Facet == Map.Internal || Count == 0)
            {
                return Point3D.Zero;
            }

            Point3D p = Point3D.Zero;

            if (Count <= 1024)
            {
                p = _Points.ElementAt(Utility.Random(Count));
            }

            if (p == Point3D.Zero)
            {
                do
                {
                    p.X = Utility.RandomMinMax(_Bounds.Start.X, _Bounds.End.X);
                    p.Y = Utility.RandomMinMax(_Bounds.Start.Y, _Bounds.End.Y);
                    p.Z = Facet.Tiles.GetLandTile(p.X, p.Y).Z;
                }
                while (!Contains(p));
            }

            if (Validator == null || Validator(Facet, p.X, p.Y, p.Z))
            {
                return p;
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

            IEnumerable<Rectangle3D> bounds;

            if (region.IsDefault)
            {
                int fw = Facet.MapID <= 1 ? 5119 : Facet.Width;
                int fh = Facet.MapID <= 1 ? 4095 : Facet.Height;
                int fd = Server.Region.MaxZ - Server.Region.MinZ;

                _Bounds = new Rectangle3D(0, 0, Server.Region.MinZ, fw, fh, fd);

                bounds = new[] { _Bounds };
            }
            else
            {
                int x1 = short.MaxValue, y1 = short.MaxValue, z1 = sbyte.MaxValue;
                int x2 = short.MinValue, y2 = short.MinValue, z2 = sbyte.MinValue;

                foreach (Rectangle3D o in region.Area)
                {
                    x1 = Math.Min(x1, o.Start.X);
                    y1 = Math.Min(y1, o.Start.Y);
                    z1 = Math.Min(z1, o.Start.Z);

                    x2 = Math.Max(x2, o.End.X);
                    y2 = Math.Max(y2, o.End.Y);
                    z2 = Math.Max(z2, o.End.Z);
                }

                _Bounds = new Rectangle3D(x1, y1, z1, x2 - x1, y2 - y1, z2 - z1);

                bounds = region.Area;
            }

            ParallelQuery<Point3D> pending = bounds.SelectMany(Slice).AsParallel().SelectMany(Compute);

            pending = pending.WithMergeOptions(ParallelMergeOptions.NotBuffered);
            pending = pending.WithExecutionMode(ParallelExecutionMode.ForceParallelism);

            foreach (Point3D p in pending)
            {
                _Points.Add(p);
            }

            Center = new Point2D(_Bounds.Start.X + (_Bounds.Width / 2), _Bounds.Start.Y + (_Bounds.Height / 2));
        }

        private IEnumerable<Point3D> Compute(Rectangle3D area)
        {
            // Check all corners to skip large bodies of water.
            if (Filters.Contains(TileFlag.Wet))
            {
                LandTile land1 = Facet.Tiles.GetLandTile(area.Start.X, area.Start.Y); // TL
                LandTile land2 = Facet.Tiles.GetLandTile(area.End.X, area.Start.Y); // TR
                LandTile land3 = Facet.Tiles.GetLandTile(area.Start.X, area.End.Y); // BL
                LandTile land4 = Facet.Tiles.GetLandTile(area.End.X, area.End.Y); // BR

                bool ignore1 = land1.Ignored || TileData.LandTable[land1.ID].Flags.HasFlag(TileFlag.Wet);
                bool ignore2 = land2.Ignored || TileData.LandTable[land2.ID].Flags.HasFlag(TileFlag.Wet);
                bool ignore3 = land3.Ignored || TileData.LandTable[land3.ID].Flags.HasFlag(TileFlag.Wet);
                bool ignore4 = land4.Ignored || TileData.LandTable[land4.ID].Flags.HasFlag(TileFlag.Wet);

                if (ignore1 && ignore2 && ignore3 && ignore4)
                {
                    yield break;
                }
            }

            Point3D p = Point3D.Zero;

            for (p.X = area.Start.X; p.X <= area.End.X; p.X++)
            {
                for (p.Y = area.Start.Y; p.Y <= area.End.Y; p.Y++)
                {
                    LandTile land = Facet.Tiles.GetLandTile(p.X, p.Y);

                    p.Z = land.Z;

                    if (Contains(p))
                    {
                        continue;
                    }

                    if (!CanSpawn(p.X, p.Y, p.Z))
                    {
                        continue;
                    }

                    if (Filters.Length > 0)
                    {
                        if (land.Ignored)
                        {
                            continue;
                        }

                        TileFlag flags = TileData.LandTable[land.ID].Flags;

                        if (Filters.Any(f => flags.HasFlag(f)))
                        {
                            continue;
                        }

                        bool valid = true;

                        foreach (StaticTile tile in Facet.Tiles.GetStaticTiles(p.X, p.Y))
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

                    if (Validator != null && !Validator(Facet, p.X, p.Y, p.Z))
                    {
                        continue;
                    }

                    yield return p;
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
            return _Points.GetEnumerator();
        }

        void ICollection<Point3D>.Clear()
        {
            _Points.Clear();
        }

        void ICollection<Point3D>.Add(Point3D p)
        {
            _Points.Add(p);
        }

        bool ICollection<Point3D>.Remove(Point3D p)
        {
            return _Points.Remove(p);
        }

        bool ICollection<Point3D>.Contains(Point3D p)
        {
            return _Points.Contains(p);
        }

        void ICollection<Point3D>.CopyTo(Point3D[] array, int index)
        {
            _Points.CopyTo(array, index);
        }
    }
}
