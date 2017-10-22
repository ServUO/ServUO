using System;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal infernal corpse")]
    public class AbyssalInfernal : BaseChampion
    {
        private DateTime m_Delay;

        [Constructable]
        public AbyssalInfernal()
            : base(AIType.AI_Mage)
        {
            Body = 713;
            Name = "Abyssal Infernal";

            SetStr(1165, 1262);
            SetDex(104, 143);
            SetInt(572, 675);

            SetHits(30000);
            SetDamage(11, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 70, 90);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 65, 76);

            SetSkill(SkillName.Anatomy, 110.1, 119.3);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 111.0, 117.6);
            SetSkill(SkillName.Wrestling, 111.0, 120.0);
            SetSkill(SkillName.Magery, 109.0, 129.6);
            SetSkill(SkillName.EvalInt, 113.6, 135.2);
            SetSkill(SkillName.Meditation, 100.0);

            Fame = 28000;
            Karma = -28000;

            VirtualArmor = 40;
        }

        public AbyssalInfernal(Serial serial)
            : base(serial)
        {
        }

        public override int GetAttackSound() { return 0x5D4; }
        public override int GetDeathSound() { return 0x5D5; }
        public override int GetHurtSound() { return 0x5D6; }
        public override int GetIdleSound() { return 0x5D7; }

        public override ChampionSkullType SkullType { get { return ChampionSkullType.None; } }

        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(DeathsHead), typeof(WallofHungryMouths), typeof(AbyssalBlade), typeof(TongueoftheBeast) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { typeof(RoyalGuardInvestigatorsCloak), typeof(JadeArmband), typeof(DetectiveBoots) };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(MagicalDoor), typeof(MonsterStatuette) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { MonsterStatuetteType.ArchDemon };
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.50)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        AddToBackpack(new HornAbyssalInferno());
                        break;
                    case 1:
                        AddToBackpack(new NetherCycloneScroll());
                        break;
                }
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.25 >= Utility.RandomDouble())
                DrainLife();

            DoSpecialAbility(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.25 >= Utility.RandomDouble())
                DrainLife();

            DoSpecialAbility(attacker);
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target == null || target.Deleted) //sanity
                return;

            if (0.05 >= Utility.RandomDouble()) // 20% chance to more ratmen
                SpawnFollowers(target);
        }

        public override void OnActionCombat()
        {
            if (DateTime.UtcNow > m_Delay)
            {
                Ability.CrimsonMeteor(this, 100);
                m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 35));
            }

            base.OnActionCombat();
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            if (Map != null && caster != this && caster is PlayerMobile && 0.20 > Utility.RandomDouble())
            {
                Combatant = caster;
                Map = caster.Map;
                Location = caster.Location;

                switch (Utility.Random(5))
                {
                    case 0:
                        caster.Location = new Point3D(6949, 701, 32);
                        break;
                    case 1:
                        caster.Location = new Point3D(6941, 761, 32);
                        break;
                    case 2:
                        caster.Location = new Point3D(7015, 688, 32);
                        break;
                    case 3:
                        caster.Location = new Point3D(7043, 751, 32);
                        break;
                    case 4:
                        caster.Location = new Point3D(6999, 798, 32);
                        break;
                }

                AOS.Damage(caster, Utility.RandomMinMax(75, 85), 0, 100, 0, 0, 0);
                caster.MoveToWorld(caster.Location, caster.Map);
                caster.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                caster.PlaySound(0x1FE);
            }

            base.OnDamagedBySpell(caster);
        }

        public override bool DrainsLife { get { return true; } }
        public override double DrainsLifeChance { get { return .25; } }

        private static Type[] m_SummonTypes = new Type[]
            {
                typeof(Efreet),         typeof(FireGargoyle),
                typeof(FireSteed),     typeof(Gargoyle),
                typeof(HellHound),      typeof(HellCat),
                typeof(Imp),            typeof(LavaElemental),
                typeof(Nightmare),      typeof(Phoenix)
            };

        private static Point2D[] m_ColumnOffset = new Point2D[]
            {
                new Point2D(0, -1),
                new Point2D(-1,  0),
                new Point2D(1,  0),
                new Point2D(0,  1)
            };

        public void SpawnFollowers(Mobile from)
        {
            if (Map == null)
                return;

            Point3D loc = Map.GetSpawnPosition(Location, 8);
            Type type = m_SummonTypes[Utility.Random(m_SummonTypes.Length)];

            PlaySound(0x218);

            for (int i = 0; i < 4; i++)
            {
                BaseCreature summon = (BaseCreature)Activator.CreateInstance(type);

                if (summon != null)
                {
                    summon.SetHits(summon.HitsMax / 2);
                    summon.Team = Team;
                    summon.OnBeforeSpawn(loc, Map);
                    summon.MoveToWorld(loc, Map);
                    summon.Combatant = from;
                }
            }
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
}