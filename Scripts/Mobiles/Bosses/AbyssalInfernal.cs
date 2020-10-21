using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an abyssal infernal corpse")]
    public class AbyssalInfernal : BaseChampion
    {
        private static readonly Dictionary<Mobile, Point3D> m_Table = new Dictionary<Mobile, Point3D>();

        private DateTime m_NextAbility;

        [Constructable]
        public AbyssalInfernal()
            : base(AIType.AI_Mage)
        {
            Body = 713;
            Name = "Abyssal Infernal";

            SetStr(1200, 1300);
            SetDex(100, 125);
            SetInt(600, 700);

            SetHits(30000);
            SetStam(203, 650);

            SetDamage(11, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Anatomy, 110.0, 120.0);
            SetSkill(SkillName.Magery, 130.0, 140.0);
            SetSkill(SkillName.EvalInt, 120.0, 140.0);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 110.0, 120.0);
            SetSkill(SkillName.Meditation, 100.0);

            Fame = 28000;
            Karma = -28000;
        }

        public AbyssalInfernal(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType => ChampionSkullType.None;
        public override Type[] UniqueList => new[] { typeof(TongueOfTheBeast), typeof(DeathsHead), typeof(WallOfHungryMouths), typeof(AbyssalBlade) };
        public override Type[] SharedList => new[] { typeof(RoyalGuardInvestigatorsCloak), typeof(DetectiveBoots), typeof(JadeArmband) };
        public override Type[] DecorativeList => new[] { typeof(MagicalDoor) };
        public override MonsterStatuetteType[] StatueTypes => new[] { MonsterStatuetteType.AbyssalInfernal, MonsterStatuetteType.ArchDemon };

        public override Poison PoisonImmune => Poison.Lethal;

        public override ScaleType ScaleType => ScaleType.All;
        public override int Scales => 20;

        public override int GetAttackSound() { return 0x5D4; }
        public override int GetDeathSound() { return 0x5D5; }
        public override int GetHurtSound() { return 0x5D6; }
        public override int GetIdleSound() { return 0x5D7; }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomBool())
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        c.DropItem(new HornAbyssalInferno());
                        break;
                    case 1:
                        c.DropItem(new NetherCycloneScroll());
                        break;
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (m_NextAbility < DateTime.UtcNow)
            {
                switch (Utility.Random(3))
                {
                    case 0: { DoCondemn(); break; }
                    case 1: { DoSummon(); break; }
                    case 2: { DoNuke(); break; }
                }

                m_NextAbility = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(35, 45));
            }
        }

        private Mobile GetRandomTarget(int range, bool playersOnly)
        {
            List<Mobile> list = GetTargets(range, playersOnly);
            Mobile m = null;

            if (list != null && list.Count > 0)
            {
                m = list[Utility.Random(list.Count)];
                ColUtility.Free(list);
            }

            return m;
        }

        private List<Mobile> GetTargets(int range, bool playersOnly)
        {
            List<Mobile> targets = new List<Mobile>();

            IPooledEnumerable eable = GetMobilesInRange(range);
            foreach (Mobile m in eable)
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (!playersOnly && m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != Team))
                    targets.Add(m);
                else if (m.Player)
                    targets.Add(m);
            }
            eable.Free();

            return targets;
        }

        #region Condemn
        private static readonly Point3D[] _Locs = {
            new Point3D(6949, 701, 32),
            new Point3D(6941, 761, 32),
            new Point3D(7015, 688, 32),
            new Point3D(7043, 751, 32),
            new Point3D(6999, 798, 32),
        };

        public void DoCondemn()
        {
            Map map = Map;

            if (map != null)
            {
                Mobile toCondemn = GetRandomTarget(8, true);

                if (toCondemn != null && !m_Table.ContainsKey(toCondemn))
                {
                    m_Table[toCondemn] = toCondemn.Location;

                    Point3D loc = _Locs[Utility.Random(_Locs.Length)];
                    toCondemn.MoveToWorld(loc, map);

                    toCondemn.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                    toCondemn.PlaySound(0x1FE);

                    toCondemn.Frozen = true;

                    int seconds = 15;

                    BuffInfo.AddBuff(toCondemn, new BuffInfo(BuffIcon.TrueFear, 1153791, 1153827, TimeSpan.FromSeconds(seconds), toCondemn));

                    Timer.DelayCall(TimeSpan.FromSeconds(seconds), () =>
                    {
                        toCondemn.Frozen = false;
                        toCondemn.SendLocalizedMessage(1005603); // You can move again!

                        loc = m_Table.ToList().Find(x => x.Key == toCondemn).Value;
                        toCondemn.MoveToWorld(loc, map);

                        m_Table.Remove(toCondemn);
                    });
                }
            }
        }
        #endregion

        public void DoNuke()
        {
            if (!Alive || Map == null)
                return;

            Say(1112362); // You will burn to a pile of ash!

            int range = 8;

            Effects.PlaySound(Location, Map, 0x349);

            //Flame Columns
            for (int i = 0; i < 2; i++)
            {
                Geometry.Circle2D(Location, Map, i, (pnt, map) =>
                {
                    Effects.SendLocationParticles(EffectItem.Create(pnt, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                });
            }

            //Flash then boom
            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
            {
                if (Alive && Map != null)
                {
                    Effects.PlaySound(Location, Map, 0x44B);

                    Packet flash = ScreenLightFlash.Instance;
                    IPooledEnumerable e = Map.GetClientsInRange(Location, (range * 4) + 5);

                    foreach (NetState ns in e)
                    {
                        if (ns.Mobile != null)
                            ns.Mobile.Send(flash);
                    }

                    e.Free();

                    for (int i = 0; i < range; i++)
                    {
                        Geometry.Circle2D(Location, Map, i, (pnt, map) =>
                        {
                            Effects.SendLocationEffect(pnt, map, 14000, 14, 10, Utility.RandomMinMax(2497, 2499), 2);
                        });
                    }
                }
            });

            IPooledEnumerable eable = GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if ((m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && CanBeHarmful(m))
                    Timer.DelayCall(TimeSpan.FromSeconds(1.75), new TimerStateCallback(DoDamage_Callback), m);
            }

            eable.Free();
        }

        private void DoDamage_Callback(object o)
        {
            Mobile m = o as Mobile;

            if (m != null && Map != null)
            {
                IMount mount = m.Mount;

                if (mount != null)
                {
                    if (m is PlayerMobile)
                        ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                    else
                        mount.Rider = null;
                }
                else if (m.Flying)
                {
                    ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                }

                DoHarmful(m);
                AOS.Damage(m, this, Utility.RandomMinMax(90, 110), 0, 0, 0, 0, 100);

                m.Frozen = true;

                Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                {
                    m.Frozen = false;
                    m.SendLocalizedMessage(1005603); // You can move again!
                });
            }
        }


        #region Spawn
        public List<BaseCreature> SummonedHelpers { get; set; }

        private static readonly Type[] SummonTypes =
        {
            typeof(HellHound),      typeof(Phoenix),
            typeof(FireSteed),      typeof(FireElemental),
            typeof(LavaElemental),  typeof(FireGargoyle),
            typeof(Efreet),         typeof(Gargoyle),
            typeof(Nightmare),      typeof(HellCat)
        };

        public int TotalSummons()
        {
            if (SummonedHelpers == null || SummonedHelpers.Count == 0)
                return 0;

            return SummonedHelpers.Count(bc => bc != null && bc.Alive);
        }

        public virtual void DoSummon()
        {
            if (Map == null || TotalSummons() > 0)
                return;

            Type type = SummonTypes[Utility.Random(SummonTypes.Length)];

            for (int i = 0; i < 3; i++)
            {
                if (Combatant == null)
                    return;

                Point3D p = Combatant.Location;

                for (int j = 0; j < 10; j++)
                {
                    int x = Utility.RandomMinMax(p.X - 3, p.X + 3);
                    int y = Utility.RandomMinMax(p.Y - 3, p.Y + 3);
                    int z = Map.GetAverageZ(x, y);

                    if (Map.CanSpawnMobile(x, y, z))
                    {
                        p = new Point3D(x, y, z);
                        break;
                    }
                }

                BaseCreature spawn = Activator.CreateInstance(type) as BaseCreature;

                if (spawn != null)
                {
                    spawn.MoveToWorld(p, Map);
                    spawn.Team = Team;
                    spawn.SummonMaster = this;

                    Timer.DelayCall(TimeSpan.FromSeconds(1), o =>
                    {
                        BaseCreature s = o;

                        if (s != null && s.Combatant != null)
                        {
                            if (!(s.Combatant is PlayerMobile) || !((PlayerMobile)s.Combatant).HonorActive)
                                s.Combatant = Combatant;
                        }

                    }, spawn);

                    AddHelper(spawn);
                }
            }
        }

        protected virtual void AddHelper(BaseCreature bc)
        {
            if (SummonedHelpers == null)
                SummonedHelpers = new List<BaseCreature>();

            if (!SummonedHelpers.Contains(bc))
                SummonedHelpers.Add(bc);
        }

        public override void Delete()
        {
            base.Delete();

            if (SummonedHelpers != null)
            {
                ColUtility.Free(SummonedHelpers);
            }
        }
        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(SummonedHelpers == null ? 0 : SummonedHelpers.Count);

            if (SummonedHelpers != null)
                SummonedHelpers.ForEach(m => writer.Write(m));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    BaseCreature summon = reader.ReadMobile() as BaseCreature;

                    if (summon != null)
                    {
                        if (SummonedHelpers == null)
                            SummonedHelpers = new List<BaseCreature>();

                        SummonedHelpers.Add(summon);
                    }
                }
            }

            m_NextAbility = DateTime.UtcNow;
        }
    }
}
