using System;

namespace Server.Mobiles
{
    [CorpseName("a dream wraith corpse")]
    public class DreamWraith : BaseCreature
    {
        [Constructable]
        public DreamWraith()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Dream Wraith";
            this.Body = 740;
            //Hue = 0;
            this.BaseSoundID = 0x482;

            this.SetStr(200, 300);
            this.SetDex(100, 200);
            this.SetInt(600, 700);

            this.SetHits(550, 650);

            this.SetDamage(18, 25);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Cold, 45);
            this.SetDamageType(ResistanceType.Energy, 45);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.Anatomy, 0.0, 10.0);
            this.SetSkill(SkillName.EvalInt, 100.0, 120.0);
            this.SetSkill(SkillName.Magery, 100.0, 120.0);
            this.SetSkill(SkillName.Meditation, 100.0, 110.0);
            this.SetSkill(SkillName.MagicResist, 120.0, 150.0);
            this.SetSkill(SkillName.Tactics, 70.0, 80.0);
            this.SetSkill(SkillName.Wrestling, 90.0, 100.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.VirtualArmor = 28;

            this.PackReg(10);
        }

        public DreamWraith(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
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
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
        }

        public override int GetIdleSound()
        {
            return 0x5F4;
        }

        public override int GetAngerSound()
        {
            return 0x5F1;
        }

        public override int GetDeathSound()
        {
            return 0x5F2;
        }

        public override int GetHurtSound()
        {
            return 0x5F3;
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