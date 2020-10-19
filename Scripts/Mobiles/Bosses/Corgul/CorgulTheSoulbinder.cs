using Server.Items;
using Server.Misc;
using Server.Regions;
using Server.Engines.CannedEvil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class CorgulTheSoulBinder : BaseSeaChampion
    {
        private DateTime m_NextDismount;
        private DateTime m_NextArea;
        private DateTime m_NextReturn;
        private bool m_HasDone2ndSpawn;
        private CorgulAltar m_Altar;
        private readonly List<BaseCreature> m_Helpers = new List<BaseCreature>();

        public override bool CanDamageBoats => false;
        public override bool TaintedLifeAura => true;
        public override int Meat => 5;
        public override double TreasureMapChance => .25;
        public override int TreasureMapLevel => 7;
        public override Poison PoisonImmune => Poison.Deadly;

        public override bool TeleportsTo => true;
        public override TimeSpan TeleportDuration => TimeSpan.FromSeconds(Utility.RandomMinMax(10, 50));
        public override double TeleportProb => 1.0;
        public override bool TeleportsPets => true;

        public override Type[] UniqueList => new Type[] { typeof(CorgulsEnchantedSash), typeof(CorgulsHandbookOnMysticism), typeof(CorgulsHandbookOnTheUndead) };
        public override Type[] SharedList => new Type[] { typeof(HelmOfVengence), typeof(RingOfTheSoulbinder), typeof(RuneEngravedPegLeg), typeof(CullingBlade) };
        public override Type[] DecorativeList => new Type[] { typeof(EnchantedBladeDeed), typeof(EnchantedVortexDeed) };

        public override bool NoGoodies => true;
        public override bool RestrictedToFelucca => false;

        private readonly int _SpawnPerLoc = 15;

        private readonly Point3D[] _SpawnLocs =
        {
            new Point3D(6447, 1262, 10),
            new Point3D(6424, 1279, 10),
            new Point3D(6406, 1250, 10),
            new Point3D(6423, 1220, 10),
            new Point3D(6461, 1237, 10),
        };

        [Constructable]
        public CorgulTheSoulBinder()
            : this(null)
        {
        }

        public CorgulTheSoulBinder(CorgulAltar altar)
            : base(null, AIType.AI_NecroMage, FightMode.Closest)
        {
            m_Altar = altar;
            Name = "Corgul the Soulbinder";
            BaseSoundID = 609;
            Body = 0x4C;
            Hue = 2076;

            m_NextDismount = DateTime.UtcNow;
            m_NextArea = DateTime.UtcNow;
            m_HasDone2ndSpawn = false;

            SetStr(800, 900);
            SetDex(121, 165);
            SetInt(300, 400);

            SetMana(4500);
            SetHits(65000);

            SetDamage(19, 24);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Fire, 10);
            SetDamageType(ResistanceType.Cold, 30);
            SetDamageType(ResistanceType.Poison, 40);
            SetDamageType(ResistanceType.Energy, 10);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 85, 95);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Wrestling, 110.0, 120.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.Magery, 110.9, 120.0);
            SetSkill(SkillName.EvalInt, 110.9, 120.0);
            SetSkill(SkillName.Meditation, 110.9, 120.0);
            SetSkill(SkillName.Necromancy, 110.9, 120.0);
            SetSkill(SkillName.SpiritSpeak, 110.9, 120.0);

            Fame = 25000;
            Karma = -25000;

            m_NextReturn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(120, 180));
        }

        public double SharedChance => Map != null && Map.Rules == MapRules.FeluccaRules ? .12 : .08;
        public double DecorativeChance => Map != null && Map.Rules == MapRules.FeluccaRules ? .40 : .25;

        public override bool OnBeforeDeath()
        {
            List<DamageStore> rights = GetLootingRights();

            Mobile winner = null;

            if (rights != null && rights.Count > 0)
            {
                rights.Sort();

                if (rights.Count >= 5)
                    winner = rights[Utility.Random(5)].m_Mobile;
                else if (rights.Count > 1)
                    winner = rights[Utility.Random(rights.Count)].m_Mobile;
                else
                    winner = rights[0].m_Mobile;
            }

            if (winner != null)
                GiveArtifact(winner, CreateArtifact(UniqueList));

            if (IsSoulboundEnemies)
                EtherealSandShower.Do(Location, Map, 50, 100, 500);

            return base.OnBeforeDeath();
        }

        public override Item GetArtifact()
        {
            double random = Utility.RandomDouble();

            if (SharedChance >= random)
                return CreateArtifact(SharedList);
            else if (DecorativeChance >= random)
                return CreateArtifact(DecorativeList);
            return null;
        }

        public void SpawnHelpers()
        {
            foreach (Point3D pnt in _SpawnLocs)
            {
                for (int i = 0; i < _SpawnPerLoc; i++)
                {
                    BaseCreature bc;

                    switch (Utility.Random(7))
                    {
                        default:
                        case 0: bc = new BoundSoul(); break;
                        case 1: bc = new SoulboundApprenticeMage(); break;
                        case 2: bc = new SoulboundBattleMage(); break;
                        case 3: bc = new SoulboundPirateCaptain(); break;
                        case 4: bc = new SoulboundPirateRaider(); break;
                        case 5: bc = new SoulboundSpellSlinger(); break;
                        case 6: bc = new SoulboundSwashbuckler(); break;
                    }

                    m_Helpers.Add(bc);
                    SpawnMobile(bc, pnt);
                }
            }
        }

        public void SpawnMobile(BaseCreature bc, Point3D p)
        {
            if (Map == null || bc == null)
            {
                if (bc != null)
                    bc.Delete();
                return;
            }

            int x, y;

            for (int i = 0; i < 25; i++)
            {
                x = Utility.RandomMinMax(p.X - 4, p.X + 4);
                y = Utility.RandomMinMax(p.Y - 4, p.Y + 4);
                var z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(x, y, z))
                {
                    p = new Point3D(x, y, z);
                    break;
                }
            }

            bc.MoveToWorld(p, Map);
            bc.Home = p;
            bc.RangeHome = 5;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextReturn < DateTime.UtcNow)
            {
                Point3D p = CorgulAltar.SpawnLoc;

                if (Region.IsPartOf<CorgulRegion>() && !Utility.InRange(Location, p, 15))
                {
                    PlaySound(0x1FE);
                    FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);

                    Location = p;
                    ProcessDelta();

                    PlaySound(0x1FE);
                    FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);

                    m_NextReturn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(120, 180));
                }
            }

            if (Combatant == null)
                return;

            if (DateTime.UtcNow > m_NextDismount && 0.1 > Utility.RandomDouble())
                DoDismount();

            else if (DateTime.UtcNow > m_NextArea && 0.1 > Utility.RandomDouble())
                DoAreaAttack();

            if (!m_HasDone2ndSpawn && m_Helpers.Count > 0)
            {
                if (m_Helpers.Where(bc => bc.Alive && !bc.Deleted).Count() == 0)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(5), SpawnHelpers);
                    m_HasDone2ndSpawn = true;
                }
            }
        }

        public void DoDismount()
        {
            List<Mobile> targets = new List<Mobile>();

            IPooledEnumerable eable = GetMobilesInRange(12);
            foreach (Mobile mob in eable)
            {
                if (!CanBeHarmful(mob) || mob == this)
                    continue;
                if (mob is BaseCreature && (((BaseCreature)mob).Controlled || ((BaseCreature)mob).Summoned || ((BaseCreature)mob).Team != Team))
                    targets.Add(mob);
                else if (mob is PlayerMobile && mob.Alive)
                    targets.Add(mob);
            }
            eable.Free();

            PlaySound(0x2F3);
            for (int i = 0; i < targets.Count; ++i)
            {
                Mobile m = targets[i];

                if (m != null && !m.Deleted && m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;

                    if (pm.Mounted || pm.Flying)
                    {
                        pm.SetMountBlock(BlockMountType.DismountRecovery, TimeSpan.FromSeconds(10), true);
                    }
                }

                double damage = m.Hits * 0.6;
                if (damage < 10.0)
                    damage = 10.0;
                else if (damage > 75.0)
                    damage = 75.0;
                DoHarmful(m);
                AOS.Damage(m, this, (int)damage, 100, 0, 0, 0, 0);
                if (m.Alive && m.Body.IsHuman && !m.Mounted)
                    m.Animate(20, 7, 1, true, false, 0);
            }


            m_NextDismount = DateTime.UtcNow + TimeSpan.FromMinutes(2);
        }

        public void DoAreaAttack()
        {
            int range = 18;

            new InternalTimer(this, range);

            IPooledEnumerable eable = GetMobilesInRange(range);
            foreach (Mobile m in eable)
            {
                if ((m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && CanBeHarmful(m))
                    Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(DoDamage_Callback), m);
            }
            eable.Free();

            m_NextArea = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(60, 180));
        }

        public void DoDamage_Callback(object o)
        {
            Mobile m = (Mobile)o;

            if (m != null)
            {
                DoHarmful(m);
                AOS.Damage(m, this, Utility.RandomMinMax(100, 150), 0, 100, 0, 0, 0);

                if (Utility.RandomBool())
                {
                    WeaponAbility bleed = WeaponAbility.BleedAttack;
                    bleed.OnHit(this, m, 0);
                }
            }
        }

        public void DoEffect(Point3D p, Map map)
        {
            int[] effect =
            {
                14000, 14013
            };

            Effects.PlaySound(p, map, 0x307);
            Effects.SendLocationEffect(p, map, Utility.RandomBool() ? 14000 : 14013, 20);
        }

        private class InternalTimer : Timer
        {
            private readonly CorgulTheSoulBinder m_Mobile;
            private int m_Tick;

            public InternalTimer(CorgulTheSoulBinder mob, int range)
                : base(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), range)
            {
                m_Tick = 1;
                m_Mobile = mob;
                Priority = TimerPriority.FiftyMS;
                Start();
            }

            protected override void OnTick()
            {
                Geometry.Circle2D(m_Mobile.Location, m_Mobile.Map, m_Tick, m_Mobile.DoEffect);

                m_Tick++;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 6);
            AddLoot(LootPack.LootItemCallback(RandomRecipe, 10.0, 1, false, false));
        }

        private Item RandomRecipe(IEntity e)
        {
            return Utility.RandomBool() ? new RecipeScroll(1101) : new RecipeScroll(1109);
        }

        public override Item GetPowerScroll()
        {
            return ChampionSpawn.CreateRandomSoT(Map != null && Map.Rules == MapRules.FeluccaRules);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Altar != null)
                m_Altar.OnBossKilled();

            c.DropItem(new MessageInABottle(c.Map));
            c.DropItem(new SpecialFishingNet());
            c.DropItem(new SpecialFishingNet());

            if (m_Helpers != null)
            {
                foreach (BaseCreature bc in m_Helpers)
                {
                    RegisterDamageTo(bc);

                    if (bc != null && bc.Alive)
                        bc.Kill();
                }
            }
        }

        public static void CheckDropSOT(BaseCreature bc)
        {
            if (bc == null)
                return;

            var killer = bc.FindMostRecentDamager(false);
            var creature = killer as BaseCreature;

            if (creature != null)
            {
                killer = creature.GetMaster();
            }

            if (killer is PlayerMobile && Utility.RandomDouble() < ChampionSystem.ScrollChance * 10)
            {
                PlayerMobile pm = (PlayerMobile)killer;

                if (Utility.RandomDouble() < ChampionSystem.TranscendenceChance)
                {
                    ChampionSpawn.GiveScrollTo(pm, ChampionSpawn.CreateRandomSoT(bc.Map != null && bc.Map.Rules == MapRules.FeluccaRules));
                }
            }
        }

        public CorgulTheSoulBinder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
            writer.Write(m_HasDone2ndSpawn);
            writer.Write(m_Altar);
            writer.Write(m_Helpers.Count);
            foreach (BaseCreature bc in m_Helpers)
                writer.Write(bc);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_HasDone2ndSpawn = reader.ReadBool();
                    goto case 0;
                case 0:
                    m_Altar = reader.ReadItem() as CorgulAltar;

                    int cnt = reader.ReadInt();
                    for (int i = 0; i < cnt; i++)
                    {
                        BaseCreature bc = reader.ReadMobile() as BaseCreature;
                        if (bc != null)
                            m_Helpers.Add(bc);
                    }
                    break;
            }

            m_NextDismount = DateTime.UtcNow;
            m_NextArea = DateTime.UtcNow;
        }
    }

    public class EtherealSandShower
    {
        public static void Do(Point3D center, Map map, int piles, int minAmount, int maxAmount)
        {
            new GoodiesTimer(center, map, piles, minAmount, maxAmount).Start();
        }

        private class GoodiesTimer : Timer
        {
            private readonly Map m_Map;
            private readonly Point3D m_Location;
            private readonly int m_PilesMax;
            private int m_PilesDone;
            private readonly int m_MinAmount;
            private readonly int m_MaxAmount;

            public GoodiesTimer(Point3D center, Map map, int piles, int minAmount, int maxAmount)
                : base(TimeSpan.FromSeconds(0.25d), TimeSpan.FromSeconds(0.25d))
            {
                m_Location = center;
                m_Map = map;
                m_PilesMax = piles;
                m_MinAmount = minAmount;
                m_MaxAmount = maxAmount;
            }

            protected override void OnTick()
            {
                if (m_PilesDone >= m_PilesMax)
                {
                    Stop();
                    return;
                }

                Point3D p = FindGoldLocation(m_Map, m_Location, m_PilesMax / 8);
                EtherealSand g = new EtherealSand(m_MinAmount, m_MaxAmount);
                g.MoveToWorld(p, m_Map);

                switch (Utility.Random(3))
                {
                    case 0: // Fire column
                        Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                        Effects.PlaySound(g, g.Map, 0x208);
                        break;
                    case 1: // Explosion
                        Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                        Effects.PlaySound(g, g.Map, 0x307);
                        break;
                    case 2: // Ball of fire
                        Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);
                        break;
                }
                ++m_PilesDone;
            }

            private static Point3D FindGoldLocation(Map map, Point3D center, int range)
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
    }
}
