using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a plant corpse")]
    public class BogThing : BaseCreature
    {
        [Constructable]
        public BogThing()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            this.Name = "a bog thing";
            this.Body = 780;

            this.SetStr(801, 900);
            this.SetDex(46, 65);
            this.SetInt(36, 50);

            this.SetHits(481, 540);
            this.SetMana(0);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 20, 25);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.MagicResist, 90.1, 95.0);
            this.SetSkill(SkillName.Tactics, 70.1, 85.0);
            this.SetSkill(SkillName.Wrestling, 65.1, 80.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 28;

            if (0.25 > Utility.RandomDouble())
                this.PackItem(new Board(10));
            else
                this.PackItem(new Log(10));

            this.PackReg(3);
            this.PackItem(new Engines.Plants.Seed());
            this.PackItem(new Engines.Plants.Seed());
        }

        public BogThing(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
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

        public void SpawnBogling(Mobile m)
        {
            Map map = this.Map;

            if (map == null)
                return;

            Bogling spawned = new Bogling();

            spawned.Team = this.Team;

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
                this.PlaySound(Utility.Random(0x3B, 2)); // Eat sound

                foreach (Mobile m in toEat)
                {
                    this.Hits += (m.Hits / 2);
                    m.Delete();
                }
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (this.Hits > (this.HitsMax / 4))
            {
                if (0.25 >= Utility.RandomDouble())
                    this.SpawnBogling(attacker);
            }
            else if (0.25 >= Utility.RandomDouble())
            {
                this.EatBoglings();
            }
        }
    }
}