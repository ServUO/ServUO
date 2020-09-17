using Server.Engines.CannedEvil;
using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a corpse of Twaulo")]
    public class Twaulo : BaseChampion
    {
        [Constructable]
        public Twaulo()
            : base(AIType.AI_Melee)
        {
            Name = "Twaulo";
            Title = "of the Glade";
            Body = 101;
            BaseSoundID = 679;
            Hue = 0x455;

            SetStr(1751, 1950);
            SetDex(251, 450);
            SetInt(801, 1000);

            SetHits(7500);

            SetDamage(19, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 0); // Per Stratics?!?
            SetSkill(SkillName.Magery, 0); // Per Stratics?!?
            SetSkill(SkillName.Meditation, 0); // Per Stratics?!?
            SetSkill(SkillName.Anatomy, 95.1, 115.0);
            SetSkill(SkillName.Archery, 95.1, 100.0);
            SetSkill(SkillName.MagicResist, 50.3, 80.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 95.1, 100.0);

            Fame = 50000;
            Karma = 50000;

            AddItem(new Bow());
        }

        public Twaulo(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType => ChampionSkullType.Pain;

        public override Type[] UniqueList => new Type[] { typeof(Quell) };

        public override Type[] SharedList => new Type[] { typeof(TheMostKnowledgePerson), typeof(OblivionsNeedle), typeof(BraveKnightOfTheBritannia) };

        public override Type[] DecorativeList => new Type[] { typeof(Pier), typeof(MonsterStatuette) };

        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { MonsterStatuetteType.DreadHorn };

        public override bool Unprovokable => true;

        public override Poison PoisonImmune => Poison.Regular;

        public override int TreasureMapLevel => 5;

        public override int Meat => 1;

        public override int Hides => 8;

        public override HideType HideType => HideType.Spined;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItem<Arrow>(Utility.RandomMinMax(500, 700)));
        }

        public void SpawnPixies(Mobile target)
        {
            Map map = Map;

            if (map == null)
                return;

            int newPixies = Utility.RandomMinMax(3, 6);

            for (int i = 0; i < newPixies; ++i)
            {
                Pixie pixie = new Pixie
                {
                    Team = Team,
                    FightMode = FightMode.Closest
                };

                bool validLocation = false;
                Point3D loc = Location;

                for (int j = 0; !validLocation && j < 10; ++j)
                {
                    int x = X + Utility.Random(3) - 1;
                    int y = Y + Utility.Random(3) - 1;
                    int z = map.GetAverageZ(x, y);

                    if (validLocation = map.CanFit(x, y, Z, 16, false, false))
                        loc = new Point3D(x, y, Z);
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
                SpawnPixies(caster);
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
                SpawnPixies(attacker);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
