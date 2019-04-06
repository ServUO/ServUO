using System;
using Server;
using Server.Multis;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using System.Linq;

namespace Server.Mobiles
{
    public class Charydbis : BaseSeaChampion
    {
        public static readonly TimeSpan SpawnRate = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan TeleportRate = TimeSpan.FromSeconds(60);
        public static readonly int SpawnMax = 25;

        private List<Mobile> m_Tentacles = new List<Mobile>();
        private DateTime m_NextSpawn;
        private DateTime m_NextTeleport;

        public override bool CanDamageBoats { get { return true; } }
        public override TimeSpan BoatDamageCooldown { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(45, 80)); } }
        public override int MinBoatDamage { get { return 5; } }
        public override int MaxBoatDamage { get { return 15; } }
        public override int DamageRange { get { return 10; } }

        public override int Meat { get { return 5; } }
        public override double TreasureMapChance { get { return .50; } }
        public override int TreasureMapLevel { get { return 7; } }

        public override Type[] UniqueList { get { return new Type[] { typeof(FishermansHat), typeof(FishermansVest), typeof(FishermansEelskinGloves), typeof(FishermansTrousers) }; } }
        public override Type[] SharedList { get { return new Type[] { typeof(HelmOfVengence), typeof(RingOfTheSoulbinder), typeof(RuneEngravedPegLeg), typeof(CullingBlade) }; } }
        public override Type[] DecorativeList { get { return new Type[] { typeof(EnchantedBladeDeed), typeof(EnchantedVortexDeed) }; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Tentacles { get { return m_Tentacles.Count; } }

        [Constructable]
        public Charydbis() : this(null) { }

        public Charydbis(Mobile fisher)
            : base(fisher, AIType.AI_Mage, FightMode.Closest)
        {
            RangeFight = 8;

            Name = "charydbis";
            Body = 1244;
            BaseSoundID = 353;

            m_NextSpawn = DateTime.UtcNow + SpawnRate;
            m_NextTeleport = DateTime.UtcNow + TeleportRate;

            CanSwim = true;
            CantWalk = true;

            SetStr(533, 586);
            SetDex(113, 131);
            SetInt(110, 155);

            SetHits(30000);
            SetMana(8000);
            SetDamage(24, 33);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Wrestling, 120.1, 121.2);
            SetSkill(SkillName.Tactics, 120.15, 123.1);
            SetSkill(SkillName.MagicResist, 165.2, 178.7);
            SetSkill(SkillName.Anatomy, 111.0, 111.7);
            SetSkill(SkillName.Magery, 134.6, 140.5);
            SetSkill(SkillName.EvalInt, 200.8, 243.6);
            SetSkill(SkillName.Meditation, 565);

            Fame = 32000;
            Karma = -32000;
        }

        public void AddTentacle(Mobile tent)
        {
            if (!m_Tentacles.Contains(tent))
                m_Tentacles.Add(tent);
        }

        public void RemoveTentacle(Mobile tent)
        {
            if (m_Tentacles.Contains(tent))
                m_Tentacles.Remove(tent);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextSpawn < DateTime.UtcNow && m_Tentacles.Count < SpawnMax)
                SpawnTentacle();

            if (m_NextTeleport < DateTime.UtcNow)
                DoTeleport();
        }

        private Point3D m_LastLocation;
        private Map m_LastMap;

        public void DoTeleport()
        {
            if (this.Combatant == null || !(Combatant is Mobile))
            {
                m_NextTeleport = DateTime.UtcNow + TeleportRate;
                return;
            }

            m_LastLocation = this.Location;
            m_LastMap = this.Map;
            DoTeleportEffects(m_LastLocation, m_LastMap);

            this.Hidden = true;
            this.Internalize();

            DoAreaLightningAttack((Mobile)Combatant);

            Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerStateCallback(FinishTeleport), new object[]{Combatant.Location,Combatant});
            m_NextTeleport = DateTime.UtcNow + TeleportRate;
        }

        public void FinishTeleport(object o)
        {
            object[] ojs = (object[])o;
            Point3D focusLoc = (Point3D)ojs[0];
            Mobile focus = ojs[1] as Mobile;

            Map map = m_LastMap;
            Point3D newLoc = Point3D.Zero;
            BaseBoat boat = BaseBoat.FindBoatAt(focus, map);

            for (int i = 0; i < 25; i++)
            {
                if (boat != null)
                    newLoc = GetValidPoint(boat, map, 10);
                else
                {
                    int x = focusLoc.X + Utility.RandomMinMax(-12, 12);
                    int y = focusLoc.Y + Utility.RandomMinMax(-12, 12);
                    int z = map.GetAverageZ(x, y);

                    newLoc = new Point3D(x, y, z);
                }

                LandTile t = map.Tiles.GetLandTile(newLoc.X, newLoc.Y);

                if (!Spells.SpellHelper.CheckMulti(new Point3D(newLoc.X, newLoc.Y, newLoc.Z), map) && IsSeaTile(t))
                    break;
            }

            if (newLoc == Point3D.Zero || this.GetDistanceToSqrt(newLoc) > 15)
                newLoc = m_LastLocation;

            DoTeleportEffects(newLoc, map);
            this.Hidden = false;
            Timer.DelayCall(TimeSpan.FromSeconds(.5), new TimerStateCallback(TimedMoveToWorld), new object[]{newLoc,map,focus});
        }

        public void TimedMoveToWorld(object o)
        {
            object[] ojs = (object[])o;
            Point3D pnt = (Point3D)ojs[0];
            Map map = ojs[1] as Map;
            Mobile focus = ojs[2] as Mobile;

            MoveToWorld(pnt, map);
            Combatant = focus;

            DoAreaLightningAttack(focus);
        }

        public void DoAreaLightningAttack(Mobile focus)
        {
            if (focus == null)
                return;

            BaseBoat boat = BaseBoat.FindBoatAt(focus, focus.Map);

            if (boat != null)
            {
                foreach (var mob in boat.GetMobilesOnBoard().Where(m => CanBeHarmful(m, false) && m.Alive))
                {
                    double damage = Math.Max(40, Utility.RandomMinMax(50, 100) * ((double)Hits / (double)HitsMax));

                    mob.BoltEffect(0);
                    AOS.Damage((Mobile)mob, this, (int)damage, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);
                    mob.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                }
            }
        }

        public void DoTeleportEffects(Point3D p, Map map)
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    if (Math.Abs(x) == 2 && Math.Abs(y) == 2)
                        continue;

                    Point3D pnt = new Point3D(p.X + x, p.Y + y, this.Map.GetAverageZ(p.X + x, p.Y + y));
                    Effects.SendLocationEffect(pnt, map, 0x3728, 16, 4);
                }
            }

            Effects.PlaySound(p, map, 0x025);
            Effects.PlaySound(p, map, 0x026);
            Effects.PlaySound(p, map, 0x027);
        }

        private bool m_HasPushed;

        public override void DoDamageBoat(BaseGalleon galleon)
        {
            if (galleon == null)
                return;

            m_HasPushed = false;
            IPoint2D pnt = galleon;

            if(Combatant != null && galleon.Contains(Combatant))
                pnt = Combatant;

            Direction dir = Utility.GetDirection(this, pnt);
            Point3DList path = new Point3DList();

            for (int i = 0; i < DamageRange; i++)
            {
                int x = 0, y = 0;
                switch ((int)dir)
                {
                    case (int)Direction.Running:
                    case (int)Direction.North: { y -= i; break; }
                    case 129:
                    case (int)Direction.Right: { y -= i; x += i; break; }
                    case 130:
                    case (int)Direction.East: { x += i; break; }
                    case 131:
                    case (int)Direction.Down: { x += i; y += i; break; }
                    case 132:
                    case (int)Direction.South: { y += i; break; }
                    case 133:
                    case (int)Direction.Left: { y += i; x -= i; break; }
                    case 134:
                    case (int)Direction.West: { x -= i; break; }
                    case (int)Direction.ValueMask:
                    case (int)Direction.Up: { x -= i; y -= i; break; }
                }

                path.Add(this.X + x, this.Y + y, this.Z);
            }

            new EffectsTimer(this, path, dir, DamageRange);
        }

        public void OnTick(Point3DList path, Direction dir, int i)
        {
            if (path.Count > i)
            {
                Point3D point = path[i];
                int o = i - 1;

                Server.Effects.PlaySound(point, this.Map, 278);
                Server.Effects.PlaySound(point, this.Map, 279);

                for (int rn = 0; rn < (o * 2) + 1; rn++)
                {
                    int y = 0, x = 0, y2 = 0, x2 = 0;
                    bool diag = false;
                    switch ((int)dir)
                    {
                        case (int)Direction.Running:
                        case (int)Direction.North: { x = x - o + rn; break; }
                        case 129:
                        case (int)Direction.Right: { x = x - o + rn; y = y - o + rn; break; }
                        case 130:
                        case (int)Direction.East: { y = y - o + rn; break; }
                        case 131:
                        case (int)Direction.Down: { y = y - o + rn; x = x + o - rn; break; }
                        case 132:
                        case (int)Direction.South: { x = x + o - rn; break; }
                        case 133:
                        case (int)Direction.Left: { x = x + o - rn; y = y + o - rn; break; }
                        case 134:
                        case (int)Direction.West: { y = y + o - rn; break; }
                        case (int)Direction.ValueMask:
                        case (int)Direction.Up: { y = y + o - rn; x = x - o + rn; break; }
                    }
                    switch ((int)dir)
                    {
                        case 129:
                        case (int)Direction.Right: { y2++; diag = true; break; }
                        case 131:
                        case (int)Direction.Down: { x2--; diag = true; break; }
                        case 133:
                        case (int)Direction.Left: { y2--; diag = true; break; }
                        case (int)Direction.ValueMask:
                        case (int)Direction.Up: { x2++; diag = true; break; }
                        default: { break; }
                    }

                    Point3D ep = new Point3D(point.X + x, point.Y + y, point.Z);
                    Point3D ep2 = new Point3D(ep.X + x2, ep.Y + y2, ep.Z);

                    if (diag && i >= ((2 * path.Count) / 3))
                        return;

                    Point3D p;
                    if (diag && rn < (o * 2))
                        p = ep2;
                    else
                        p = ep;

                    if (Spells.SpellHelper.CheckMulti(p, this.Map))
                    {
                        BaseGalleon galleon = BaseGalleon.FindGalleonAt(p, this.Map);
                        if (galleon != null && !m_HasPushed)
                        {
                            int damage = Utility.RandomMinMax(MinBoatDamage, MaxBoatDamage);
                            galleon.OnTakenDamage(this, damage);

                            galleon.StartMove(dir, 1,0x2, galleon.SlowDriftInterval, true, false);
                            m_HasPushed = true;
                        }
                        continue;
                    }

                    LandTile t = this.Map.Tiles.GetLandTile(x, y);

                    if (IsSeaTile(t))
                    {
                        Mobile spawn = new EffectSpawn();
                        spawn.MoveToWorld(p, this.Map);
                    }
                }
            }
        }

        public class EffectSpawn : BaseCreature
        {
            public EffectSpawn()
                : base(AIType.AI_Vendor, FightMode.Closest, 10, 1, 0.2, 0.4)
            {
                Body = 16;
                BaseSoundID = 278;
                CantWalk = true;
                CanSwim = false;
                Frozen = true;

                SetHits(150000);

                SetResistance(ResistanceType.Physical, 100);
                SetResistance(ResistanceType.Fire, 100);
                SetResistance(ResistanceType.Cold, 100);
                SetResistance(ResistanceType.Poison, 100);
                SetResistance(ResistanceType.Energy, 100);

                Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerCallback(DoDelete));
            }

            public override bool DeleteCorpseOnDeath
            {
                get
                {
                    return true;
                }
            }

            public void DoDelete()
            {
                if (this.Alive)
                    Kill();
            }

            public override void OnDelete()
            {
                Effects.SendLocationEffect(this.Location, this.Map, 0x352D, 16, 4);
                Effects.PlaySound(this.Location, this.Map, 0x364);

                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 1, 14, 0, 7, 9915, 0);

                base.OnDelete();
            }

            public override bool AutoDispel { get { return true; } }
            public override double AutoDispelChance { get { return 1.0; } }
            public override bool BardImmune { get { return true; } }
            public override bool Unprovokable { get { return true; } }
            public override bool Uncalmable { get { return true; } }
            public override Poison PoisonImmune { get { return Poison.Lethal; } }
            
            public override int Meat { get { return 1; } }

            public EffectSpawn(Serial serial)
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

        private class EffectsTimer : Timer
        {
            private Direction m_Dir;
            private int m_I, m_IMax;
            private Point3DList m_Path;
            private Charydbis m_Mobile;

            public EffectsTimer(Charydbis mobile, Point3DList path, Direction dir, int imax)
                : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(0.25))
            {
                m_Dir = dir;
                m_I = 1;
                m_IMax = imax;
                m_Path = path;
                m_Mobile = mobile;
                Priority = TimerPriority.FiftyMS;
                this.Start();
            }

            protected override void OnTick()
            {
                m_Mobile.OnTick(m_Path, m_Dir, m_I);

                if (m_I >= m_IMax)
                {
                    this.Stop();
                    return;
                }

                m_I++;
            }
        }

        public void SpawnTentacle()
        {
            if (this.Combatant == null)
            {
                m_NextSpawn = DateTime.UtcNow + SpawnRate;
                return;
            }

            Map map = this.Map;

            List<Mobile> list = new List<Mobile>();

            IPooledEnumerable eable = this.GetMobilesInRange(15);
            foreach (Mobile m in eable)
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }
            eable.Free();

            if (list.Count > 0)
            {
                Mobile spawn = list[Utility.Random(list.Count)];
                BaseBoat boat = BaseBoat.FindBoatAt(spawn, map);
                Point3D loc = spawn.Location;

                for (int i = 0; i < 25; i++)
                {
                    Point3D spawnLoc = Point3D.Zero;

                    if (boat != null)
                        spawnLoc = GetValidPoint(boat, map, 4);
                    else
                    {
                        int y = Utility.RandomMinMax(loc.X - 10, loc.Y + 10);
                        int x = Utility.RandomMinMax(loc.X - 10, loc.Y + 10);
                        int z = map.GetAverageZ(x, y);

                        spawnLoc = new Point3D(x, y, z);
                    }

                    if (Spells.SpellHelper.CheckMulti(spawnLoc, map))
                        continue;

                    LandTile t = map.Tiles.GetLandTile(spawnLoc.X, spawnLoc.Y);

                    if (IsSeaTile(t) && spawnLoc != Point3D.Zero)
                    {
                        GiantTentacle tent = new GiantTentacle(this);

                        tent.MoveToWorld(spawnLoc, map);
                        tent.Home = tent.Location;
                        tent.RangeHome = 15;
                        tent.Team = this.Team;
                        if (spawn != this)
                            tent.Combatant = spawn;
                        break;
                    }

                }
            }

            m_NextSpawn = DateTime.UtcNow + SpawnRate;
        }

        public bool IsSeaTile(LandTile t)
        {
            return t.Z == -5 && ((t.ID >= 0xA8 && t.ID <= 0xAB) || (t.ID >= 0x136 && t.ID <= 0x137));
        }

        public override bool OnBeforeDeath()
        {
            if (Map == Map.Internal)
                MoveToWorld(m_LastLocation, m_LastMap);

            if (CharydbisSpawner.SpawnInstance != null && CharydbisSpawner.SpawnInstance.Charydbis == this)
                CharydbisSpawner.SpawnInstance.OnCharybdisKilled();

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            int drop = Utility.RandomMinMax(2, 5);

            for (int i = 0; i < drop; i++)
            {
                Type pieType = m_Pies[Utility.Random(m_Pies.Length)];

                Item pie = Loot.Construct(pieType);

                if (pie != null)
                    c.DropItem(pie);
            }

            drop = Utility.RandomMinMax(2, 5);

            for (int i = 0; i < drop; i++)
            {
                Type steakType = m_Steaks[Utility.Random(m_Steaks.Length)];

                Item steak = Loot.Construct(steakType);

                if (steak != null)
                    c.DropItem(steak);
            }

            c.DropItem(new MessageInABottle(c.Map));
            c.DropItem(new SpecialFishingNet());
            c.DropItem(new SpecialFishingNet());
            c.DropItem(new SpecialFishingNet());
            c.DropItem(new SpecialFishingNet());

            FishingPole pole = new FishingPole();
            BaseRunicTool.ApplyAttributesTo(pole, false, 0, Utility.RandomMinMax(2, 5), 50, 100);
            c.DropItem(pole);

            #region TOL
            if(Core.TOL)
                SkillMasteryPrimer.CheckPrimerDrop(this);
            #endregion
        }

        public override void Delete()
        {
            if (m_Tentacles != null)
            {
                List<Mobile> tents = new List<Mobile>(m_Tentacles);
                for (int i = 0; i < tents.Count; i++)
                {
                    if (tents[i] != null)
                        tents[i].Kill();
                }
            }

            base.Delete();
        }

        private Type[] m_Pies = new Type[]
        {
            typeof(AutumnDragonfishPie),
            typeof(BlueLobsterPie),
            typeof(BullFishPie),
            typeof(CrystalFishPie),
            typeof(FairySalmonPie),
            typeof(FireFishPie),
            typeof(GiantKoiPie),
            typeof(GreatBarracudaPie),
            typeof(HolyMackerelPie),
            typeof(LavaFishPie),
            typeof(ReaperFishPie),
            typeof(SpiderCrabPie),
            typeof(StoneCrabPie),
            typeof(SummerDragonfishPie),
            typeof(UnicornFishPie),
            typeof(YellowtailBarracudaPie),
        };

        private Type[] m_Steaks = new Type[]
        {
            typeof(AutumnDragonfishSteak),
            typeof(BlueLobsterMeat),
            typeof(BullFishSteak),
            typeof(CrystalFishSteak),
            typeof(FairySalmonSteak),
            typeof(FireFishSteak),
            typeof(GiantKoiSteak),
            typeof(GreatBarracudaSteak),
            typeof(HolyMackerelSteak),
            typeof(LavaFishSteak),
            typeof(ReaperFishSteak),
            typeof(SpiderCrabMeat),
            typeof(StoneCrabMeat),
            typeof(SummerDragonfishSteak),
            typeof(UnicornFishSteak),
            typeof(YellowtailBarracudaSteak),
        };

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
        }

        public Charydbis(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Tentacles.Count);
            foreach (Mobile tent in m_Tentacles)
                writer.Write(tent);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int cnt = reader.ReadInt();
            for (int i = 0; i < cnt; i++)
            {
                Mobile tent = reader.ReadMobile();
                if (tent != null && !tent.Deleted && tent.Alive)
                    m_Tentacles.Add(tent);
            }

            m_NextSpawn = DateTime.UtcNow;
        }
    }

    public class GiantTentacle : BaseCreature
    {
        private Mobile m_Master;

        public GiantTentacle(Mobile master) : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            if (master is Charydbis)
            {
                m_Master = master;
                ((Charydbis)master).AddTentacle(this);
            }

            Name = "a giant tentacle";
            Body = 1245;
            BaseSoundID = 0x161;

            CanSwim = true;

            SetStr(127, 155);
            SetDex(66, 85);
            SetInt(102, 123);

            SetHits(105, 113);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 10, 25);
            SetResistance(ResistanceType.Cold, 10, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.Wrestling, 52.0, 70.0);
            SetSkill(SkillName.Tactics, 0.0);
            SetSkill(SkillName.MagicResist, 100.4, 113.5);
            SetSkill(SkillName.Anatomy, 1.0, 0.0);
            SetSkill(SkillName.Magery, 60.2, 72.4);
            SetSkill(SkillName.EvalInt, 60.1, 73.4);
            SetSkill(SkillName.Meditation, 100.0);

            Fame = 2500;
            Karma = -2500;
        }


        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override void Delete()
        {
            if (m_Master != null && m_Master is Charydbis)
                ((Charydbis)m_Master).RemoveTentacle(this);

            base.Delete();
        }

        public GiantTentacle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Master);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Master = reader.ReadMobile();
        }
    }
}