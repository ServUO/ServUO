using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class DeceitBrazier : Item
    {
        private static readonly Type[] m_Creatures = new Type[]
        {
            #region Animals
            typeof(FireSteed), //Set the tents up people!
            #endregion

            #region Undead
            typeof(Skeleton), typeof(SkeletalKnight), typeof(SkeletalMage), typeof(Mummy),
            typeof(BoneKnight), typeof(Lich), typeof(LichLord), typeof(BoneMagi),
            typeof(Wraith), typeof(Shade), typeof(Spectre), typeof(Zombie),
            typeof(RottingCorpse), typeof(Ghoul),
            #endregion

            #region Demons
            typeof(Balron), typeof(Daemon), typeof(Imp), typeof(GreaterMongbat),
            typeof(Mongbat), typeof(IceFiend), typeof(Gargoyle), typeof(StoneGargoyle),
            typeof(FireGargoyle), typeof(HordeMinion),
            #endregion

            #region Gazers
            typeof(Gazer), typeof(ElderGazer), typeof(GazerLarva),
            #endregion

            #region Uncategorized
            typeof(Harpy), typeof(StoneHarpy), typeof(HeadlessOne), typeof(HellHound),
            typeof(HellCat), typeof(Phoenix), typeof(LavaLizard), typeof(SandVortex),
            typeof(ShadowWisp), typeof(SwampTentacle), typeof(PredatorHellCat), typeof(Wisp),
            #endregion

            #region Arachnid
            typeof(GiantSpider), typeof(DreadSpider), typeof(FrostSpider), typeof(Scorpion),
            #endregion

            #region Repond
            typeof(ArcticOgreLord), typeof(Cyclops), typeof(Ettin), typeof(EvilMage),
            typeof(FrostTroll), typeof(Ogre), typeof(OgreLord), typeof(Orc),
            typeof(OrcishLord), typeof(OrcishMage), typeof(OrcBrute), typeof(Ratman),
            typeof(RatmanMage), typeof(OrcCaptain), typeof(Troll), typeof(Titan),
            typeof(EvilMageLord), typeof(OrcBomber), typeof(RatmanArcher),
            #endregion

            #region Reptilian
            typeof(Dragon), typeof(Drake), typeof(Snake), typeof(GreaterDragon),
            typeof(IceSerpent), typeof(GiantSerpent), typeof(IceSnake), typeof(LavaSerpent),
            typeof(Lizardman), typeof(Wyvern), typeof(WhiteWyrm),
            typeof(ShadowWyrm), typeof(SilverSerpent), typeof(LavaSnake),
            #endregion

            #region Elementals
            typeof(EarthElemental), typeof(PoisonElemental), typeof(FireElemental), typeof(SnowElemental),
            typeof(IceElemental), typeof(ToxicElemental), typeof(WaterElemental), typeof(Efreet),
            typeof(AirElemental), typeof(Golem),
            #endregion

            #region Random Critters
            typeof(Sewerrat), typeof(GiantRat), typeof(DireWolf), typeof(TimberWolf),
            typeof(Cougar), typeof(Alligator)
            #endregion
        };

        public static Type[] Creatures => m_Creatures;

        private Timer m_Timer;
        private DateTime m_NextSpawn;
        private int m_SpawnRange;
        private TimeSpan m_NextSpawnDelay;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSpawn => m_NextSpawn;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get
            {
                return m_SpawnRange;
            }
            set
            {
                m_SpawnRange = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextSpawnDelay
        {
            get
            {
                return m_NextSpawnDelay;
            }
            set
            {
                m_NextSpawnDelay = value;
            }
        }

        public override int LabelNumber => 1023633;// Brazier

        [Constructable]
        public DeceitBrazier()
            : base(0xE31)
        {
            Movable = false;
            Light = LightType.Circle225;
            m_NextSpawn = DateTime.UtcNow;
            m_NextSpawnDelay = TimeSpan.FromMinutes(15.0);
            m_SpawnRange = 5;
        }

        public DeceitBrazier(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_SpawnRange);
            writer.Write(m_NextSpawnDelay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_SpawnRange = reader.ReadInt();
                m_NextSpawnDelay = reader.ReadTimeSpan();
            }

            m_NextSpawn = DateTime.UtcNow;
        }

        public virtual void HeedWarning()
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, 500761);// Heed this warning well, and use this brazier at your own peril.
        }

        public override bool HandlesOnMovement => true;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_NextSpawn < DateTime.UtcNow) // means we haven't spawned anything if the next spawn is below
            {
                if (Utility.InRange(m.Location, Location, 1) && !Utility.InRange(oldLocation, Location, 1) && m.Player && !(m.IsStaff() || m.Hidden))
                {
                    if (m_Timer == null || !m_Timer.Running)
                        m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(2), HeedWarning);
                }
            }

            base.OnMovement(m, oldLocation);
        }

        public Point3D GetSpawnPosition()
        {
            Map map = Map;

            if (map == null)
                return Location;

            // Try 10 times to find a Spawnable location.
            for (int i = 0; i < 10; i++)
            {
                int x = Location.X + (Utility.Random((m_SpawnRange * 2) + 1) - m_SpawnRange);
                int y = Location.Y + (Utility.Random((m_SpawnRange * 2) + 1) - m_SpawnRange);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(new Point2D(x, y), Z))
                    return new Point3D(x, y, Z);
                else if (Map.CanSpawnMobile(new Point2D(x, y), z))
                    return new Point3D(x, y, z);
            }

            return Location;
        }

        public virtual void DoEffect(Point3D loc, Map map)
        {
            Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
            Effects.PlaySound(loc, map, 0x225);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(from.Location, Location, 2))
            {
                try
                {
                    if (m_NextSpawn < DateTime.UtcNow)
                    {
                        Map map = Map;
                        BaseCreature bc = (BaseCreature)Activator.CreateInstance(m_Creatures[Utility.Random(m_Creatures.Length)]);

                        if (bc != null)
                        {
                            Point3D spawnLoc = GetSpawnPosition();

                            DoEffect(spawnLoc, map);

                            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate ()
                            {
                                bc.Home = Location;
                                bc.RangeHome = m_SpawnRange;
                                bc.FightMode = FightMode.Closest;

                                bc.MoveToWorld(spawnLoc, map);

                                DoEffect(spawnLoc, map);

                                bc.ForceReacquire();
                            });

                            m_NextSpawn = DateTime.UtcNow + m_NextSpawnDelay;
                        }
                    }
                    else
                    {
                        PublicOverheadMessage(MessageType.Regular, 0x3B2, 500760); // The brazier fizzes and pops, but nothing seems to happen.
                    }
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }
    }
}
