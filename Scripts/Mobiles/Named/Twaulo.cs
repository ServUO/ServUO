using System;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a corpse of Twaulo")]
    public class Twaulo : BaseChampion
    {
        [Constructable]
        public Twaulo()
            : base(AIType.AI_Melee)
        {
            this.Name = "Twaulo";
            this.Title = "of the Glade";
            this.Body = 101;
            this.BaseSoundID = 679;
            this.Hue = 0x455;

            this.SetStr(1751, 1950);
            this.SetDex(251, 450);
            this.SetInt(801, 1000);

            this.SetHits(7500);

            this.SetDamage(19, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 45, 55);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 0); // Per Stratics?!?
            this.SetSkill(SkillName.Magery, 0); // Per Stratics?!?
            this.SetSkill(SkillName.Meditation, 0); // Per Stratics?!?
            this.SetSkill(SkillName.Anatomy, 95.1, 115.0);
            this.SetSkill(SkillName.Archery, 95.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 50.3, 80.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 95.1, 100.0);

            this.Fame = 50000;
            this.Karma = 50000;

            this.VirtualArmor = 50;

            this.AddItem(new Bow());
            this.PackItem(new Arrow(Utility.RandomMinMax(500, 700)));
        }

        public Twaulo(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Pain;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(Quell) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { typeof(TheMostKnowledgePerson), typeof(OblivionsNeedle), typeof(BraveKnightOfTheBritannia) };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(Pier), typeof(MonsterStatuette) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { MonsterStatuetteType.DreadHorn };
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems);
        }

        public void SpawnPixies(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

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

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            if (0.1 >= Utility.RandomDouble())
                this.SpawnPixies(caster);
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

            if (0.1 >= Utility.RandomDouble())
                this.SpawnPixies(attacker);
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