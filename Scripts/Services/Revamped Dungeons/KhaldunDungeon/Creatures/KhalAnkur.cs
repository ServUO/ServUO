using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a Khal Ankur corpse")]
    public class KhalAnkur : BaseChampion
    {
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

            VirtualArmor = 80;

            SetMagicalAbility(MagicalAbility.WrestlingMastery);
        }

        public KhalAnkur(Serial serial)
            : base(serial)
        {
        }

        public override bool Unprovokable { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool ShowFameTitle { get { return false; } }
        public override bool ClickTitle { get { return false; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.None; } }

        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { };
            }
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

        private class InternalTimer : Timer
        {
            private KhalAnkur m_Mobile;
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
                if (m_Tick < 15)
                {
                    Point3D p = FindLocation(m_Mobile.Map, m_Mobile.Location, 7);
                    Effects.SendLocationEffect(p, m_Mobile.Map, 0x3789, 40, 1, 2062, 0x4);

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

            Server.Misc.Geometry.Circle2D(loc, pmmap, 7, (pnt, map) =>
            {
                if (map.CanFit(pnt, 0) && InLOS(pnt))
                    points.Add(pnt);
            });

            if (pmmap != Map.Internal && pmmap != null)
            {
                Server.Misc.Geometry.Circle2D(loc, pmmap, 6, (pnt, map) =>
                {
                    if (map.CanFit(pnt, 0) && InLOS(pnt) && Utility.RandomBool())
                        Effects.SendLocationEffect(pnt, map, 0x3789, 40, 1, 2062, 0x4);
                });

                Server.Misc.Geometry.Circle2D(loc, pmmap, 7, (pnt, map) =>
                {
                    if (map.CanFit(pnt, 0) && InLOS(pnt) && Utility.RandomBool())
                        Effects.SendLocationEffect(pnt, map, 0x3789, 40, 1, 2062, 0x4);
                });
            }

            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                IPooledEnumerable eable = GetMobilesInRange(6);

                foreach (Mobile from in eable)
                {
                    if (!from.Alive || from == this || from.AccessLevel > AccessLevel.Player)
                        continue;

                    if (from is PlayerMobile || (from is BaseCreature && (((BaseCreature)from).Controlled) || ((BaseCreature)from).Summoned))
                    {
                        Point3D point = points[Utility.Random(points.Count)];
                        from.MoveToWorld(point, pmmap);
                        Frozen = true;

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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Hue = 0;
        }
    }
}
