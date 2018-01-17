using Server;
using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using System.Linq;

namespace Server.Engines.MiniChamps
{  
    public class MiniChamp : Item
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenMiniChamp", AccessLevel.Administrator, new CommandEventHandler(GenStoneRuins_OnCommand));
        }

        private static List<MiniChamp> Controllers = new List<MiniChamp>();

        [Usage("GenMiniChamp")]
        [Description("MiniChampion Generator")]
        public static void GenStoneRuins_OnCommand(CommandEventArgs e)
        {
            Controllers.ForEach(x => x.Delete());

            Map map = Map.TerMur;

            MiniChamp MiniChamp = new MiniChamp();

            MiniChamp.Type = MiniChampType.CrimsonVeins;
            MiniChamp.MoveToWorld(new Point3D(974, 161, -10), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.AbyssalLair;
            MiniChamp.MoveToWorld(new Point3D(987, 328, 11), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.DiscardedCavernClanRibbon;
            MiniChamp.MoveToWorld(new Point3D(915, 501, -11), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.DiscardedCavernClanScratch;
            MiniChamp.MoveToWorld(new Point3D(950, 552, -13), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.DiscardedCavernClanChitter;
            MiniChamp.MoveToWorld(new Point3D(980, 491, -11), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.EnslavedGoblins;
            MiniChamp.MoveToWorld(new Point3D(578, 799, -45), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.FairyDragonLair;
            MiniChamp.MoveToWorld(new Point3D(887, 273, 4), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.FireTemple;
            MiniChamp.MoveToWorld(new Point3D(546, 760, -91), map);
            MiniChamp.Active = true;
            
            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.LandsoftheLich;
            MiniChamp.MoveToWorld(new Point3D(530, 658, 9), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.LavaCaldera;
            MiniChamp.MoveToWorld(new Point3D(578, 900, -72), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.PassageofTears;
            MiniChamp.MoveToWorld(new Point3D(684, 579, -14), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.SecretGarden;
            MiniChamp.MoveToWorld(new Point3D(434, 701, 29), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.Type = MiniChampType.SkeletalDragon;
            MiniChamp.MoveToWorld(new Point3D(677, 824, -108), map);
            MiniChamp.Active = true;

            MiniChamp = new MiniChamp();
            MiniChamp.BossSpawnPoint = new Point3D(384, 1931, 50);
            MiniChamp.Type = MiniChampType.MeraktusTheTormented;
            MiniChamp.MoveToWorld(new Point3D(395, 1913, 12), Map.Malas);
            MiniChamp.Active = true;

            e.Mobile.SendMessage("Created Mini Champion Spawns.");
        }

        private bool m_Active;
        private MiniChampType m_Type;
        private List<MiniChampSpawnInfo> Spawn;
        public List<Mobile> Despawns;
        private int m_Level;
        private int m_SpawnRange;
        private TimeSpan m_RestartDelay;
        private Timer m_Timer, m_RestartTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BossSpawnPoint { get; set; }        

        [Constructable]
        public MiniChamp()
            : base(0xBD2)
        {
            Movable = false;
            Visible = false;
            Name = "Mini Champion Controller";

            Despawns = new List<Mobile>();
            Spawn = new List<MiniChampSpawnInfo>();
            m_RestartDelay = TimeSpan.FromMinutes(5.0);
            m_SpawnRange = 30;
            Controllers.Add(this);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get { return m_SpawnRange; }
            set { m_SpawnRange = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RestartDelay
        {
            get { return m_RestartDelay; }
            set { m_RestartDelay = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MiniChampType Type
        {
            get { return m_Type; }
            set { m_Type = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; InvalidateProperties(); }
        }

        public void Start()
        {
            if (m_Active || Deleted)
                return;

            m_Active = true;

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = new SliceTimer(this);
            m_Timer.Start();

            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            AdvanceLevel();
        }

        public void Stop()
        {
            if (!m_Active || Deleted)
                return;

            m_Active = false;
            m_Level = 0;

            ClearSpawn();
            Despawns.ForEach(x => x.Delete());
            Despawns.Clear();

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            if (m_RestartTimer != null)
                m_RestartTimer.Stop();

            m_RestartTimer = null;
        }

        public void OnSlice()
        {
            if (!m_Active || Deleted)
                return;

            bool changed = false;
            bool done = true;

            Spawn.ForEach(x =>
            {
                changed |= x.Slice();
                done &= x.Done;
            });

            if (done)
                AdvanceLevel();

            if (m_Active)
            {
                Spawn.ForEach(x => changed |= x.Respawn());
            }

            if (done || changed)
            {
                InvalidateProperties();
            }
        }

        public void ClearSpawn()
        {
            Spawn.ForEach(x =>
            {
                x.Creatures.ForEach(y => Despawns.Add(y));
            });

            Spawn.Clear();
        }

        public void AdvanceLevel()
        {
            MiniChampLevelInfo levelInfo = MiniChampInfo.GetInfo(m_Type).GetLevelInfo(++Level);

            if (levelInfo != null)
            {
                ClearSpawn();

                if (m_Type == MiniChampType.MeraktusTheTormented)
                {
                    MinotaurShouts();
                }

                levelInfo.Types.ToList().ForEach(x => Spawn.Add(new MiniChampSpawnInfo(this, x)));
            }
            else // begin restart
            {
                Stop();

                m_RestartTimer = Timer.DelayCall(m_RestartDelay, new TimerCallback(Start));
            }
        }

        private void MinotaurShouts()
        {
            int cliloc = 0;

            switch (Level)
            {
                case 1:
                    return;
                case 2:
                    cliloc = 1073370;
                    break;
                case 3:
                    cliloc = 1073367;
                    break;
                case 4:
                    cliloc = 1073368;
                    break;
                case 5:
                    cliloc = 1073369;
                    break;
            }

            IPooledEnumerable eable = GetMobilesInRange(m_SpawnRange);

            foreach(Mobile x in eable)
            {
                if (x is PlayerMobile)
                    x.SendLocalizedMessage(cliloc);
            }

            eable.Free();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Type\t{0}", m_Type); // ~1_val~: ~2_val~
            list.Add(1060661, "Spawn Range\t{0}", m_SpawnRange); // ~1_val~: ~2_val~

            if (m_Active)
            {
                list.Add(1060742); // active
                list.Add(1060659, "Level\t{0}", Level); // ~1_val~: ~2_val~

                for (int i = 0; i < Spawn.Count; i++)
                {
                    Spawn[i].AddProperties(list, i + 1150301);
                }
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new PropertiesGump(from, this));
        }

        public override void OnDelete()
        {
            Controllers.Remove(this);
            Stop();

            base.OnDelete();
        }

        public Point3D GetBossSpawnPoint() { return BossSpawnPoint; }

        public MiniChamp(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(BossSpawnPoint);
            writer.Write((bool)m_Active);
            writer.Write((int)m_Type);
            writer.Write((int)m_Level);
            writer.Write((int)m_SpawnRange);
            writer.Write((TimeSpan)m_RestartDelay);

            writer.Write((int)Spawn.Count);

            for (int i = 0; i < Spawn.Count; i++)
            {
                Spawn[i].Serialize(writer);
            }

            writer.Write(Despawns, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        Spawn = new List<MiniChampSpawnInfo>();

                        BossSpawnPoint = reader.ReadPoint3D();
                        m_Active = reader.ReadBool();
                        m_Type = (MiniChampType)reader.ReadInt();
                        m_Level = reader.ReadInt();
                        m_SpawnRange = reader.ReadInt();
                        m_RestartDelay = reader.ReadTimeSpan();

                        int spawnCount = reader.ReadInt();

                        for (int i = 0; i < spawnCount; i++)
                        {
                            Spawn.Add(new MiniChampSpawnInfo(reader));
                        }

                        Despawns = reader.ReadStrongMobileList();

                        if (m_Active)
                        {
                            m_Timer = new SliceTimer(this);
                            m_Timer.Start();
                        }
                        else
                        {
                            m_RestartTimer = Timer.DelayCall(m_RestartDelay, new TimerCallback(Start));
                        }

                        break;
                    }
            }

            Controllers.Add(this);
        }
    }
}