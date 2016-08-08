using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.HuntsmasterChallenge
{
	public class HuntingSystem : Item
	{
		private static HuntingSystem m_Instance;
		public static HuntingSystem Instance { get { return m_Instance; } }
		
		private DateTime m_SeasonBegins;
		private DateTime m_SeasonEnds;
        private DateTime m_NextHint;
        private DateTime m_NextBonusIndex;
        private int m_BonusIndex;
        private bool m_Active;

        private Timer m_Timer;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime SeasonBegins { get { return m_SeasonBegins; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime SeasonEnds { get { return m_SeasonEnds; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BonusIndex { get { return m_BonusIndex; } }

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
			if(Core.SA && m_Instance == null)
				m_Instance = new HuntingSystem();
		}
		
		public HuntingSystem() : base(17603)
		{
			if(m_Instance != null)
			{
				this.Delete();
				return;
			}
			
			m_Instance = this;
            m_Active = true;

			m_Top10 = new Dictionary<HuntType, List<HuntingKillEntry>>();
			m_Leaders = new Dictionary<HuntType, List<HuntingKillEntry>>();

            m_SeasonBegins = DateTime.Now;
            DateTime ends = DateTime.Now + TimeSpan.FromDays(30);
            m_SeasonEnds = new DateTime(ends.Year, ends.Month, ends.Day, 8, 0, 0);
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

                m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(OnTick));
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
			if(DateTime.Now >= m_SeasonEnds)
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
			if(permit.KillEntry == null || permit.KillEntry.KillIndex < 0 || permit.KillEntry.KillIndex > HuntingTrophyInfo.Infos.Count)
				master.SayTo(from, 1155706); // That is not a valid kill.
			else
			{
				HuntingTrophyInfo info = HuntingTrophyInfo.Infos[permit.KillEntry.KillIndex];
				
				if(info != null)
				{
                    if (!m_Leaders.ContainsKey(info.HuntType))
                        m_Leaders[info.HuntType] = new List<HuntingKillEntry>();

					List<HuntingKillEntry> leaders = m_Leaders[info.HuntType];
					
					if(leaders.Count == 0 || permit.KillEntry.Measurement >= leaders[0].Measurement)
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

                for(int i = 0; i < copy.Count; i++)
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
                    if(killEntry.Owner != null)
                        m_UnclaimedWinners[killEntry.Owner] = killEntry;
                }
            }
			
			m_Leaders.Clear();
			
			m_SeasonBegins = DateTime.Now;
            DateTime ends = DateTime.Now + TimeSpan.FromDays(30);
			m_SeasonEnds = new DateTime(ends.Year, ends.Month, ends.Day, 8, 0, 0);

            HuntingDisplayTrophy.InvalidateDisplayTrophies();
		}
		
		public bool CheckUnclaimedEntry(Mobile from)
		{
            List<Mobile> copy = new List<Mobile>(m_UnclaimedWinners.Keys);
			foreach(Mobile m in copy)
			{
				if(m == from)
				{
					ClaimWin(from, m_UnclaimedWinners[m]);
				
					m_UnclaimedWinners.Remove(m);
					return true;
				}
			}
			
			return false;
		}
		
		private void ClaimWin(Mobile from, HuntingKillEntry entry)
		{
			TryDropItemTo(from, new HarvestersBlade());
            TryDropItemTo(from, new HuntmastersRewardTitleDeed());
			TryDropItemTo(from, new RangersGuildSash());
            TryDropItemTo(from, new HornOfPlenty());
		}
		
		private void TryDropItemTo(Mobile from, Item item)
		{
			if(from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
			{
				from.BankBox.DropItem(item);
				from.SendMessage("A reward item has been placed in your bankbox.");
			}
			else
				from.SendMessage("A reward item has been placed in your backpack."); 
		}
	
		public HuntingSystem(Serial serial) : base(serial)
		{
		}
		
		private Dictionary<HuntType, List<HuntingKillEntry>> m_Leaders;
		public Dictionary<HuntType, List<HuntingKillEntry>> Leaders { get { return m_Leaders; } }

        private Dictionary<Mobile, HuntingKillEntry> m_UnclaimedWinners = new Dictionary<Mobile, HuntingKillEntry>();
        public Dictionary<Mobile, HuntingKillEntry> UnclaimedWinners { get { return m_UnclaimedWinners; } }
		
		private Dictionary<HuntType, List<HuntingKillEntry>> m_Top10;
		public Dictionary<HuntType, List<HuntingKillEntry>> Top10 { get { return m_Top10; } }

        public override void Delete()
        {
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

            writer.Write(m_Active);
            writer.Write(m_SeasonBegins);
            writer.Write(m_SeasonEnds);

			writer.Write(m_UnclaimedWinners.Count);
            foreach (KeyValuePair<Mobile, HuntingKillEntry> kvp in m_UnclaimedWinners)
            {
                writer.Write(kvp.Key);
                kvp.Value.Serialize(writer);
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
			foreach(KeyValuePair<HuntType, List<HuntingKillEntry>> kvp in m_Leaders)
			{
				writer.Write((int)kvp.Key);
				writer.Write(kvp.Value.Count);
				
				foreach(HuntingKillEntry entry in kvp.Value)
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
			for(int i = 0; i < count; i++)
			{
                Mobile m = reader.ReadMobile();
				HuntingKillEntry entry = new HuntingKillEntry(reader);

				if(m != null)
					m_UnclaimedWinners[m] = entry;
			}
			
			count = reader.ReadInt();
			for(int i = 0; i < count; i++)
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
			for(int i = 0; i < count; i++)
			{
				HuntType type = (HuntType)reader.ReadInt();
				int c = reader.ReadInt();

                if (!m_Leaders.ContainsKey(type))
                    m_Leaders[type] = new List<HuntingKillEntry>();
				
				for(int j = 0; j < c; j++)
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

            XmlSpawner spawner = new XmlSpawner("HuntMaster");
            spawner.MoveToWorld(new Point3D(747, 2148, 0), Map.Felucca);
            spawner.DoRespawn = true;

            spawner = new XmlSpawner("HuntMaster");
            spawner.MoveToWorld(new Point3D(747, 2148, 0), Map.Trammel);
            spawner.DoRespawn = true;
        }
	}
}