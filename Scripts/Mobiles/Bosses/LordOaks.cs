using System;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public class LordOaks : BaseChampion
    {
        private Mobile m_Queen;
        private bool m_SpawnedQueen;
        [Constructable]
        public LordOaks()
            : base(AIType.AI_Mage, FightMode.Evil)
        {
            this.Body = 175;
            this.Name = "Lord Oaks";

            this.SetStr(403, 850);
            this.SetDex(101, 150);
            this.SetInt(503, 800);

            this.SetHits(3000);
            this.SetStam(202, 400);

            this.SetDamage(21, 33);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 85, 90);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 80, 90);
            this.SetResistance(ResistanceType.Energy, 80, 90);

            this.SetSkill(SkillName.Anatomy, 75.1, 100.0);
            this.SetSkill(SkillName.EvalInt, 120.1, 130.0);
            this.SetSkill(SkillName.Magery, 120.0);
            this.SetSkill(SkillName.Meditation, 120.1, 130.0);
            this.SetSkill(SkillName.MagicResist, 100.5, 150.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 22500;
            this.Karma = 22500;

            this.VirtualArmor = 100;
        }

        public LordOaks(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Enlightenment;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(OrcChieftainHelm) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[]
                {
                    typeof(RoyalGuardSurvivalKnife),
                    typeof(DjinnisRing),
                    typeof(LieutenantOfTheBritannianRoyalGuard),
                    typeof(SamaritanRobe),
                    typeof(DetectiveBoots),
                    typeof(TheMostKnowledgePerson)
                };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[]
                {
                    typeof(WaterTile),
                    typeof(WindSpirit),
                    typeof(Pier),
                };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { };
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool Uncalmable
        {
            get
            {
                return Core.SE;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 5);
        }

        public void SpawnPixies(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            this.Say(1042154); // You shall never defeat me as long as I have my queen!

            int newPixies = Utility.RandomMinMax(3, 6);

            for (int i = 0; i < newPixies; ++i)
            {
                Pixie pixie = new Pixie();

                pixie.Team = this.Team;
                pixie.FightMode = FightMode.Closest;

                bool validLocation = false;
                Point3D loc = this.Location;

                for (int j = 0; !validLocation && j < 10; ++j)
                {
                    int x = this.X + Utility.Random(3) - 1;
                    int y = this.Y + Utility.Random(3) - 1;
                    int z = map.GetAverageZ(x, y);

                    if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                        loc = new Point3D(x, y, this.Z);
                    else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                        loc = new Point3D(x, y, z);
                }

                pixie.MoveToWorld(loc, map);
                pixie.Combatant = target;
            }
        }

        public override int GetAngerSound()
        {
            return 0x2F8;
        }

        public override int GetIdleSound()
        {
            return 0x2F8;
        }

        public override int GetAttackSound()
        {
            return Utility.Random(0x2F5, 2);
        }

        public override int GetHurtSound()
        {
            return 0x2F9;
        }

        public override int GetDeathSound()
        {
            return 0x2F7;
        }

        public void CheckQueen()
        {
            if (this.Map == null)
                return;

            if (!this.m_SpawnedQueen)
            {
                this.Say(1042153); // Come forth my queen!

                this.m_Queen = new Silvani();

                ((BaseCreature)this.m_Queen).Team = this.Team;

                this.m_Queen.MoveToWorld(this.Location, this.Map);

                this.m_SpawnedQueen = true;
            }
            else if (this.m_Queen != null && this.m_Queen.Deleted)
            {
                this.m_Queen = null;
            }
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            this.CheckQueen();

            if (this.m_Queen != null)
            {
                scalar *= 0.1;

                if (0.1 >= Utility.RandomDouble())
                    this.SpawnPixies(caster);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.Damage(Utility.Random(20, 10), this);
            defender.Stam -= Utility.Random(20, 10);
            defender.Mana -= Utility.Random(20, 10);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            this.CheckQueen();

            if (this.m_Queen != null && 0.1 >= Utility.RandomDouble())
                this.SpawnPixies(attacker);

            attacker.Damage(Utility.Random(20, 10), this);
            attacker.Stam -= Utility.Random(20, 10);
            attacker.Mana -= Utility.Random(20, 10);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Queen);
            writer.Write(this.m_SpawnedQueen);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Queen = reader.ReadMobile();
                        this.m_SpawnedQueen = reader.ReadBool();

                        break;
                    }
            }
        }
    }
}