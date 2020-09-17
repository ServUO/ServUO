using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a plant corpse")]
    public class BogThing : BaseCreature
    {
        [Constructable]
        public BogThing()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            Name = "a bog thing";
            Body = 780;

            SetStr(801, 900);
            SetDex(46, 65);
            SetInt(36, 50);

            SetHits(481, 540);
            SetMana(0);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 25);
            SetResistance(ResistanceType.Cold, 10, 15);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 20, 25);

            SetSkill(SkillName.MagicResist, 90.1, 95.0);
            SetSkill(SkillName.Tactics, 70.1, 85.0);
            SetSkill(SkillName.Wrestling, 65.1, 80.0);

            Fame = 8000;
            Karma = -8000;
        }

        public BogThing(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MageryRegs, 3);
            AddLoot(LootPack.LootItems(new[] { new LootPackItem(typeof(Board), 1), new LootPackItem(typeof(Log), 3) }, 10));
            AddLoot(LootPack.LootItem<Engines.Plants.Seed>(), 2);
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

        public void SpawnBogling(Mobile m)
        {
            Map map = Map;

            if (map == null)
                return;

            Bogling spawned = new Bogling
            {
                Team = Team
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

            spawned.MoveToWorld(loc, map);
            spawned.Combatant = m;
        }

        public void EatBoglings()
        {
            ArrayList toEat = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(2);

            foreach (Mobile m in eable)
            {
                if (m is Bogling)
                    toEat.Add(m);
            }
            eable.Free();

            if (toEat.Count > 0)
            {
                PlaySound(Utility.Random(0x3B, 2)); // Eat sound

                foreach (Mobile m in toEat)
                {
                    Hits += (m.Hits / 2);
                    m.Delete();
                }
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Hits > (HitsMax / 4))
            {
                if (0.25 >= Utility.RandomDouble())
                    SpawnBogling(attacker);
            }
            else if (0.25 >= Utility.RandomDouble())
            {
                EatBoglings();
            }
        }
    }
}
