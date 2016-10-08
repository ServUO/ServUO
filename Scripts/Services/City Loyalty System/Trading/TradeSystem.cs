using System;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.Points;
using System.Collections.Generic;
using System.Linq;
using Server.Engines;
using Server.Items;
using Server.Multis;
using Server.Regions;

namespace Server.Engines.CityLoyalty
{
    public enum TradeTitle
    {
        Trader      = 1151739,
        Exporter    = 1151741,
        Broker      = 1151743,
        Tycoon      = 1151745,
        Smuggler    = 1151747,
        Magnate     = 1155481
    }

	public class CityTradeSystem : PointsSystem
	{
        public static readonly int TurnInGold = Config.Get("CityTrading.TurnInGold", 10000);
        public static readonly int CrateDuration = Config.Get("CityTrading.CrateDuration", 24);
        public static readonly int AmbushWaitDuration = Config.Get("CityTrading.AmbushWaitDuration", 5);
        public static readonly int AmbusherDelete = Config.Get("CityTrading.AmbusherDelete", 10);

        public override TextDefinition Name { get { return new TextDefinition("City Trading"); } }
        public override PointsType Loyalty { get { return PointsType.CityTrading; } }
        public override bool AutoAdd { get { return false; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

		public static Dictionary<Mobile, TradeOrderCrate> ActiveTrades { get; private set; }
        public static Dictionary<BaseCreature, DateTime> Ambushers { get; private set; }
		
		public CityTradeSystem() 
		{
			ActiveTrades = new Dictionary<Mobile, TradeOrderCrate>();

            _NameBuffer = new Dictionary<Type, string>();
		}

        public override PointsEntry GetSystemEntry(PlayerMobile pm)
        {
            return new CityTradeEntry(pm);
        }

		public int GetMaxTrades(Mobile m)
		{
            CityTradeEntry entry = GetPlayerEntry<CityTradeEntry>(m as PlayerMobile);

			if(entry == null)
				return 1;

            return Math.Min(8, Math.Max(1, (entry.Completed / 25) + 1));
		}
		
		public static bool HasTrade(Mobile from)
		{
            return ActiveTrades.ContainsKey(from);
		}
		
		public bool HasTurnIn(Mobile from, TradeMinister minister)
		{
			if(from == null || minister == null || !ActiveTrades.ContainsKey(from) || ActiveTrades[from] == null)
				return false;

            TradeOrderCrate crate = ActiveTrades[from];

            return crate.Entry != null && crate.Entry.Origin != minister.City;
		}
		
		public bool TryOfferTrade(Mobile from, TradeMinister minister)
		{
			if(from == null || from.Backpack == null)
				return true;
				
			if(ActiveTrades.ContainsKey(from))
				minister.SayTo(from, 1151722); // It appears you are already delivering a trade order. Deliver your current order before requesting another.
			else
			{
				City origin = minister.City;
				City destination;

                do
                {
                    destination = CityLoyaltySystem.GetRandomCity();
                }
                while (destination == origin);

                int distance = GetDistance(minister, destination);
                int trades = Utility.RandomMinMax(1, GetMaxTrades(from));
                TradeEntry entry = new TradeEntry(destination, origin, distance);
                TradeOrderCrate crate = new TradeOrderCrate(from, entry);

                GetPlayerEntry<CityTradeEntry>(from as PlayerMobile, true);

                for (int i = 0; i < trades; i++)
                {
                    int worth = 1;
                    string name = null;

                    Type t = GetRandomTrade(origin, destination, ref worth, ref name);
                    int amount = Utility.RandomList(5, 10, 15, 20);

                    entry.Details.Add(new TradeEntry.TradeDetails(t, worth, amount, name));
                }

				if(from.Backpack == null || !from.Backpack.TryDropItem(from, crate, false))
				{
					crate.Delete();
					from.SendLocalizedMessage(114456); // Your backpack cannot hold the Trade Order.  Free up space and speak to the Trade Minister again.
				}
					
				ActiveTrades[from] = crate;
				
				return true;
			}
			
			return false;
		}
		
		public bool TryTurnIn(Mobile from, TradeOrderCrate order, Mobile turninMobile)
		{
            if (order == null || from == null || turninMobile == null || order.Entry == null)
				return false;
				
			TradeEntry entry = order.Entry;
            TradeMinister minister = turninMobile as TradeMinister;

			if(from.AccessLevel == AccessLevel.Player && minister != null && minister.City != entry.Destination)
                turninMobile.SayTo(from, 1151738, String.Format("#{0}", CityLoyaltySystem.GetCityLocalization(entry.Destination))); // Begging thy pardon, but those goods are destined for the City of ~1_city~
			else if(!order.Fulfilled)
                turninMobile.SayTo(from, 1151732); // This trade order has not been fulfilled.  Fill the trade order with all necessary items and try again.
			else
			{
				CityLoyaltySystem.OnTradeComplete(from, order.Entry);
                CityTradeEntry pentry = GetPlayerEntry<CityTradeEntry>(from as PlayerMobile);

                if (pentry != null)
                {
                    pentry.Points++;
                    pentry.DistanceTraveled += entry.Distance;
                    pentry.Completed++;
                    CheckTitle(pentry);
                }
				
				order.Delete();
                return true;
			}

            return false;
		}

        public bool TryTurnInToSlim(Mobile from, TradeOrderCrate order, SlimTheFence slim)
        {
            if (order == null || from == null || slim == null || order.Entry == null)
                return false;

            TradeEntry entry = order.Entry;

            if (!order.Fulfilled)
                slim.SayTo(from, 1151732); // This trade order has not been fulfilled.  Fill the trade order with all necessary items and try again.
            else
            {
                CityLoyaltySystem.OnSlimTradeComplete(from, order.Entry);
                CityTradeEntry pentry = GetPlayerEntry<CityTradeEntry>(from as PlayerMobile, true);

                if (pentry != null)
                {
                    pentry.Points++;
                    pentry.DistanceTraveled += entry.Distance;
                    pentry.CompletedSlim++;
                    CheckTitle(pentry);
                }

                slim.SayTo(from, 1151736); // Haha! These goods will fetch me quite a bit o' coin!  Thanks fer yer help!  Here's a little something I was able to get me hands on...

                order.Delete();
                return true;
            }

            return false;
        }

        public void CheckTitle(CityTradeEntry entry)
        {
            switch (entry.Completed)
            {
                case 1: entry.Player.AddCollectionTitle((int)TradeTitle.Trader); break;
                case 25: entry.Player.AddCollectionTitle((int)TradeTitle.Exporter); break;
                case 50: entry.Player.AddCollectionTitle((int)TradeTitle.Broker); break;
                case 100: entry.Player.AddCollectionTitle((int)TradeTitle.Tycoon); break;
                case 150: entry.Player.AddCollectionTitle((int)TradeTitle.Magnate); break;
            }

            if(entry.CompletedSlim == 50)
                entry.Player.AddCollectionTitle((int)TradeTitle.Smuggler);
        }

        public override void OnPlayerAdded(PlayerMobile m)
        {
            m.Backpack.DropItem(new MysteriousNote());
            m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1150, 1151734, m.NetState); // *A passerby slips a rolled bit of parchment into your hand...*
        }
		
		public static void CancelTradeOrder(Mobile from, TradeOrderCrate crate)
		{
            if (from == null)
                from = crate.Owner;

            if (from != null)
            {
                crate.Items.ForEach(i =>
                {
                    from.Backpack.DropItem(i);
                });

                CityTradeEntry entry = CityLoyaltySystem.CityTrading.GetPlayerEntry<CityTradeEntry>(from as PlayerMobile, true);

                if (entry != null)
                    entry.Canceled++;
            }
			
			crate.Delete();
		}

        private Dictionary<Type, string> _NameBuffer;

        public string GetNameFor(Type t, string fallbackname)
        {
            if (_NameBuffer.ContainsKey(t))
                return _NameBuffer[t];

            Item item = Loot.Construct(t);

            if (item != null)
            {
                string name;

                if (item.Name != null)
                {
                    name = item.Name;
                }
                else
                {
                    name = item.LabelNumber.ToString();
                }

                _NameBuffer[t] = name;
                item.Delete();

                return name;
            }

            Console.WriteLine("WARNING: Using Fallback name for: {0}", t.Name);
            return fallbackname;
        }
		
		public void RemoveCrate(Mobile from, TradeOrderCrate crate)
		{
			if(ActiveTrades.ContainsKey(from))
			{
                ActiveTrades.Remove(from);
			}
		}

        public static void OnPublicMoongateUsed(Mobile from)
        {
            if (ActiveTrades.ContainsKey(from))
            {
                ActiveTrades[from].Entry.Distance = 0;
            }
        }
		
		public static int GetDistance(TradeMinister origin, City destination)
		{
            TradeMinister destMinister = TradeMinister.Ministers.FirstOrDefault(m => m.City == destination);
			
			if(destMinister != null)
			{
				return (int)origin.GetDistanceToSqrt(destMinister.Location);
			}
			
			return 0;
		}

        public static Type GetRandomTrade(City originCity, City dest, ref int worth, ref string name)
        {
            Region region = CityLoyaltySystem.GetCityInstance(originCity).Definition.Region;

            List<BaseVendor> list = new List<BaseVendor>(region.GetEnumeratedMobiles().OfType<BaseVendor>().Where(bv => bv.GetBuyInfo() != null && bv.GetBuyInfo().Length > 0));

            do
            {
                BaseVendor vendor = list[Utility.Random(list.Count)];
                IBuyItemInfo[] buyInfo = vendor.GetBuyInfo();

                GenericBuyInfo info = buyInfo[Utility.Random(buyInfo.Length)] as GenericBuyInfo;

                if (!(info is BeverageBuyInfo) && !(info is AnimalBuyInfo) && info != null && info.Type != null && info.Args == null)
                {
                    list.Clear();
                    list.TrimExcess();

                    worth = info.Price;
                    name = info.Name;

                    return info.Type;
                }
                else
                    list.Remove(vendor);
            }
            while (list.Count > 0);

            list.Clear();
            list.TrimExcess();
            return null;
        }

        public static void OnTick()
        {
            List<TradeOrderCrate> crates = new List<TradeOrderCrate>(ActiveTrades.Values);
            List<BaseCreature> toDelete = new List<BaseCreature>();

            foreach (var c in crates)
            {
                if (c.Expired)
                {
                    CancelTradeOrder(c.Owner, c);
                }
                else if (c.Entry != null)
                {
                    CheckAmbush(c);
                }
            }

            if (Ambushers != null)
            {
                foreach (KeyValuePair<BaseCreature, DateTime> kvp in Ambushers)
                {
                    if (kvp.Value < DateTime.UtcNow)
                        toDelete.Add(kvp.Key);
                }

                toDelete.ForEach(bc =>
                    {
                        if (!bc.Deleted)
                            bc.Delete();

                        Ambushers.Remove(bc);
                    });
            }

            toDelete.Clear();
            toDelete.TrimExcess();
            crates.Clear();
            crates.TrimExcess();
        }

        public static void CheckAmbush(TradeOrderCrate crate)
        {
            if (crate == null || crate.Deleted || crate.Entry == null || crate.Expired || crate.Entry.LastAmbush + TimeSpan.FromMinutes(AmbushWaitDuration) > DateTime.UtcNow)
                return;

            if (crate.RootParentEntity is Mobile && !((Mobile)crate.RootParentEntity).Region.IsPartOf(typeof(GuardedRegion)))
            {
                Mobile m = crate.RootParentEntity as Mobile;

                if (m.NetState != null && m.Map != null && m.Map != Map.Internal)
                {
                    double chance = crate.Entry.LastAmbush == DateTime.MinValue ? 0.25 : .05;

                    if (chance > Utility.RandomDouble())
                    {
                        double dif = (double)(Math.Min(7200, m.SkillsTotal) + m.RawStr + m.RawInt + m.RawDex) / 10000;

                        m.RevealingAction();

                        SpawnCreatures(m, dif);
                        crate.Entry.LastAmbush = DateTime.UtcNow;
                    }
                }
            }
        }

        public static void SpawnCreatures(Mobile m, double difficulty)
        {
            BaseBoat boat = BaseBoat.FindBoatAt(m.Location, m.Map);

            Type[] types = boat != null ? _SeaTypes : _LandTypes;
            int amount = Utility.RandomMinMax(2, 4);

            for (int i = 0; i < amount; i++)
            {
                BaseCreature bc = Activator.CreateInstance(types[Utility.Random(types.Length)]) as BaseCreature;

                if (bc != null)
                {
                    Rectangle2D zone;

                    if (boat != null)
                    {
                        if (boat.Facing == Direction.North || boat.Facing == Direction.South)
                        {
                            if (Utility.RandomBool())
                            {
                                zone = new Rectangle2D(boat.X - 7, m.Y - 4, 3, 3);
                            }
                            else
                            {
                                zone = new Rectangle2D(boat.X + 4, m.Y - 4, 3, 3);
                            }
                        }
                        else
                        {
                            if (Utility.RandomBool())
                            {
                                zone = new Rectangle2D(m.X + 4, boat.Y - 7, 3, 3);
                            }
                            else
                            {
                                zone = new Rectangle2D(m.X + 4, boat.Y + 4, 3, 3);
                            }
                        }
                    }
                    else
                    {
                        zone = new Rectangle2D(m.X - 3, m.Y - 3, 6, 6);
                    }

                    Point3D p = m.Location;

                    for (int j = 0; j < 25; j++)
                    {
                        Point3D check = zone.GetRandomSpawnPoint(m.Map);

                        if (CanFit(check.X, check.Y, check.Z, m.Map, bc))
                        {
                            p = check;
                            break;
                        }
                    }

                    foreach (Skill sk in bc.Skills.Where(s => s.Base > 0))
                    {
                        sk.Base += sk.Base * (difficulty);
                    }

                    bc.RawStr += (int)(bc.RawStr * difficulty);
                    bc.RawInt += (int)(bc.RawInt * difficulty);
                    bc.RawDex += (int)(bc.RawDex * difficulty);

                    bc.HitsMaxSeed += (int)(bc.HitsMaxSeed * difficulty);
                    bc.StamMaxSeed += (int)(bc.StamMaxSeed * difficulty);
                    bc.ManaMaxSeed += (int)(bc.ManaMaxSeed * difficulty);

                    bc.Hits = bc.HitsMaxSeed;
                    bc.Stam = bc.RawDex;
                    bc.Mana = bc.RawInt;

                    bc.PhysicalResistanceSeed += (int)(bc.PhysicalResistanceSeed * (difficulty / 3));
                    bc.FireResistSeed += (int)(bc.FireResistSeed * (difficulty / 3));
                    bc.ColdResistSeed += (int)(bc.ColdResistSeed * (difficulty / 3));
                    bc.PoisonResistSeed += (int)(bc.PoisonResistSeed * (difficulty / 3));
                    bc.EnergyResistSeed += (int)(bc.EnergyResistSeed * (difficulty / 3));

                    bc.IsAmbusher = true;

                    if (Ambushers == null)
                        Ambushers = new Dictionary<BaseCreature, DateTime>();

                    Ambushers.Add(bc, DateTime.UtcNow + TimeSpan.FromMinutes(AmbusherDelete));

                    bc.MoveToWorld(p, m.Map);
                    Timer.DelayCall(() => bc.Combatant = m);

                    m.SendLocalizedMessage(1049330, "", 0x22); // You have been ambushed! Fight for your honor!!!
                }
            }
        }

        public override void ProcessKill(BaseCreature victim, Mobile damager, int index)
        {
            if (Ambushers != null && Ambushers.ContainsKey(victim))
            {
                if (ActiveTrades.ContainsKey(damager))
                {
                    TradeOrderCrate crate = ActiveTrades[damager];

                    if (crate.Entry != null)
                        crate.Entry.Kills++;
                }

                Ambushers.Remove(victim);
            }
        }

        private static Type[] _SeaTypes =
        {
            typeof(SeaSerpent), typeof(DeepSeaSerpent), typeof(Kraken), typeof(WaterElemental)
        };

        private static Type[] _LandTypes =
        {
            typeof(Troll), typeof(Ettin), typeof(GiantSpider), typeof(Brigand)
        };

        public static bool CanFit(int x, int y, int z, Map map, Mobile mob, int height = 16, bool checkMobiles = true, bool requireSurface = true)
        {
            if (map == null || map == Map.Internal)
                return false;

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                return false;

            bool hasSurface = false;
            bool canswim = mob.CanSwim;
            bool cantwalk = mob.CantWalk;

            LandTile lt = map.Tiles.GetLandTile(x, y);
            int lowZ = 0, avgZ = 0, topZ = 0;

            bool surface, impassable;

            map.GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);
            TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

            impassable = (landFlags & TileFlag.Impassable) != 0;

            bool wet = (landFlags & TileFlag.Wet) != 0;

            if (cantwalk && !wet)
            {
                impassable = true;
            }

            if (canswim && wet)
            {
                impassable = false;
            }

            if (impassable && avgZ > z && (z + height) > lowZ)
                return false;
            else if (!impassable && z == avgZ && !lt.Ignored)
                hasSurface = true;

            StaticTile[] staticTiles = map.Tiles.GetStaticTiles(x, y, true);

            for (int i = 0; i < staticTiles.Length; ++i)
            {
                ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];
                surface = id.Surface;
                impassable = id.Impassable;

                wet = (id.Flags & TileFlag.Wet) != 0;
                
                if (cantwalk && !wet)
                {
                    impassable = true;
                }
                if (canswim && wet)
                {
                    surface = true;
                    impassable = false;
                }

                if ((surface || impassable) && (staticTiles[i].Z + id.CalcHeight) > z && (z + height) > staticTiles[i].Z)
                    return false;
                else if (surface && !impassable && z == (staticTiles[i].Z + id.CalcHeight))
                    hasSurface = true;
            }

            IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, z), 0);

            foreach(Item item in eable)
            {
                if (item.ItemID < 0x4000)
                {
                    ItemData id = item.ItemData;
                    surface = id.Surface;
                    impassable = id.Impassable;

                    wet = (id.Flags & TileFlag.Wet) != 0;
                    if (cantwalk && !wet)
                    {
                        impassable = true;
                    }
                    if (canswim && wet)
                    {
                        surface = true;
                        impassable = false;
                    }

                    if ((surface || impassable) && (item.Z + id.CalcHeight) > z && (z + height) > item.Z)
                    {
                        eable.Free();
                        return false;
                    }
                    else if (surface && !impassable && !item.Movable && z == (item.Z + id.CalcHeight))
                    {
                        hasSurface = true;
                    }
                }
            }

            eable.Free();

            if (checkMobiles)
            {
                eable = map.GetMobilesInRange(new Point3D(x, y, z), 0);

                foreach(Mobile m in eable)
                {
                    if (m.AccessLevel == AccessLevel.Player || !m.Hidden)
                    {
                        if ((m.Z + 16) > z && (z + height) > m.Z)
                        {
                            eable.Free();
                            return false;
                        }
                    }
                }

                eable.Free();
            }

            return !requireSurface || hasSurface;
        }

        [PropertyObject]
		public class CityTradeEntry : PointsEntry
		{
            [CommandProperty(AccessLevel.GameMaster)]
			public int Canceled { get; set; }

            [CommandProperty(AccessLevel.GameMaster)]
			public int DistanceTraveled { get; set; }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Completed { get; set; }

            [CommandProperty(AccessLevel.GameMaster)]
            public int CompletedSlim { get; set; }
			
			public CityTradeEntry(PlayerMobile pm) : base(pm)
			{
			}
			
			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);
				writer.Write(0);
				
				writer.Write(Canceled);
				writer.Write(DistanceTraveled);
                writer.Write(Completed);

                writer.Write(Ambushers == null ? 0 : Ambushers.Count);
                if (Ambushers != null)
                {
                    foreach (KeyValuePair<BaseCreature, DateTime> kvp in Ambushers)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }
                }
			}
			
			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);
				int version = reader.ReadInt();
				
				Canceled = reader.ReadInt();
				DistanceTraveled = reader.ReadInt();
                Completed = reader.ReadInt();

                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    BaseCreature bc = reader.ReadMobile() as BaseCreature;
                    DateTime dt = reader.ReadDateTime();

                    if (bc != null)
                    {
                        if (dt < DateTime.UtcNow)
                            bc.Delete();
                        else
                        {
                            if (Ambushers == null)
                                Ambushers = new Dictionary<BaseCreature, DateTime>();

                            bc.IsAmbusher = true;

                            Ambushers[bc] = dt;
                        }
                    }
                }
			}
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write(ActiveTrades.Count);
            foreach (KeyValuePair<Mobile, TradeOrderCrate> kvp in ActiveTrades)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(_NameBuffer.Count);
            foreach (KeyValuePair<Type, string> kvp in _NameBuffer)
            {
                writer.Write(kvp.Key.Name);
                writer.Write(kvp.Value);
            }
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                TradeOrderCrate crate = reader.ReadItem() as TradeOrderCrate;

                if (m != null && crate != null)
                    ActiveTrades[m] = crate;
            }

            _NameBuffer = new Dictionary<Type, string>();

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Type t = ScriptCompiler.FindTypeByName(reader.ReadString());
                string name = reader.ReadString();

                if (t != null)
                    _NameBuffer[t] = name;
            }
		}
	}
}