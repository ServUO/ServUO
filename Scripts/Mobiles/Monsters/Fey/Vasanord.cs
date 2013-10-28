using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a plant corpse")]
    public class Vasanord : BaseCreature
    {
        [Constructable]
        public Vasanord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            this.Name = "Vasanord";
            this.Body = 780;
            this.Hue = 2071;

            this.SetStr(805, 869);
            this.SetDex(51, 64);
            this.SetInt(38, 48);

            this.SetHits(2553, 2626);
            this.SetMana(110);
            this.SetStam(51, 64);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 30, 50);
            this.SetResistance(ResistanceType.Fire, 20, 40);
            this.SetResistance(ResistanceType.Cold, 20, 50);
            this.SetResistance(ResistanceType.Poison, 100, 120);
            this.SetResistance(ResistanceType.Energy, 20, 50);

            this.SetSkill(SkillName.MagicResist, 72.8, 77.7);
            this.SetSkill(SkillName.Tactics, 50.7, 99.6);
            this.SetSkill(SkillName.Anatomy, 6.5, 17.1);
            this.SetSkill(SkillName.EvalInt, 92.5, 106.2);
            this.SetSkill(SkillName.Magery, 95.5, 106.9);
            this.SetSkill(SkillName.Wrestling, 93.6, 98.6);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 88;

            this.PackItem(new DaemonBone(30));
            this.PackItem(new Board(10));
        }

        public Vasanord(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
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
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new TaintedSeeds(2));

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidEssence(2));

            if (Utility.RandomDouble() < 0.10)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        this.AddToBackpack(new VoidOrb());
                        break;
                    case 1:
                        this.AddToBackpack(new VoidCore());
                        break;
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
        /*public override void OnGotMeleeAttack( Mobile attacker )
        {
        base.OnGotMeleeAttack( attacker );
        if ( this.Hits > (this.HitsMax / 4) )
        {
        if ( 0.25 >= Utility.RandomDouble() )
        //SpawnBogling( attacker );
        }
        else if ( 0.25 >= Utility.RandomDouble() )
        {
        //EatBoglings();
        }
        }*/
    }
}