using System;
using System.Collections.Generic;
using Server.Events.Halloween;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Events
{
    public class HalloweenHauntings
    {
        private static readonly Rectangle2D[] m_Cemetaries = new Rectangle2D[]
        {
            new Rectangle2D(1272,3712,30,20), // Jhelom
            new Rectangle2D(1337,1444,48,52), // Britain
            new Rectangle2D(2424,1098,20,28), // Trinsic
            new Rectangle2D(2728,840,54,54), // Vesper
            new Rectangle2D(4528,1314,20,28), // Moonglow
            new Rectangle2D(712,1104,30,22), // Yew
            new Rectangle2D(5824,1464,22,6), // Fire Dungeon
            new Rectangle2D(5224,3655,14,5), // T2A

            new Rectangle2D(1272,3712,20,30), // Jhelom
            new Rectangle2D(1337,1444,52,48), // Britain
            new Rectangle2D(2424,1098,28,20), // Trinsic
            new Rectangle2D(2728,840,54,54), // Vesper
            new Rectangle2D(4528,1314,28,20), // Moonglow
            new Rectangle2D(712,1104,22,30), // Yew
            new Rectangle2D(5824,1464,6,22), // Fire Dungeon
            new Rectangle2D(5224,3655,5,14), // T2A
        };
        private static Timer m_Timer;
        private static Timer m_ClearTimer;
        private static int m_TotalZombieLimit;
        private static int m_DeathQueueLimit;
        private static int m_QueueDelaySeconds;
        private static int m_QueueClearIntervalSeconds;
        private static Dictionary<PlayerMobile, ZombieSkeleton> m_ReAnimated;
        private static List<PlayerMobile> m_DeathQueue;
        public static Dictionary<PlayerMobile, ZombieSkeleton> ReAnimated
        {
            get
            {
                return m_ReAnimated;
            }
            set
            {
                m_ReAnimated = value;
            }
        }
        public static void Initialize()
        {
            m_TotalZombieLimit = 200;
            m_DeathQueueLimit = 200;
            m_QueueDelaySeconds = 120;
            m_QueueClearIntervalSeconds = 1800;

            DateTime today = DateTime.UtcNow;
            TimeSpan tick = TimeSpan.FromSeconds(m_QueueDelaySeconds);
            TimeSpan clear = TimeSpan.FromSeconds(m_QueueClearIntervalSeconds);

            m_ReAnimated = new Dictionary<PlayerMobile, ZombieSkeleton>();
            m_DeathQueue = new List<PlayerMobile>();

            if (today >= HolidaySettings.StartHalloween && today <= HolidaySettings.FinishHalloween)
            {
                m_Timer = Timer.DelayCall(tick, tick, new TimerCallback(Timer_Callback));

                m_ClearTimer = Timer.DelayCall(clear, clear, new TimerCallback(Clear_Callback));

                EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);
            }
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            if (e.Mobile != null && !e.Mobile.Deleted) /* not sure .. better safe than sorry? */
            {
                if (e.Mobile is PlayerMobile)
                {
                    PlayerMobile player = e.Mobile as PlayerMobile;

                    if (m_Timer.Running && !m_DeathQueue.Contains(player) && m_DeathQueue.Count < m_DeathQueueLimit)
                    {
                        m_DeathQueue.Add(player);
                    }
                }
            }
        }

        private static void Clear_Callback()
        {
            m_ReAnimated.Clear();

            m_DeathQueue.Clear();

            if (DateTime.UtcNow <= HolidaySettings.FinishHalloween)
            {
                m_ClearTimer.Stop();
            }
        }

        private static void Timer_Callback()
        {
            PlayerMobile player = null;

            if (DateTime.UtcNow <= HolidaySettings.FinishHalloween)
            {
                for (int index = 0; m_DeathQueue.Count > 0 && index < m_DeathQueue.Count; index++)
                {
                    if (!m_ReAnimated.ContainsKey(m_DeathQueue[index]))
                    {
                        player = m_DeathQueue[index];

                        break;
                    }
                }

                if (player != null && !player.Deleted && m_ReAnimated.Count < m_TotalZombieLimit)
                {
                    Map map = Utility.RandomBool() ? Map.Trammel : Map.Felucca;
                    Point3D home = (GetRandomPointInRect(m_Cemetaries[Utility.Random(m_Cemetaries.Length)], map));

                    if (map.CanSpawnMobile(home))
                    {
                        ZombieSkeleton zombieskel = new ZombieSkeleton(player);

                        m_ReAnimated.Add(player, zombieskel);
                        zombieskel.Home = home;
                        zombieskel.RangeHome = 10;

                        zombieskel.MoveToWorld(home, map);

                        m_DeathQueue.Remove(player);
                    }
                }
            }
            else
            {
                m_Timer.Stop();
            }
        }

        private static Point3D GetRandomPointInRect(Rectangle2D rect, Map map)
        {
            int x = Utility.Random(rect.X, rect.Width);
            int y = Utility.Random(rect.Y, rect.Height);

            return new Point3D(x, y, map.GetAverageZ(x, y));
        }
    }

    public class PlayerBones : BaseContainer
    {
        [Constructable]
        public PlayerBones(String name)
            : base(Utility.RandomMinMax(0x0ECA, 0x0ED2))
        {
            this.Name = String.Format("{0}'s bones", name);

            switch( Utility.Random(10) )
            {
                case 0:
                    this.Hue = 0xa09;
                    break;
                case 1:
                    this.Hue = 0xa93;
                    break;
                case 2:
                    this.Hue = 0xa47;
                    break;
                default:
                    break;
            }
        }

        public PlayerBones(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a rotting corpse")]
    public class ZombieSkeleton : BaseCreature
    {
        private static readonly string m_Name = "Zombie Skeleton";
        private PlayerMobile m_DeadPlayer;
        public ZombieSkeleton()
            : this(null)
        {
        }

        public ZombieSkeleton(PlayerMobile player)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.m_DeadPlayer = player;

            this.Name = (player != null) ? String.Format("{0}'s {1}", player.Name, m_Name) : m_Name;

            this.Body = 0x93;
            this.BaseSoundID = 0x1c3;

            this.SetStr(500);
            this.SetDex(500);
            this.SetInt(500);

            this.SetHits(2500);
            this.SetMana(500);
            this.SetStam(500);

            this.SetDamage(8, 18);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Fire, 50);
            this.SetResistance(ResistanceType.Energy, 50);
            this.SetResistance(ResistanceType.Physical, 50);
            this.SetResistance(ResistanceType.Cold, 50);
            this.SetResistance(ResistanceType.Poison, 50);

            this.SetSkill(SkillName.MagicResist, 65.1, 80.0);
            this.SetSkill(SkillName.Tactics, 95.1, 100);
            this.SetSkill(SkillName.Wrestling, 85.1, 95);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 18;
        }

        public ZombieSkeleton(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }

        public override void GenerateLoot()
        {
            switch( Utility.Random(10) )
            {
                case 0:
                    this.PackItem(new LeftArm());
                    break;
                case 1:
                    this.PackItem(new RightArm());
                    break;
                case 2:
                    this.PackItem(new Torso());
                    break;
                case 3:
                    this.PackItem(new Bone());
                    break;
                case 4:
                    this.PackItem(new RibCage());
                    break;
                case 5:
                    if (this.m_DeadPlayer != null && !this.m_DeadPlayer.Deleted)
                    {
                        this.PackItem(new PlayerBones(this.m_DeadPlayer.Name));
                    }
                    break;
                default:
                    break;
            }

            this.AddLoot(LootPack.Meager);
        }

        public override void OnDelete()
        {
            if (HalloweenHauntings.ReAnimated != null)
            {
                if (this.m_DeadPlayer != null && !this.m_DeadPlayer.Deleted)
                {
                    if (HalloweenHauntings.ReAnimated.Count > 0 && HalloweenHauntings.ReAnimated.ContainsKey(this.m_DeadPlayer))
                    {
                        HalloweenHauntings.ReAnimated.Remove(this.m_DeadPlayer);
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.WriteMobile(this.m_DeadPlayer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_DeadPlayer = (PlayerMobile)reader.ReadMobile();
        }
    }
}