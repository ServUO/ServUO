using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class CorgulAltar : Container
    {
        #region Statics
        private static TimeSpan ExpireTime = TimeSpan.FromMinutes(180);
        private static TimeSpan CompleteTime = TimeSpan.FromMinutes(15);

        private static readonly int m_RegionSize = 186;

        private static readonly Rectangle2D[] m_WarpLocations =
        {
                new Rectangle2D(2885, 1373, 500, 800),
                new Rectangle2D(330,  2940, 400, 400),
                new Rectangle2D(4040, 2550, 500, 350),
                new Rectangle2D(4040, 1755, 500, 250),
                new Rectangle2D(180,  180,  300, 300)
        };

        public static Rectangle2D[] WarpLocations => m_WarpLocations;

        private static Rectangle2D m_BoatKickLocation = new Rectangle2D(2400, 2500, 500, 500);
        public static Rectangle2D BoatKickLocation => m_BoatKickLocation;

        private static Rectangle2D m_LandKickLocation = new Rectangle2D(2125, 3090, 25, 30);
        public static Rectangle2D LandKickLocation => m_LandKickLocation;

        private static Rectangle2D m_CorgulBounds = new Rectangle2D(6337, 1156, m_RegionSize, m_RegionSize);

        public static Rectangle2D CorgulBounds => m_CorgulBounds;
        #endregion

        private bool m_Activated;
        private bool m_Active;
        private CorgulWarpRegion m_WarpRegion;
        private CorgulRegion m_BossRegion;
        private Point3D m_WarpPoint;
        private DateTime m_DeadLine;
        private Mobile m_Boss;
        private int m_KeyStage;
        private readonly List<Item> m_IslandMaps = new List<Item>();

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated => m_Activated;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                m_Active = value;
                PublicOverheadMessage(Network.MessageType.Regular, 25, false, string.Format("Corgul Altar for {0} has been {1}", Map, m_Active ? "activated" : "deactivated"));
            }
        }

        public CorgulWarpRegion WarpRegion => m_WarpRegion;
        public CorgulRegion BossRegion => m_BossRegion;

        private readonly Type[] m_Keys =
        {
            typeof(TreasureMap), typeof(WorldMap)
        };

        public static Point3D SpawnLoc = new Point3D(6431, 1236, 10);

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeadLine => m_DeadLine;

        public int KeyStage => m_KeyStage;

        public override int LabelNumber => 1074818;

        public CorgulAltar()
            : base(13807)
        {
            Movable = false;
            Hue = 2075;
            m_Activated = false;
            m_Active = true;
            m_WarpRegion = null;
            m_WarpPoint = Point3D.Zero;
            m_KeyStage = 0;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), InitializeBossRegion);
        }

        public void InitializeBossRegion()
        {
            m_BossRegion = new CorgulRegion(m_CorgulBounds, this);
            m_BossRegion.Register();
        }

        public override void Delete()
        {
            Active = false;
            Reset();

            if (m_BossRegion != null)
            {
                m_BossRegion.Unregister();
                m_BossRegion = null;
            }

            if (m_WarpRegion != null)
            {
                m_WarpRegion.Unregister();
                m_WarpRegion = null;
            }

            base.Delete();
        }

        public override void OnMapChange()
        {
            if (m_BossRegion != null)
            {
                m_BossRegion.Unregister();
                InitializeBossRegion();
            }

            Reset();
        }

        private Point3D GetRandomWarpPoint()
        {
            Rectangle2D rec = m_WarpLocations[Utility.Random(m_WarpLocations.Length)];

            int x = Utility.Random(rec.X, rec.Width);
            int y = Utility.Random(rec.Y, rec.Height);

            return new Point3D(x, y, -5);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!m_Active)
                from.SendMessage("This altar has been deactivated.");
            else if (!CheckCanUse(from))
                from.SendLocalizedMessage(1116791); // You must wait a few minutes before making your sacrifice.
            else if (from.InRange(Location, 3))
            {
                from.Target = new InternalTarget(this);

                if (m_KeyStage == 0)
                    from.SendLocalizedMessage(1116586); // Your offering will be consumed by the altar if the sacrifice is accepted. You will then have 30 seconds to re-use the shrine to mark your map and pay the blood cost.
            }

        }

        private bool CheckCanUse(Mobile from)
        {
            if (m_Activated)
            {
                if (Map == Map.Trammel)
                    return false;

                if (m_Boss == null || !m_Boss.Alive || m_Boss.Hits < m_Boss.HitsMax / 2)
                    return false;
            }

            return true;
        }

        private class InternalTarget : Target
        {
            private readonly CorgulAltar m_Altar;

            public InternalTarget(CorgulAltar altar) : base(-1, false, TargetFlags.None)
            {
                m_Altar = altar;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item && ((Item)targeted).IsChildOf(from.Backpack) && m_Altar.IsKey((Item)targeted, from))
                    m_Altar.OnSacraficedItem((Item)targeted, from);
            }
        }

        public void OnSacraficedItem(Item item, Mobile from)
        {
            if (m_KeyStage == 1)
            {
                if (!m_Activated)
                {
                    SpawnBoss(from);
                    m_DeadLineTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), OnTick);
                    m_DeadLine = DateTime.UtcNow + ExpireTime;
                }

                from.Hits = 0;
                from.Mana = 0;
                from.Stam = 2;
                from.PlaySound(0x244);
                from.FixedParticles(0x3709, 1, 30, 9965, 5, 7, EffectLayer.Waist);
                from.FixedParticles(0x376A, 1, 30, 9502, 5, 3, EffectLayer.Waist);

                GiveMap(from);
                ResetKeys();
            }
            else
            {
                m_KeyStage = 1;
                from.SendLocalizedMessage(1116585); // Your offering has been accepted. The price of blood will be taken when your -world map- is marked with the secret location.

                m_KeyResetTimer = Timer.DelayCall(TimeSpan.FromSeconds(30), ResetKeys);
            }

            item.Delete();
        }

        private void OnTick()
        {
            if (DateTime.UtcNow > m_DeadLine)
            {
                OnDeadLine();
            }
            else
            {
                for (int i = 0; i < m_IslandMaps.Count; i++)
                {
                    if (m_IslandMaps[i] != null)
                    {
                        m_IslandMaps[i].InvalidateProperties();
                    }
                }
            }
        }

        private void ResetKeys()
        {
            m_KeyStage = 0;

            if (m_KeyResetTimer != null)
                m_KeyResetTimer.Stop();

            m_KeyResetTimer = null;
        }

        public bool IsKey(Item item, Mobile from)
        {
            Type type = item.GetType();

            if (m_KeyStage >= 0 && m_KeyStage < m_Keys.Length && type == m_Keys[m_KeyStage])
                return true;
            else if (m_KeyStage == 1 && item is PresetMap && item.LabelNumber == 1041204)
                return true;
            else if (m_KeyStage == 1 && item is TreasureMap)
                from.SendLocalizedMessage(1116360); // The island's location cannot be marked on a treasure map.
            else if (m_KeyStage == 1 && item is MapItem)
                from.SendLocalizedMessage(1116358); // The island's location cannot be marked on this map.
            else
                from.SendLocalizedMessage(1072682); // This is not the proper key.
            return false;
        }

        public void OnBossKilled()
        {
            m_ResetTimer = Timer.DelayCall(CompleteTime, Reset);

            EndDeadLineTimer();

            if (m_BossRegion == null)
                return;

            foreach (Mobile m in m_BossRegion.GetMobiles())
            {
                if (m is PlayerMobile)
                    m.SendLocalizedMessage(1072681); // The master of this realm has been slain! You may only stay here so long.
            }
        }

        public void OnDeadLine()
        {
            if (m_BossRegion == null)
                return;

            foreach (Mobile m in m_BossRegion.GetMobiles())
            {
                if (m is PlayerMobile)
                    m.SendMessage("You have failed to slay Corgul in time.");
            }
            Reset();
        }

        public void Reset()
        {
            m_Activated = false;

            m_WarpPoint = Point3D.Zero;

            if (m_BossRegion != null)
                m_BossRegion.RemovePlayers(false);

            EndResetTimer();
            EndDeadLineTimer();

            if (m_Boss != null && !m_Boss.Deleted)
                m_Boss.Delete();

            m_Boss = null;

            foreach (Item item in m_IslandMaps)
            {
                if (item != null && !item.Deleted)
                    item.Delete();
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1), UnregisterWarpRegion);

            m_IslandMaps.Clear();
            ResetKeys();
        }

        private Timer m_ResetTimer;
        private Timer m_DeadLineTimer;
        private Timer m_KeyResetTimer;

        public void EndResetTimer()
        {
            if (m_ResetTimer != null)
                m_ResetTimer.Stop();

            m_ResetTimer = null;
        }

        public void EndDeadLineTimer()
        {
            if (m_DeadLineTimer != null)
                m_DeadLineTimer.Stop();

            m_DeadLine = DateTime.MinValue;
            m_DeadLineTimer = null;
        }

        public void UnregisterWarpRegion()
        {
            if (m_WarpRegion != null)
            {
                m_WarpRegion.Unregister();
                m_WarpRegion = null;
            }
        }

        public void SpawnBoss(Mobile from)
        {
            //Spawn boss
            CorgulTheSoulBinder boss = new CorgulTheSoulBinder(this);
            boss.MoveToWorld(SpawnLoc, Map);
            boss.SpawnHelpers();
            m_Boss = boss;

            //create dummy spawn point and bounds for warp region
            m_WarpPoint = GetRandomWarpPoint();
            Rectangle2D bounds = GetRectangle(m_WarpPoint);

            //create region based on dummy spot and bounds
            m_WarpRegion = new CorgulWarpRegion(this, bounds);
            m_WarpRegion.Register();
            m_Activated = true;
        }

        private void GiveMap(Mobile from)
        {
            CorgulIslandMap map = new CorgulIslandMap(m_WarpPoint, this);
            from.AddToBackpack(map);

            m_IslandMaps.Add(map);

            from.SendLocalizedMessage(1116359); // The island's location has been marked on your map. You should make haste while the island is still afloat.
        }

        public static Point3D GetRandomPoint(Rectangle2D rec, Map map)
        {
            int x = Utility.Random(rec.X, rec.Width);
            int y = Utility.Random(rec.Y, rec.Height);
            int z = map.GetAverageZ(x, y);

            return new Point3D(x, y, z);
        }

        public Rectangle2D GetRectangle(Point3D pnt)
        {
            return new Rectangle2D(pnt.X - (m_RegionSize / 2), pnt.Y - (m_RegionSize / 2), m_RegionSize, m_RegionSize);
        }

        public CorgulAltar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(m_DeadLine);
            writer.Write(m_Boss);
            writer.Write(m_Activated);
            writer.Write(m_Active);
            writer.Write(m_WarpPoint);
            //writer.Write(m_IslandMap); Old version 0

            writer.Write(m_IslandMaps.Count);
            foreach (Item item in m_IslandMaps)
                writer.Write(item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_DeadLine = reader.ReadDateTime();
                        m_Boss = reader.ReadMobile();
                        m_Activated = reader.ReadBool();
                        m_Active = reader.ReadBool();
                        m_WarpPoint = reader.ReadPoint3D();

                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Item map = reader.ReadItem();

                            if (map != null && !map.Deleted && map is CorgulIslandMap)
                            {
                                m_IslandMaps.Add(map);
                                ((CorgulIslandMap)map).Altar = this;
                            }
                        }

                        break;
                    }
                case 0:
                    {
                        m_DeadLine = reader.ReadDateTime();
                        m_Boss = reader.ReadMobile();
                        m_Activated = reader.ReadBool();
                        m_Active = reader.ReadBool();
                        m_WarpPoint = reader.ReadPoint3D();
                        //m_IslandMap = reader.ReadItem() as CorgulIslandMap;
                        Item item = reader.ReadItem();
                        break;
                    }
            }

            InitializeBossRegion();

            if (m_Active && m_Activated && m_WarpPoint != Point3D.Zero)
            {
                if (m_DeadLine < DateTime.UtcNow || m_Boss == null || m_Boss.Deleted)
                    Reset();
                else
                {
                    Rectangle2D bounds = GetRectangle(m_WarpPoint);
                    m_WarpRegion = new CorgulWarpRegion(this, bounds);
                    m_WarpRegion.Register();

                    m_DeadLineTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), OnTick);
                }
            }
        }
    }
}
