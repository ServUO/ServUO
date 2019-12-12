using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public class Rikktor : BaseChampion
    {
        [Constructable]
        public Rikktor()
            : base(AIType.AI_Melee)
        {
            Body = 172;
            Name = "Rikktor";

            SetStr(701, 900);
            SetDex(201, 350);
            SetInt(51, 100);

            SetHits(15000);
            SetStam(203, 650);

            SetDamage(28, 55);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.MagicResist, 140.2, 160.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 118.4, 123.9);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 130;

            ForceActiveSpeed = 0.35;
            ForcePassiveSpeed = 0.7;
        }

        public Rikktor(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Power;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(CrownOfTalKeesh) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[]
                {
                    typeof(TheMostKnowledgePerson),
                    typeof(BraveKnightOfTheBritannia),
                    typeof(LieutenantOfTheBritannianRoyalGuard)
                };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[]
                {
                    typeof(LavaTile),
                    typeof(MonsterStatuette),
                    typeof(MonsterStatuette)
                };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[]
                {
                    MonsterStatuetteType.OphidianArchMage,
                    MonsterStatuetteType.OphidianWarrior
                };
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.All;
            }
        }
        public override int Scales
        {
            get
            {
                return 20;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.2 >= Utility.RandomDouble())
                this.Earthquake();
        }

        public void Earthquake()
        {
            Map map = this.Map;

            if (map == null)
                return;

            ArrayList targets = new ArrayList();

            IPooledEnumerable eable = GetMobilesInRange(8);

            foreach (Mobile m in eable)
            {
                if (m == this || !this.CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    targets.Add(m);
                else if (m.Player)
                    targets.Add(m);
            }

            eable.Free();

            this.PlaySound(0x2F3);

            for (int i = 0; i < targets.Count; ++i)
            {
                Mobile m = (Mobile)targets[i];

                double damage = m.Hits * 0.6;

                if (damage < 10.0)
                    damage = 10.0;
                else if (damage > 75.0)
                    damage = 75.0;

                this.DoHarmful(m);

                AOS.Damage(m, this, (int)damage, 100, 0, 0, 0, 0);

                if (m.Alive && m.Body.IsHuman && !m.Mounted)
                    m.Animate(20, 7, 1, true, false, 0); // take hit
            }
        }

        public override int GetAngerSound()
        {
            return Utility.Random(0x2CE, 2);
        }

        public override int GetIdleSound()
        {
            return 0x2D2;
        }

        public override int GetAttackSound()
        {
            return Utility.Random(0x2C7, 5);
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override int GetDeathSound()
        {
            return 0x2CC;
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
        }
    }
}
