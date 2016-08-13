using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public static class ItemExtensions
    {
        public static int GetInsuranceCost(this Item item)
        {
            var imbueWeight = SkillHandlers.Imbuing.GetTotalWeight(item);
            int cost = 600; // this handles old items, set items, etc

            if (item.GetType().IsAssignableFrom(typeof(Factions.FactionItem)))
                cost = 800;
            else if (imbueWeight > 0)
                cost = Math.Min(800, Math.Max(10, imbueWeight));
            else if (Mobiles.GenericBuyInfo.BuyPrices.ContainsKey(item.GetType()))
                cost = Math.Min(800, Math.Max(10, Mobiles.GenericBuyInfo.BuyPrices[item.GetType()]));
            else if (item.LootType == LootType.Newbied)
                return 10;

            if ((item is BaseArmor && ((BaseArmor)item).NegativeAttributes.Prized > 0) ||
                (item is BaseWeapon && ((BaseWeapon)item).NegativeAttributes.Prized > 0) ||
                (item is BaseJewel && ((BaseJewel)item).NegativeAttributes.Prized > 0) ||
                (item is BaseClothing && ((BaseClothing)item).NegativeAttributes.Prized > 0))
                cost *= 2;

            return cost;
        }

        public static Region GetRegion(this Item item)
        {
            return Region.Find(item.GetWorldLocation(), item.Map);
        }

        public static double GetDistanceToSqrt(this Item item, IPoint3D p)
        {
            Point3D loc = item.GetWorldLocation();

            int xDelta = loc.X - p.X;
            int yDelta = loc.Y - p.Y;

            return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
        }

        public static bool InRange(this Item item, IPoint3D p, int range)
        {
            Point3D loc = item.GetWorldLocation();

            return (p.X >= (loc.X - range))
                && (p.X <= (loc.X + range))
                && (p.Y >= (loc.Y - range))
                && (p.Y <= (loc.Y + range));
        }

        public static void PrivateOverheadMessage(this Item item, MessageType type, int hue, int number, NetState state, string args = "")
        {
            if (item.Map != null && state != null)
            {
                Packet p = null;
                Point3D worldLoc = item.GetWorldLocation();

                Mobile m = state.Mobile;

                if (m != null && m.CanSee(item) && m.InRange(worldLoc, item.GetUpdateRange(m)))
                {
                    if (p == null)
                        p = Packet.Acquire(new MessageLocalized(item.Serial, item.ItemID, type, hue, 3, number, item.Name, args));

                    state.Send(p);
                }

                Packet.Release(p);
            }
        }

        public static bool InLOS(this Item item, Point3D target)
        {
            if (item.Deleted || item.Map == null || item.Parent != null)
                return false;

            return item.Map.LineOfSight(item, target);
        }
    }
}

namespace Server
{
    public static class GeomontryExtentions
    {
        public static Point3D GetRandomSpawnPoint(this Rectangle2D rec, Map map)
        {
            if (map == null || map == Map.Internal)
                return Point3D.Zero;

            int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
            int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
            int z = map.GetAverageZ(x, y);

            return new Point3D(x, y, z);
        }
    }

    public static class RegionExtensions
    {
        private static readonly object _ItemLock = new object();
        private static readonly object _MultiLock = new object();
        private static readonly object _MobileLock = new object();

        public static List<Item> GetItems(this Region region)
        {
            List<Item> list = new List<Item>();

            foreach(Sector s in region.Sectors)
            {
                foreach(Item item in s.Items.Where(i => i.GetRegion().IsPartOf(region)))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public static IEnumerable<Item> GetEnumeratedItems(this Region region)
        {
            List<Item> list = region.GetItems();
            IEnumerable<Item> e;

            lock (_ItemLock)
            {
                e = list.AsParallel().Where(i => i != null && i.GetRegion().IsPartOf(region));
            }

            foreach (Item item in e)
            {
                yield return item;
            }

            list.Clear();
            list.TrimExcess();
        }

        public static int GetItemCount(this Region region)
        {
            int count = 0;

            foreach(Sector s in region.Sectors)
            {
                count += s.Items.Where(i => i.GetRegion().IsPartOf(region)).Count();
            }

            return count;
        }

        public static int GetItemCount(this Region region, Func<Item, bool> predicate)
        {
            int count = 0;

            foreach(Sector s in region.Sectors)
            {
                count += s.Items.Where(i => i.GetRegion().IsPartOf(region) && (predicate == null || predicate(i))).Count();
            }

            return count;
        }

        public static List<BaseMulti> GetMultis(this Region region)
        {
            List<BaseMulti> list = new List<BaseMulti>();

            foreach (Sector s in region.Sectors)
            {
                foreach(BaseMulti multi in s.Multis.Where(m => m.GetRegion().IsPartOf(region)))
                {
                    list.Add(multi);
                }
            }

            return list;
        }

        public static IEnumerable<BaseMulti> GetEnumeratedMultis(this Region region)
        {
            List<BaseMulti> list = region.GetMultis();
            IEnumerable<BaseMulti> e;

            lock (_MultiLock)
            {
                e = list.AsParallel().Where(m => m != null && m.GetRegion().IsPartOf(region));
            }

            foreach (BaseMulti multi in e)
            {
                yield return multi;
            }

            list.Clear();
            list.TrimExcess();
        }

        public static int GetMultiCount(this Region region)
        {
            int count = 0;

            foreach (Sector s in region.Sectors)
            {
                count += s.Multis.Where(m => m.GetRegion().IsPartOf(region)).Count();
            }

            return count;
        }

        public static IEnumerable<Mobile> GetEnumeratedMobiles(this Region region)
        {
            List<Mobile> list = region.GetMobiles();
            IEnumerable<Mobile> e;

            lock (_MobileLock)
            {
                e = list.AsParallel().Where(m => m != null && m.Region.IsPartOf(region));
            }

            foreach (Mobile m in e)
            {
                yield return m;
            }

            list.Clear();
            list.TrimExcess();
        }
    }
}
