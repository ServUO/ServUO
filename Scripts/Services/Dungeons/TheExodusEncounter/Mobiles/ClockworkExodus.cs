using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a vile corpse")]
    public class ClockworkExodus : BaseCreature
    {
        public static int m_MinHits;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinHits
        {
            get { return m_MinHits; }
            set { m_MinHits = value; }
        }

        public static List<ClockworkExodus> Instances { get; set; }

        private static readonly Type[] m_Artifact = new Type[]
        {
            typeof(ScrollofValiantCommendation),
            typeof(BracersofAlchemicalDevastation),
            typeof(HygieiasAmulet),
            typeof(Asclepius),
            typeof(ClockworkLeggings),
            typeof(DupresSword),
            typeof(GargishDupresSword),
            typeof(GargishClockworkLeggings),
            typeof(GargishBracersofAlchemicalDevastation),
            typeof(GargishAsclepius)
        };

        private Point3D m_LastTarget;

        [Constructable]
        public ClockworkExodus() : base(AIType.AI_Mystic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Clockwork Exodus";
            Body = 1248;
            BaseSoundID = 639;
            Hue = 2500;
            Female = true;

            SetStr(851, 950);
            SetDex(581, 683);
            SetInt(601, 750);

            SetHits(50000);
            SetStam(507, 669);

            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 75, 90);
            SetResistance(ResistanceType.Fire, 50, 65);
            SetResistance(ResistanceType.Cold, 45, 60);
            SetResistance(ResistanceType.Poison, 45, 60);
            SetResistance(ResistanceType.Energy, 45, 60);

            SetSkill(SkillName.EvalInt, 130.0);
            SetSkill(SkillName.Mysticism, 120);
            SetSkill(SkillName.MagicResist, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 105.0);
            SetSkill(SkillName.Wrestling, 90.1, 105.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.Poisoning, 120.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Anatomy, 120.0);
            SetSkill(SkillName.Healing, 120.0);

            Fame = 24000;
            Karma = -24000;

            m_MinHits = Hits;

            if (Instances == null)
                Instances = new List<ClockworkExodus>();

            Instances.Add(this);

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetAreaEffect(AreaEffect.AuraOfEnergy);
        }

        public static void DistributeRandomArtifact(BaseCreature bc, Type[] typelist)
        {
            int random = Utility.Random(typelist.Length);
            Item item = Loot.Construct(typelist[random]);
            DistributeArtifact(bc.RandomPlayerWithLootingRights(), item);
        }

        public static void DistributeArtifact(Mobile to, Item artifact)
        {
            if (to != null)
            {
                Container pack = to.Backpack;

                if (pack == null || !pack.TryDropItem(to, artifact, false))
                    to.BankBox.DropItem(artifact);

                to.SendLocalizedMessage(502088); // A special gift has been placed in your backpack.
            }
            else if (artifact != null)
            {
                artifact.Delete();
            }
        }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomDouble() < 0.2)
                DistributeRandomArtifact(this, m_Artifact);

            Map map = Map;

            if (map != null)
            {
                for (int x = -8; x <= 8; ++x)
                {
                    for (int y = -8; y <= 8; ++y)
                    {
                        double dist = Math.Sqrt(x * x + y * y);

                        if (dist <= 8)
                            new GoldTimer(map, X + x, Y + y).Start();
                    }
                }
            }

            if (Instances != null && Instances.Contains(this))
                Instances.Remove(this);

            return base.OnBeforeDeath();
        }

        public override bool CanBeParagon => false;
        public override bool Unprovokable => true;
        public virtual double ChangeCombatant => 0.3;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Greater;
        public override int TreasureMapLevel => 5;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
        }

        public void SpawnVortices(Mobile target)
        {
            Map map = Map;

            if (map == null)
                return;

            MovingParticles(target, 0x1AF6, 5, 0, false, false, 0x816, 0, 3006, 0, 0, 0);

            DeathVortexTrap dv;

            for (int i = 0; i < 3; ++i)
            {
                dv = new DeathVortexTrap();
                dv.MoveToWorld(GetSpawnPosition(target.Location, map, 3), map);
            }

            target.SendLocalizedMessage(1152693); // The power of the Void surges around you! 

            m_LastTarget = target.Location;
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target != null)
            {
                if (m_LastTarget != target.Location)
                {
                    target.SendLocalizedMessage(1152692, Name); // ~1_CREATURE~ casts a deadly vortex at you!                    
                    SpawnVortices(target);
                }
            }
        }

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            DoSpecialAbility(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);  //if it hits you it spawns vortices

            DoSpecialAbility(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker); //if you hit creature it spawns vortices

            DoSpecialAbility(attacker);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (Hits < m_MinHits && Hits < HitsMax * 0.60)
                m_MinHits = Hits;

            if (Hits >= HitsMax * 0.75)
                m_MinHits = HitsMax;
        }

        public ClockworkExodus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(m_MinHits);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_MinHits = reader.ReadInt();

            if (Instances == null)
                Instances = new List<ClockworkExodus>();

            Instances.Add(this);
        }

        private class GoldTimer : Timer
        {
            private readonly Map m_Map;
            private readonly int m_X, m_Y;

            public GoldTimer(Map map, int x, int y) : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                m_Map = map;
                m_X = x;
                m_Y = y;
            }

            protected override void OnTick()
            {
                int z = m_Map.GetAverageZ(m_X, m_Y);
                bool canFit = m_Map.CanFit(m_X, m_Y, z, 6, false, false);

                for (int i = -3; !canFit && i <= 3; ++i)
                {
                    canFit = m_Map.CanFit(m_X, m_Y, z + i, 6, false, false);

                    if (canFit)
                        z += i;
                }

                if (!canFit)
                    return;

                Gold g = new Gold(500, 1000);
                g.MoveToWorld(new Point3D(m_X, m_Y, z), m_Map);

                if (0.3 >= Utility.RandomDouble())
                {
                    Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                    Effects.PlaySound(g, g.Map, 0x208);
                }
            }
        }
    }
}
