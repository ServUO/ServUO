using System;
using System.Collections;
using Server.Items;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;
using Server;

namespace Server.Mobiles
{
    [CorpseName("a Vile corpse")]
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
            this.Name = "Clockwork Exodus";
            this.Body = 1248;
            this.BaseSoundID = 639;

            this.SetStr(851, 950);
            this.SetDex(581, 683);
            this.SetInt(601, 750);

            this.SetHits(50000);
            this.SetStam(507, 669);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 75, 90);
            this.SetResistance(ResistanceType.Fire, 50, 65);
            this.SetResistance(ResistanceType.Cold, 45, 60);
            this.SetResistance(ResistanceType.Poison, 45, 60);
            this.SetResistance(ResistanceType.Energy, 45, 60);

            this.SetSkill(SkillName.EvalInt, 130.0);
            this.SetSkill(SkillName.Mysticism, 120);
            this.SetSkill(SkillName.MagicResist, 140.0);
            this.SetSkill(SkillName.Tactics, 90.1, 105.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 105.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);
            this.SetSkill(SkillName.Necromancy, 120.0);
            this.SetSkill(SkillName.Poisoning, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.Anatomy, 120.0);
            this.SetSkill(SkillName.Healing, 120.0);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 20;

            m_MinHits = this.Hits;

            if (Instances == null)
                Instances = new List<ClockworkExodus>();

            Instances.Add(this);
        }        

        public static void DistributeRandomArtifact(BaseCreature bc, Type[] typelist)
        {
            int random = Utility.Random(typelist.Length);
            Item item = Loot.Construct(typelist[random]);
            DistributeArtifact(DemonKnight.FindRandomPlayer(bc), item);
        }

        public static void DistributeArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                to.BankBox.DropItem(artifact);

            to.SendLocalizedMessage(502088); // A special gift has been placed in your backpack.
		}

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomDouble() < 0.2)
                DistributeRandomArtifact(this, m_Artifact);

            Map map = this.Map;

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

        public override bool CanBeParagon { get { return false; } }
        public override bool Unprovokable { get { return true; } }
        public virtual double ChangeCombatant { get { return 0.3; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 5; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 2);
        }

        public void SpawnVortices(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            DeathVortexTrap dv = new DeathVortexTrap();

            this.MovingParticles(target, 0x1AF6, 5, 0, false, false, 0x816, 0, 3006, 0, 0, 0);

            dv.MoveToWorld(new Point3D(target.X + 1, target.Y + 1, this.Z), map);

            m_LastTarget = target.Location;
        }

        public override bool HasAura { get { return true; } }
        public override TimeSpan AuraInterval { get { return TimeSpan.FromSeconds(3); } }
        public override int AuraRange { get { return 3; } }
        public override int AuraBaseDamage { get { return 25; } }
        public override int AuraEnergyDamage { get { return 100; } }

        public override void AuraEffect(Mobile m)
        {
            if (m.NetState != null)
                m.SendLocalizedMessage(1151112, String.Format("{0}\t#1072073", this.Name)); // : The creature's aura of energy is damaging you!
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target != null)
            {
                if (m_LastTarget != target.Location)
                {
                    target.SendLocalizedMessage(1152692, this.Name); // ~1_CREATURE~ casts a deadly vortex at you! 
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

            if (this.Hits < m_MinHits && this.Hits < this.HitsMax * 0.60)
                m_MinHits = this.Hits;

            if (this.Hits >= this.HitsMax * 0.75)
                m_MinHits = this.HitsMax;            
        }

        public ClockworkExodus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_MinHits);
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
            private Map m_Map;
            private int m_X, m_Y;

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

                if (0.5 >= Utility.RandomDouble())
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