using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.HuntsmasterChallenge
{
    public class HuntingSystem : Item
    {
        private static HuntingSystem m_Instance;
        public static HuntingSystem Instance => m_Instance;

        private DateTime m_SeasonBegins;
        private DateTime m_SeasonEnds;
        private DateTime m_NextHint;
        private DateTime m_NextBonusIndex;
        private int m_BonusIndex;
        private bool m_Active;

        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SeasonBegins { get { return m_SeasonBegins; } set { m_SeasonBegins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SeasonEnds { get { return m_SeasonEnds; } set { m_SeasonEnds = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BonusIndex => m_BonusIndex;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return m_Active; } set { m_Active = value; CheckTimer(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool NewSeason
        {
            get { return false; }
            set
            {
                EndSeason();
            }
        }

        public static void Initialize()
        {
            if (m_Instance == null)
                m_Instance = new HuntingSystem();
        }

        public HuntingSystem() : base(17603)
        {
            if (m_Instance != null)
            {
                Delete();
                return;
            }

            m_Instance = this;
            m_Active = true;

            m_Top10 = new Dictionary<HuntType, List<HuntingKillEntry>>();
            m_Leaders = new Dictionary<HuntType, List<HuntingKillEntry>>();

            m_SeasonBegins = DateTime.Now;
            DateTime ends = DateTime.Now + TimeSpan.FromDays(30);
            m_SeasonEnds = new DateTime(ends.Year, ends.Month, 1, 0, 0, 0);
            m_NextHint = DateTime.UtcNow;
            m_NextBonusIndex = DateTime.UtcNow;

            CheckTimer();

            Timer.DelayCall(TimeSpan.FromSeconds(1), Setup);

            Movable = false;
            Visible = false;
            Name = "Huntsmaster Challenge System";

            MoveToWorld(new Point3D(744, 2136, 0), Map.Trammel);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.AccessLevel > AccessLevel.Player)
                m.SendGump(new PropertiesGump(m, this));
        }

        private void CheckTimer()
        {
            if (m_Active)
            {
                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);
            }
            else
            {
                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }
            }
        }

        public void OnTick()
        {
            if (DateTime.Now >= m_SeasonEnds)
            {
                EndSeason();
            }

            if (m_NextBonusIndex < DateTime.UtcNow)
            {
                m_BonusIndex = Utility.Random(WorldLocationInfo.Locations[0].Length);
                m_NextBonusIndex = DateTime.UtcNow + TimeSpan.FromHours(6);
            }

            if (m_NextHint < DateTime.UtcNow)
            {
                CheckHint(Map.Trammel);
                CheckHint(Map.Felucca);
            }
        }

        private void CheckHint(Map map)
        {
            IPooledEnumerable eable = map.GetMobilesInBounds(new Rectangle2D(735, 2135, 24, 24));

            foreach (Mobile m in eable)
            {
                if (m is BaseVendor)
                {
                    IPooledEnumerable players = map.GetMobilesInRange(m.Location, 3);

                    foreach (Mobile player in players)
                    {
                        if (player is PlayerMobile && ((PlayerMobile)player).NpcGuild == NpcGuild.RangersGuild && !player.Hidden)
                        {
                            GiveHint(player, m);
                            eable.Free();
                            players.Free();
                            return;
                        }
                    }

                    players.Free();
                }
            }

            eable.Free();
        }

        private void GiveHint(Mobile player, Mobile vendor)
        {
            vendor.SayTo(player, "A good place to hunt these days is {0}.", WorldLocationInfo.Locations[0][m_BonusIndex].RegionName);

            m_NextHint = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(30, 60));
        }

        public bool IsPrimeHunt(Mobile from, Point3D p)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null || pm.NpcGuild != NpcGuild.RangersGuild)
                return false;

            WorldLocationInfo info = WorldLocationInfo.Locations[0][m_BonusIndex];

            if (info != null)
            {
                foreach (Rectangle2D rec in info.Bounds)
                {
                    if (rec.Contains(p))
                        return true;
                }
            }

            return false;
        }

        public void TrySubmitKill(HuntMaster master, Mobile from, HuntingPermit permit)
        {
            if (permit.KillEntry == null || permit.KillEntry.KillIndex < 0 || permit.KillEntry.KillIndex > HuntingTrophyInfo.Infos.Count)
                master.SayTo(from, 1155706); // That is not a valid kill.
            else
            {
                HuntingTrophyInfo info = HuntingTrophyInfo.Infos[permit.KillEntry.KillIndex];

                if (info != null)
                {
                    if (!m_Leaders.ContainsKey(info.HuntType))
                        m_Leaders[info.HuntType] = new List<HuntingKillEntry>();

                    List<HuntingKillEntry> leaders = m_Leaders[info.HuntType];

                    if (leaders.Count == 0 || permit.KillEntry.Measurement >= leaders[0].Measurement)
                    {
                        if (leaders.Count > 0 && permit.KillEntry.Measurement > leaders[0].Measurement)
                            leaders.Clear();

                        leaders.Add(new HuntingKillEntry(permit.Owner, permit.KillEntry.Measurement, permit.KillEntry.DateKilled, permit.KillEntry.KillIndex, permit.KillEntry.Location));

                        from.SendGump(new BasicInfoGump(1155722));

                        HuntingDisplayTrophy.InvalidateDisplayTrophies();
                        master.PlaySound(0x3D);
                    }
                    else
                        master.SayTo(from, 1155721); // Begging thy pardon, but your permit has not broken the current record for this species!

                    permit.HasSubmitted = true;

                    CheckKill(info.HuntType, permit.KillEntry);

                    if (from is PlayerMobile)
                    {
                        BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)from, typeof(HuntmastersChallengeQuest));

                        if (quest != null && quest is HuntmastersChallengeQuest)
                        {
                            ((HuntmastersChallengeQuest)quest).CompleteChallenge();
                        }
                    }
                }
            }
        }

        private void CheckKill(HuntType type, HuntingKillEntry entry)
        {
            if (!m_Top10.ContainsKey(type))
                m_Top10[type] = new List<HuntingKillEntry>();

            if (m_Top10[type].Count < 10)
                m_Top10[type].Add(entry);
            else
            {
                List<HuntingKillEntry> copy = new List<HuntingKillEntry>(m_Top10[type]);
                copy.Sort();

                for (int i = 0; i < copy.Count; i++)
                {
                    if (entry.Measurement > copy[i].Measurement)
                    {
                        m_Top10[type].Remove(copy[i]);
                        m_Top10[type].Add(entry);
                        break;
                    }
                }
            }
        }

        public void EndSeason()
        {
            foreach (KeyValuePair<HuntType, List<HuntingKillEntry>> kvp in m_Leaders)
            {
                foreach (HuntingKillEntry killEntry in kvp.Value)
                {
                    Mobile owner = killEntry.Owner;

                    if (owner != null)
                    {
                        if (!m_UnclaimedWinners.ContainsKey(owner))
                            m_UnclaimedWinners[owner] = 1;
                        else
                            m_UnclaimedWinners[owner]++;
                    }
                }
            }

            m_Leaders.Clear();

            DateTime now = DateTime.Now;
            DateTime ends = DateTime.Now + TimeSpan.FromDays(32);

            m_SeasonEnds = new DateTime(ends.Year, ends.Month, 1, 0, 0, 0);
            m_SeasonBegins = new DateTime(now.Year, now.Month, 1, 0, 0, 0);

            HuntingDisplayTrophy.InvalidateDisplayTrophies();
        }

        public bool CheckUnclaimedEntry(Mobile from, Mobile vendor)
        {
            List<Mobile> copy = new List<Mobile>(m_UnclaimedWinners.Keys);

            foreach (Mobile m in copy)
            {
                if (m == from && m is PlayerMobile)
                {
                    m.SendGump(new HuntmasterRewardGump(vendor, (PlayerMobile)m));
                    return true;
                }
            }

            return false;
        }

        public HuntingSystem(Serial serial) : base(serial)
        {
        }

        private Dictionary<HuntType, List<HuntingKillEntry>> m_Leaders;
        public Dictionary<HuntType, List<HuntingKillEntry>> Leaders => m_Leaders;

        private readonly Dictionary<Mobile, int> m_UnclaimedWinners = new Dictionary<Mobile, int>();
        public Dictionary<Mobile, int> UnclaimedWinners => m_UnclaimedWinners;

        private Dictionary<HuntType, List<HuntingKillEntry>> m_Top10;
        public Dictionary<HuntType, List<HuntingKillEntry>> Top10 => m_Top10;

        public override void Delete()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(m_Active);
            writer.Write(m_SeasonBegins);
            writer.Write(m_SeasonEnds);

            writer.Write(m_UnclaimedWinners.Count);
            foreach (KeyValuePair<Mobile, int> kvp in m_UnclaimedWinners)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(m_Top10.Count);
            foreach (KeyValuePair<HuntType, List<HuntingKillEntry>> kvp in m_Top10)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value.Count);

                foreach (HuntingKillEntry entry in kvp.Value)
                    entry.Serialize(writer);
            }

            writer.Write(m_Leaders.Count);
            foreach (KeyValuePair<HuntType, List<HuntingKillEntry>> kvp in m_Leaders)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value.Count);

                foreach (HuntingKillEntry entry in kvp.Value)
                    entry.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_Active = reader.ReadBool();
            m_SeasonBegins = reader.ReadDateTime();
            m_SeasonEnds = reader.ReadDateTime();

            m_Top10 = new Dictionary<HuntType, List<HuntingKillEntry>>();
            m_Leaders = new Dictionary<HuntType, List<HuntingKillEntry>>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                int c = 1;

                if (v == 0)
                {
                    new HuntingKillEntry(reader);
                }
                else
                {
                    c = reader.ReadInt();
                }

                if (m != null)
                    m_UnclaimedWinners[m] = c;
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                HuntType type = (HuntType)reader.ReadInt();
                int c = reader.ReadInt();

                if (!m_Top10.ContainsKey(type))
                    m_Top10[type] = new List<HuntingKillEntry>();

                for (int j = 0; j < c; j++)
                {
                    m_Top10[type].Add(new HuntingKillEntry(reader));
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                HuntType type = (HuntType)reader.ReadInt();
                int c = reader.ReadInt();

                if (!m_Leaders.ContainsKey(type))
                    m_Leaders[type] = new List<HuntingKillEntry>();

                for (int j = 0; j < c; j++)
                {
                    m_Leaders[type].Add(new HuntingKillEntry(reader));
                }
            }

            m_Instance = this;
            m_NextHint = DateTime.UtcNow;
            m_NextBonusIndex = DateTime.UtcNow;

            CheckTimer();
        }

        private void Setup()
        {
            Static s;

            for (int i = 0; i < 9; i++)
            {
                s = new Static(2931);
                s.MoveToWorld(new Point3D(748 + i, 2136, 0), Map.Trammel);

                s = new Static(2928);
                s.MoveToWorld(new Point3D(748 + i, 2137, 0), Map.Trammel);

                s = new Static(2931);
                s.MoveToWorld(new Point3D(748 + i, 2136, 0), Map.Felucca);

                s = new Static(2928);
                s.MoveToWorld(new Point3D(748 + i, 2137, 0), Map.Felucca);
            }

            s = new Static(2923);
            s.MoveToWorld(new Point3D(736, 2150, 0), Map.Trammel);

            s = new Static(2925);
            s.MoveToWorld(new Point3D(736, 2149, 0), Map.Trammel);

            s = new Static(2924);
            s.MoveToWorld(new Point3D(736, 2148, 0), Map.Trammel);

            s = new Static(2923);
            s.MoveToWorld(new Point3D(736, 2146, 0), Map.Trammel);

            s = new Static(2925);
            s.MoveToWorld(new Point3D(736, 2145, 0), Map.Trammel);

            s = new Static(2924);
            s.MoveToWorld(new Point3D(736, 2144, 0), Map.Trammel);

            s = new Static(2923);
            s.MoveToWorld(new Point3D(736, 2150, 0), Map.Felucca);

            s = new Static(2925);
            s.MoveToWorld(new Point3D(736, 2149, 0), Map.Felucca);

            s = new Static(2924);
            s.MoveToWorld(new Point3D(736, 2148, 0), Map.Felucca);

            s = new Static(2923);
            s.MoveToWorld(new Point3D(736, 2146, 0), Map.Felucca);

            s = new Static(2925);
            s.MoveToWorld(new Point3D(736, 2145, 0), Map.Felucca);

            s = new Static(2924);
            s.MoveToWorld(new Point3D(736, 2144, 0), Map.Felucca);

            HuntingDisplayTrophy trophy = new HuntingDisplayTrophy(HuntType.GrizzlyBear);
            trophy.MoveToWorld(new Point3D(748, 2137, 6), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.GrizzlyBear);
            trophy.MoveToWorld(new Point3D(748, 2137, 6), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.GrayWolf);
            trophy.MoveToWorld(new Point3D(751, 2137, 6), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.GrayWolf);
            trophy.MoveToWorld(new Point3D(751, 2137, 6), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Cougar);
            trophy.MoveToWorld(new Point3D(753, 2137, 6), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Cougar);
            trophy.MoveToWorld(new Point3D(753, 2137, 6), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Turkey);
            trophy.MoveToWorld(new Point3D(756, 2137, 6), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Turkey);
            trophy.MoveToWorld(new Point3D(756, 2137, 6), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Bull);
            trophy.MoveToWorld(new Point3D(748, 2136, 2), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Bull);
            trophy.MoveToWorld(new Point3D(748, 2136, 2), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Boar);
            trophy.MoveToWorld(new Point3D(750, 2136, 2), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Boar);
            trophy.MoveToWorld(new Point3D(750, 2136, 2), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Walrus);
            trophy.MoveToWorld(new Point3D(752, 2136, 2), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Walrus);
            trophy.MoveToWorld(new Point3D(752, 2136, 2), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Alligator);
            trophy.MoveToWorld(new Point3D(754, 2136, 2), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Alligator);
            trophy.MoveToWorld(new Point3D(754, 2136, 2), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Eagle);
            trophy.MoveToWorld(new Point3D(756, 2136, 3), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Eagle);
            trophy.MoveToWorld(new Point3D(756, 2136, 3), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Saurosaurus);
            trophy.MoveToWorld(new Point3D(746, 2136, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Saurosaurus);
            trophy.MoveToWorld(new Point3D(746, 2136, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Anchisaur);
            trophy.MoveToWorld(new Point3D(744, 2136, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Anchisaur);
            trophy.MoveToWorld(new Point3D(744, 2136, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.BlackTiger);
            trophy.MoveToWorld(new Point3D(744, 2138, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.BlackTiger);
            trophy.MoveToWorld(new Point3D(744, 2138, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.WhiteTiger);
            trophy.MoveToWorld(new Point3D(744, 2140, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.WhiteTiger);
            trophy.MoveToWorld(new Point3D(744, 2140, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Triceratops);
            trophy.MoveToWorld(new Point3D(744, 2142, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Triceratops);
            trophy.MoveToWorld(new Point3D(744, 2142, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Allosaurus);
            trophy.MoveToWorld(new Point3D(743, 2144, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Allosaurus);
            trophy.MoveToWorld(new Point3D(743, 2144, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.MyrmidexDrone);
            trophy.MoveToWorld(new Point3D(741, 2144, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.MyrmidexDrone);
            trophy.MoveToWorld(new Point3D(741, 2144, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Dimetrosaur);
            trophy.MoveToWorld(new Point3D(758, 2136, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Dimetrosaur);
            trophy.MoveToWorld(new Point3D(758, 2136, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Tiger);
            trophy.MoveToWorld(new Point3D(738, 2144, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Tiger);
            trophy.MoveToWorld(new Point3D(738, 2144, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Najasaurus);
            trophy.MoveToWorld(new Point3D(736, 2145, 6), Map.Trammel);

            s = new Static(0x9C03);
            s.MoveToWorld(new Point3D(736, 2144, 6), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Najasaurus);
            trophy.MoveToWorld(new Point3D(736, 2145, 6), Map.Felucca);

            s = new Static(0x9C03);
            s.MoveToWorld(new Point3D(736, 2144, 6), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.Lion);
            trophy.MoveToWorld(new Point3D(736, 2147, 0), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.Lion);
            trophy.MoveToWorld(new Point3D(736, 2147, 0), Map.Felucca);

            trophy = new HuntingDisplayTrophy(HuntType.MyrmidexLarvae);
            trophy.MoveToWorld(new Point3D(736, 2149, 6), Map.Trammel);

            s = new Static(0x9C01);
            s.MoveToWorld(new Point3D(736, 2149, 6), Map.Trammel);

            trophy = new HuntingDisplayTrophy(HuntType.MyrmidexLarvae);
            trophy.MoveToWorld(new Point3D(736, 2149, 6), Map.Felucca);

            s = new Static(0x9C01);
            s.MoveToWorld(new Point3D(736, 2149, 6), Map.Felucca);

            XmlSpawner spawner = new XmlSpawner("HuntMaster");
            spawner.MoveToWorld(new Point3D(747, 2148, 0), Map.Felucca);
            spawner.DoRespawn = true;

            spawner = new XmlSpawner("HuntMaster");
            spawner.MoveToWorld(new Point3D(747, 2148, 0), Map.Trammel);
            spawner.DoRespawn = true;
        }
    }
}
