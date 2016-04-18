using System;

namespace Server.Mobiles
{
    [CorpseName("a harpy corpse")]
    public class Harpy : BaseCreature
    {
        [Constructable]
        public Harpy()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a harpy";
            this.Body = 30;
            this.BaseSoundID = 402;

            this.SetStr(96, 120);
            this.SetDex(86, 110);
            this.SetInt(51, 75);

            this.SetHits(58, 72);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 30);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 50.1, 65.0);
            this.SetSkill(SkillName.Tactics, 70.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 90.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 28;
        }

        public Harpy(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Bird;
            }
        }
        public override int Feathers
        {
            get
            {
                return 50;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager, 2);
        }

        public override int GetAttackSound()
        {
            return 916;
        }

        public override int GetAngerSound()
        {
            return 916;
        }

        public override int GetDeathSound()
        {
            return 917;
        }

        public override int GetHurtSound()
        {
            return 919;
        }

        public override int GetIdleSound()
        {
            return 918;
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