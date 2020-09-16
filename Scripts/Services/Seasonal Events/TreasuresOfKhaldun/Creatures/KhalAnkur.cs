using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a Khal Ankur corpse")]
    public class KhalAnkur : BaseChampion
    {
        public ChampionSpawn Spawn { get; set; }
        private DateTime m_NextSpawn;
        private DateTime m_NextSay;
        private DateTime m_NextAbilityTime;

        [Constructable]
        public KhalAnkur()
            : base(AIType.AI_Necro)
        {
            Name = "Khal Ankur";
            Body = 0x5C7;
            BaseSoundID = 0x301;

            SetStr(700, 800);
            SetDex(500, 600);
            SetInt(800, 900);

            SetHits(30000);

            SetDamage(28, 35);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Poison, 25);

            SetResistance(ResistanceType.Physical, 90);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Parry, 80.0, 100.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);

            Fame = 28000;
            Karma = -28000;

            SetMagicalAbility(MagicalAbility.WrestlingMastery);
        }

        public KhalAnkur(Serial serial)
            : base(serial)
        {
        }

        public override bool Unprovokable => true;

        public override bool BleedImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override bool ShowFameTitle => false;

        public override bool ClickTitle => false;

        public override bool AlwaysMurderer => true;

        public override bool AutoDispel => true;

        public override double AutoDispelChance => 1.0;

        public override ChampionSkullType SkullType => ChampionSkullType.None;

        public override Type[] UniqueList => new Type[] { };

        public override Type[] SharedList => new Type[] { };

        public override Type[] DecorativeList => new Type[] { };

        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };

        public override void OnBeforeDamage(Mobile from, ref int totalDamage, DamageType type)
        {
            if (Region.IsPartOf("Khaldun") && IsChampionSpawn && !Caddellite.CheckDamage(from, type))
            {
                totalDamage = 0;
            }

            base.OnBeforeDamage(from, ref totalDamage, type);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Hue == 0 && Hits < HitsMax * 0.60 && DateTime.UtcNow > m_NextAbilityTime && 0.2 > Utility.RandomDouble())
            {
                Hue = 2745;
                new InternalTimer(this);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Blessed)
            {
                from.SendLocalizedMessage(1071372); // It's covered with treasure guardian's magical power. To touch it, you need to beat them!
            }

            base.OnDoubleClick(from);
        }

        public override void OnChampPopped(ChampionSpawn spawn)
        {
            Blessed = true;
            Spawn = spawn;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Spawn == null || Map == null)
                return;

            if (!Utility.InRange(Location, Home, 150))
            {
                Timer.DelayCall(TimeSpan.FromSeconds(5), () => { Location = Home; });
            }

            if (Blessed)
            {
                if (m_NextSay < DateTime.UtcNow)
                {
                    Say(1158752 + Utility.Random(5), 0x25);

                    m_NextSay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));
                }

                if (m_NextSpawn < DateTime.UtcNow)
                {
                    BaseCreature bc = new KhalAnkurWarriors(KhalAnkurWarriors.WarriorType.General);

                    int x, y, z = 0;

                    Point3D p = Location;

                    for (int i = 0; i < 25; i++)
                    {
                        x = Utility.RandomMinMax(p.X - 4, p.X + 4);
                        y = Utility.RandomMinMax(p.Y - 4, p.Y + 4);
                        z = Map.GetAverageZ(x, y);

                        if (Map.CanSpawnMobile(x, y, z))
                        {
                            p = new Point3D(x, y, z);
                            break;
                        }
                    }

                    bc.MoveToWorld(p, Map);
                    bc.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);
                    bc.Home = p;
                    bc.IsChampionSpawn = true;
                    Spawn.Creatures.Add(bc);

                    m_NextSpawn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(120, 180));
                }
                else if (Spawn.Creatures.OfType<KhalAnkurWarriors>().Where(x => x._Type == KhalAnkurWarriors.WarriorType.General && !x.Deleted).Count() <= 0)
                {
                    Blessed = false;
                    return;
                }
            }
        }

        private class InternalTimer : Timer
        {
            private readonly KhalAnkur m_Mobile;
            private int m_Tick;

            public InternalTimer(KhalAnkur mob)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Tick = 1;
                m_Mobile = mob;
                Start();
            }

            protected override void OnTick()
            {
                if (!m_Mobile.Alive || m_Mobile.Deleted || m_Mobile.Map == null)
                {
                    Stop();
                }
                else if (m_Tick < 15)
                {
                    Point3D p = FindLocation(m_Mobile.Map, m_Mobile.Location, 7);
                    Effects.SendLocationEffect(p, m_Mobile.Map, 0x3789, 30, 1, 2062, 0x4);

                    m_Tick++;
                }
                else
                {
                    m_Mobile.ClearAround();
                    Stop();
                }
            }

            private Point3D FindLocation(Map map, Point3D center, int range)
            {
                int cx = center.X;
                int cy = center.Y;

                for (int i = 0; i < 20; ++i)
                {
                    int x = cx + Utility.Random(range * 2) - range;
                    int y = cy + Utility.Random(range * 2) - range;

                    if ((cx - x) * (cx - x) + (cy - y) * (cy - y) > range * range)
                        continue;

                    int z = map.GetAverageZ(x, y);

                    if (!map.CanFit(x, y, z, 6, false, false))
                        continue;

                    int topZ = z;

                    foreach (Item item in map.GetItemsInRange(new Point3D(x, y, z), 0))
                    {
                        topZ = Math.Max(topZ, item.Z + item.ItemData.CalcHeight);
                    }

                    return new Point3D(x, y, topZ);
                }

                return center;
            }
        }

        private void ClearAround()
        {
            Point3D loc = Location;
            Map pmmap = Map;

            List<Point3D> points = new List<Point3D>();

            Misc.Geometry.Circle2D(loc, pmmap, 7, (pnt, map) =>
            {
                if (map.CanFit(pnt, 0) && InLOS(pnt))
                    points.Add(pnt);
            });

            if (pmmap != Map.Internal && pmmap != null)
            {
                Misc.Geometry.Circle2D(loc, pmmap, 6, (pnt, map) =>
                {
                    if (map.CanFit(pnt, 0) && InLOS(pnt) && Utility.RandomBool())
                    {
                        Effects.SendPacket(pnt, map, new ParticleEffect(EffectType.FixedXYZ, Serial, Serial.Zero, 0x3789, pnt, pnt, 1, 30, false, false, 0, 3, 0, 9502, 1, Serial, 153, 0));
                        Effects.SendPacket(pnt, map, new ParticleEffect(EffectType.FixedXYZ, Serial, Serial.Zero, 0x9DAC, pnt, pnt, 1, 30, false, false, 0, 0, 0, 9502, 1, Serial, 153, 0));
                    }
                });

                Misc.Geometry.Circle2D(loc, pmmap, 7, (pnt, map) =>
                {
                    if (map.CanFit(pnt, 0) && InLOS(pnt) && Utility.RandomBool())
                    {
                        Effects.SendPacket(pnt, map, new ParticleEffect(EffectType.FixedXYZ, Serial, Serial.Zero, 0x3789, pnt, pnt, 1, 30, false, false, 0, 3, 0, 9502, 1, Serial, 153, 0));
                        Effects.SendPacket(pnt, map, new ParticleEffect(EffectType.FixedXYZ, Serial, Serial.Zero, 0x9DAC, pnt, pnt, 1, 30, false, false, 0, 0, 0, 9502, 1, Serial, 153, 0));
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                IPooledEnumerable eable = GetMobilesInRange(6);

                foreach (Mobile from in eable)
                {
                    if (!from.Alive || from == this || from.AccessLevel > AccessLevel.Player)
                        continue;

                    if (points.Count > 0 && (from is PlayerMobile || (from is BaseCreature && (((BaseCreature)from).Controlled) || ((BaseCreature)from).Summoned)))
                    {
                        Point3D point = points[Utility.Random(points.Count)];
                        from.MoveToWorld(point, pmmap);
                        from.Frozen = true;

                        Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                        {
                            from.Frozen = false;
                            from.SendLocalizedMessage(1005603); // You can move again!
                        });

                        if (CanBeHarmful(from))
                        {
                            double damage = from.Hits * 0.6;

                            if (damage < 10.0)
                                damage = 10.0;
                            else if (damage > 75.0)
                                damage = 75.0;

                            DoHarmful(from);

                            AOS.Damage(from, this, (int)damage, 100, 0, 0, 0, 0);
                        }
                    }
                }

                eable.Free();
            });

            Hue = 0;

            m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(200, 300));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.Meager);
        }
        
        private int _120GPowerScrolls = 4;

        public override Item GetPowerScroll()
        {
            if (_120GPowerScrolls > 0)
            {
                _120GPowerScrolls--;

                return PowerScroll.CreateRandomNoCraft(20, 20);
            }

            return base.GetPowerScroll();
        }

        private int _120GJPowerScrolls = 4;

        public override Item GetJusticePowerScroll()
        {
            if (_120GJPowerScrolls > 0)
            {
                _120GJPowerScrolls--;

                return PowerScroll.CreateRandomNoCraft(20, 20);
            }

            return base.GetJusticePowerScroll();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.WriteItem(Spawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Spawn = reader.ReadItem<ChampionSpawn>();
        }
    }
}
