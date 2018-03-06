using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Commands;
using Server.Engines.Quests;
using Server.Spells;
using System.Linq;

namespace Server.Regions
{
    public class SeaMarketRegion : GuardedRegion
    {
        private static readonly TimeSpan KickDuration = TimeSpan.FromMinutes(30);

        private static SeaMarketRegion m_Region1;
        private static SeaMarketRegion m_Region2;

        public static void SetRegions(SeaMarketRegion reg1, SeaMarketRegion reg2)
        {
            m_Region1 = reg1;
            m_Region2 = reg2;
        }

        private Timer m_Timer;

        private static Timer m_BlabTimer;
        private static bool m_RestrictBoats;

        private Dictionary<BaseBoat, DateTime> m_BoatTable = new Dictionary<BaseBoat, DateTime>();
        public Dictionary<BaseBoat, DateTime> BoatTable { get { return m_BoatTable; } }

        public static bool RestrictBoats
        {
            get { return m_RestrictBoats; }
            set
            {
                m_RestrictBoats = value;

                if (value)
                {
                    if (m_Region1 != null)
                        m_Region1.StartTimer();

                    if (m_Region2 != null)
                        m_Region2.StartTimer();
                }
                else
                {
                    if (m_Region1 != null)
                        m_Region1.StopTimer();

                    if (m_Region2 != null)
                        m_Region2.StopTimer();
                }
            }
        }

        public static Rectangle2D[] Bounds { get { return m_Bounds; } }
        private static Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(4472, 2250, 200, 200),
        };

        public SeaMarketRegion(Map map)
            : base("Sea Market", map, Region.DefaultPriority, m_Bounds)
        {
            GoLocation = new Point3D(4550, 2317, -2);
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            switch (type)
            {
                case TravelCheckType.RecallTo:
                case TravelCheckType.GateTo:
                case TravelCheckType.Mark: return false;
            }

            return base.CheckTravel(traveller, p, type);
        }

        #region Pirate Blabbing
        public static Dictionary<Mobile, DateTime> m_PirateBlabTable = new Dictionary<Mobile, DateTime>();
        private static readonly TimeSpan BlabDuration = TimeSpan.FromSeconds(60);

        public static void TryPirateBlab(Mobile from, Mobile npc)
        {
            if (m_PirateBlabTable.ContainsKey(from) && m_PirateBlabTable[from] > DateTime.UtcNow || BountyQuestSpawner.Bounties.Count <= 0)
                return;

            //Make of list of bounties on their map
            List<Mobile> bounties = new List<Mobile>();
            foreach (Mobile mob in BountyQuestSpawner.Bounties.Keys)
            {
                if (mob.Map == from.Map && mob is PirateCaptain && !bounties.Contains(mob))
                    bounties.Add(mob);
            }

            if (bounties.Count > 0)
            {
                Mobile bounty = bounties[Utility.Random(bounties.Count)];

                if (bounty != null)
                {
                    PirateCaptain capt = (PirateCaptain)bounty;

                    int xLong = 0, yLat = 0;
                    int xMins = 0, yMins = 0;
                    bool xEast = false, ySouth = false;
                    Point3D loc = capt.Location;
                    Map map = capt.Map;

                    string locArgs;
                    //string nameArgs;
                    string combine;

                    if (Sextant.Format(loc, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                        locArgs = String.Format("{0}°{1}'{2},{3}°{4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                    else
                        locArgs = "?????";

                    combine = String.Format("{0}\t{1}", capt.PirateName > -1 ? String.Format("#{0}", capt.PirateName) : capt.Name, locArgs);

                    int cliloc = Utility.RandomMinMax(1149856, 1149865);
                    npc.SayTo(from, cliloc, combine);

                    m_PirateBlabTable[from] = DateTime.UtcNow + BlabDuration;
                }
            }

            ColUtility.Free(bounties);
        }

        public static void CheckBlab_Callback()
        {
            CheckBabble(m_Region1);
            CheckBabble(m_Region2);
            CheckBabble(TokunoDocksRegion.Instance);
        }

        public static void CheckBabble(Region r)
        {
            if (r == null)
                return;

            foreach (var player in r.GetEnumeratedMobiles().Where(p => p is PlayerMobile && 
                                                                       p.Alive &&
                                                                       QuestHelper.GetQuest((PlayerMobile)p, typeof(ProfessionalBountyQuest)) != null))
            {
                    IPooledEnumerable eable = player.GetMobilesInRange(4);

                    foreach (Mobile mob in eable)
                    {
                        if (mob is BaseVendor || mob is MondainQuester || mob is GalleonPilot)
                        {
                            TryPirateBlab(player, mob);
                            break;
                        }
                    }
                    eable.Free();
            }
        }
        #endregion

        #region Boat Restriction
        public void StartTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public List<BaseBoat> GetBoats()
        {
            List<BaseBoat> list = new List<BaseBoat>();

            foreach (BaseBoat boat in this.GetEnumeratedMultis().OfType<BaseBoat>())
                list.Add(boat);

            return list;
        }

        public void OnTick()
        {
            if (!m_RestrictBoats)
            {
                StopTimer();
                return;
            }

            List<BaseBoat> boats = GetBoats();
            List<BaseBoat> toRemove = new List<BaseBoat>();

            foreach (KeyValuePair<BaseBoat, DateTime> kvp in m_BoatTable)
            {
                BaseBoat boat = kvp.Key;
                DateTime moveBy = kvp.Value;

                if (boat == null || !boats.Contains(boat) || boat.Deleted)
                    toRemove.Add(boat);
                else if (DateTime.UtcNow >= moveBy && KickBoat(boat))
                    toRemove.Add(boat);
                else
                {
                    if (boat.Owner != null && boat.Owner.NetState != null)
                    {
                        TimeSpan ts = moveBy - DateTime.UtcNow;

                        if ((int)ts.TotalMinutes <= 10)
                        {
                            int rem = Math.Max(1, (int)ts.TotalMinutes);
                            boat.Owner.SendLocalizedMessage(1149787 + (rem - 1));
                        }
                    }
                }
            }

            foreach (BaseBoat boat in boats)
            {
                if (!m_BoatTable.ContainsKey(boat) && !boat.IsMoving && boat.Owner != null && boat.Owner.AccessLevel == AccessLevel.Player)
                    AddToTable(boat);
            }

            foreach (BaseBoat b in toRemove)
                m_BoatTable.Remove(b);

            ColUtility.Free(toRemove);
            ColUtility.Free(boats);
        }

        public void AddToTable(BaseBoat boat)
        {
            if (m_BoatTable.ContainsKey(boat))
                return;

            m_BoatTable.Add(boat, DateTime.UtcNow + KickDuration);

            if (boat.Owner != null && boat.Owner.NetState != null)
                boat.Owner.SendMessage("You can only dock your boat here for {0} minutes.", (int)KickDuration.TotalMinutes);
        }

        private Rectangle2D[] m_KickLocs = new Rectangle2D[]
        {
	        new Rectangle2D(m_Bounds[0].X - 100, m_Bounds[0].X - 100, 200 + m_Bounds[0].Width, 100),
	        new Rectangle2D(m_Bounds[0].X - 100, m_Bounds[0].Y, 100, m_Bounds[0].Height + 100),
	        new Rectangle2D(m_Bounds[0].X, m_Bounds[0].Y + m_Bounds[0].Height, m_Bounds[0].Width + 100, 100),
	        new Rectangle2D(m_Bounds[0].X + m_Bounds[0].Width, m_Bounds[0].Y, 100, m_Bounds[0].Height),
        };

        public bool KickBoat(BaseBoat boat)
        {
            if (boat == null || boat.Deleted)
                return false;

            for (int i = 0; i < 25; i++)
            {
                Rectangle2D rec = m_KickLocs[Utility.Random(m_KickLocs.Length)];

                int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
                int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
                int z = boat.Z;

                Point3D p = new Point3D(x, y, z);

                if (boat.CanFit(p, boat.Map, boat.ItemID))
                {
                    boat.Teleport(x - boat.X, y - boat.Y, z - boat.Z);

                    if (boat.Owner != null && boat.Owner.NetState != null)
                        boat.SendMessageToAllOnBoard(1149785); //A strong tide comes and carries your boat to deeper water.
                    return true;
                }
            }
            return false;
        }

        private class InternalTimer : Timer
        {
            private SeaMarketRegion m_Region;

            public InternalTimer(SeaMarketRegion reg)
                : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
            {
                m_Region = reg;
            }

            protected override void OnTick()
            {
                if (m_Region != null)
                    m_Region.OnTick();
            }
        }

        public static void StartTimers_Callback()
        {
            RestrictBoats = m_RestrictBoats;

            m_BlabTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerCallback(CheckBlab_Callback));
            m_BlabTimer.Priority = TimerPriority.OneSecond;
        }
        #endregion

        public static void Save(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(m_RestrictBoats);
        }

        public static void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_RestrictBoats = reader.ReadBool();

            Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerCallback(StartTimers_Callback));
        }

        public static void GetBoatInfo_OnCommand(CommandEventArgs e)
        {
            List<BaseBoat> boats = new List<BaseBoat>(m_Region1.BoatTable.Keys);
            List<DateTime> times = new List<DateTime>(m_Region1.BoatTable.Values);

            e.Mobile.SendMessage("========Boat Info for Felucca as Follows===========");
            e.Mobile.SendMessage("Boats: {0}", boats.Count);

            if (!m_RestrictBoats)
                e.Mobile.SendMessage("Boat restriction is currenlty disabled.");

            Console.WriteLine("========Boat Info as Follows===========");
            Console.WriteLine("Boats: {0}", boats.Count);

            if (!m_RestrictBoats)
                Console.WriteLine("Boat restriction is currenlty disabled.");

            for (int i = 0; i < boats.Count; i++)
            {
                BaseBoat boat = boats[i];

                if (boat == null || boat.Deleted)
                    continue;

                e.Mobile.SendMessage("Boat Name: {0}; Boat Owner: {1}; Expires: {2}", boat.ShipName, boat.Owner, times[i]);

                Console.WriteLine("Boat Name: {0}; Boat Owner: {1}; Expires: {2}", boat.ShipName, boat.Owner, times[i]);
            }

            boats.Clear();
            times.Clear();

            boats = new List<BaseBoat>(m_Region2.BoatTable.Keys);
            times = new List<DateTime>(m_Region2.BoatTable.Values);

            e.Mobile.SendMessage("========Boat Info for Trammel as Follows===========");
            e.Mobile.SendMessage("Boats: {0}", boats.Count);

            if (!m_RestrictBoats)
                e.Mobile.SendMessage("Boat restriction is currenlty disabled.");

            Console.WriteLine("========Boat Info as Follows===========");
            Console.WriteLine("Boats: {0}", boats.Count);

            if (!m_RestrictBoats)
                Console.WriteLine("Boat restriction is currenlty disabled.");

            for (int i = 0; i < boats.Count; i++)
            {
                BaseBoat boat = boats[i];

                if (boat == null || boat.Deleted)
                    continue;

                e.Mobile.SendMessage("Boat Name: {0}; Boat Owner: {1}; Expires: {2}", boat.ShipName, boat.Owner, times[i]);

                Console.WriteLine("Boat Name: {0}; Boat Owner: {1}; Expires: {2}", boat.ShipName, boat.Owner, times[i]);
            }
        }

        public static void SetRestriction_OnCommand(CommandEventArgs e)
        {
            if (m_RestrictBoats)
            {
                RestrictBoats = false;

                e.Mobile.SendMessage("Boat restriction in the sea market is no longer active.");
            }
            else
            {
                RestrictBoats = true;

                e.Mobile.SendMessage("Boat restriction in the sea market is now active.");
            }
        }
    }
}