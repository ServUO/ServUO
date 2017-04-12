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

namespace Server.Mobiles
{
    public static class MobileExtensions
    {
        public static void SayTo(this Mobile mobile, Mobile to, int number, int hue)
        {
            mobile.PrivateOverheadMessage(MessageType.Regular, hue, number, to.NetState);
        }

        public static void SayTo(this Mobile mobile, Mobile to, int number, string args, int hue)
        {
            mobile.PrivateOverheadMessage(MessageType.Regular, hue, number, args, to.NetState);
        }

        public static void SayTo(this Mobile mobile, Mobile to, string text, int hue, bool ascii = false)
        {
            mobile.PrivateOverheadMessage(MessageType.Regular, hue, ascii, text, to.NetState);
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
            if (region == null)
                return null;

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
            if (region == null)
            {
                yield break;
            }

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
            if (region == null)
            {
                return 0;
            }

            int count = 0;

            foreach(Sector s in region.Sectors)
            {
                count += s.Items.Where(i => i.GetRegion().IsPartOf(region)).Count();
            }

            return count;
        }

        public static int GetItemCount(this Region region, Func<Item, bool> predicate)
        {
            if (region == null)
            {
                return 0;
            }

            int count = 0;

            foreach(Sector s in region.Sectors)
            {
                count += s.Items.Where(i => i.GetRegion().IsPartOf(region) && (predicate == null || predicate(i))).Count();
            }

            return count;
        }

        public static List<BaseMulti> GetMultis(this Region region)
        {
            if (region == null)
            {
                return null;
            }

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
            if (region == null)
            {
                yield break;
            }

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
            if (region == null)
            {
                return 0;
            }

            int count = 0;

            foreach (Sector s in region.Sectors)
            {
                count += s.Multis.Where(m => m.GetRegion().IsPartOf(region)).Count();
            }

            return count;
        }

        public static IEnumerable<Mobile> GetEnumeratedMobiles(this Region region)
        {
            if (region == null)
            {
                yield break;
            }

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

            ColUtility.Free(list);
        }
    }

    public static class ColUtility
    {
        public static void Free<T>(List<T> l)
        {
            if (l == null)
                return;

            l.Clear();
            l.TrimExcess();
        }

        public static void ForEach<T>(IEnumerable<T> list, Action<T> action)
        {
            if (list == null || action == null)
                return;

            List<T> l = list.ToList();

            foreach (T o in l)
                action(o);

            Free(l);
        }

        public static void ForEach<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary, Action<KeyValuePair<TKey, TValue>> action)
        {
            if (dictionary == null || dictionary.Count == 0 || action == null)
                return;

            List<KeyValuePair<TKey, TValue>> l = dictionary.ToList();

            foreach (KeyValuePair<TKey, TValue> kvp in l)
                action(kvp);

            Free(l);
        }

        public static void ForEach<TKey, TValue>(IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
        {
            if (dictionary == null || dictionary.Count == 0 || action == null)
                return;

            List<KeyValuePair<TKey, TValue>> l = dictionary.ToList();

            foreach (KeyValuePair<TKey, TValue> kvp in l)
                action(kvp.Key, kvp.Value);

            Free(l);
        }

        public static void For<T>(IEnumerable<T> list, Action<int, T> action)
        {
            if (list == null || action == null)
                return;

            List<T> l = list.ToList();

            for (int i = 0; i < l.Count; i++)
                action(i, l[i]);

            Free(l);
        }

        public static void For<TKey, TValue>(IDictionary<TKey, TValue> list, Action<int, TKey, TValue> action)
        {
            if (list == null || action == null)
                return;

            List<KeyValuePair<TKey, TValue>> l = list.ToList();

            for (int i = 0; i < l.Count; i++)
                action(i, l[i].Key, l[i].Value);

            Free(l);
        }
    }
}
