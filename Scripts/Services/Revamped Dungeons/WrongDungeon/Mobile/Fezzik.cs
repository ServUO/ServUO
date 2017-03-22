using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ogre corpse")]
    public class Fezzik : BaseCreature
    {
        [Constructable]
        public Fezzik()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Fezzik";
            this.Title = "The Ogre Cook";
            this.Body = 1;
            this.BaseSoundID = 427;

            this.SetStr(1142, 1381);
            this.SetDex(73, 90);
            this.SetInt(52, 84);
            
            this.SetMana(0);

            this.SetDamage(25, 30);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 75, 80);
            this.SetResistance(ResistanceType.Fire, 70, 75);
            this.SetResistance(ResistanceType.Cold, 65, 75);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.MagicResist, 133.3, 151.9);
            this.SetSkill(SkillName.Tactics, 120.3, 128.9);
            this.SetSkill(SkillName.Wrestling, 122.2, 128.9);

            this.Fame = 3000;
            this.Karma = -3000;

            this.VirtualArmor = 52;

            this.PackItem(new Club());
        }

        public Fezzik(Serial serial)
            : base(serial)
        {
        }

        public override int Meat { get { return 2; } }

        public void SpawnGreenGoo(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            int newGreenGoo = Utility.RandomMinMax(3, 6);

            for (int i = 0; i < newGreenGoo; ++i)
            {
                GooeyMaggots goo = new GooeyMaggots();

                goo.Team = this.Team;
                goo.FightMode = FightMode.Closest;

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

                goo.MoveToWorld(loc, map);
                goo.Combatant = target;
            }
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            if (0.5 >= Utility.RandomDouble())
                this.SpawnGreenGoo(caster);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.5 >= Utility.RandomDouble())
                this.SpawnGreenGoo(attacker);
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