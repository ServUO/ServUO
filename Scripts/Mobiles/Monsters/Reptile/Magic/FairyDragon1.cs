using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fairy dragon corpse")]
    public class FairyDragon1 : BaseCreature
    {
        [Constructable]
        public FairyDragon1()
            : base(AIType.AI_OmniAI, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fairy dragon";
            this.Body = 718;

            this.SetStr(506, 561);
            this.SetDex(97, 103);
            this.SetInt(401, 580);

            this.SetHits(393, 409);

            this.SetDamage(15, 20);

            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Cold, 25);
            this.SetDamageType(ResistanceType.Poison, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 15, 30);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 45);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 10.4, 50.0);
            this.SetSkill(SkillName.Magery, 20.0, 30.0);
            this.SetSkill(SkillName.Anatomy, 65.0, 73.6);
            this.SetSkill(SkillName.Mysticism, 35.0, 55.0);
            this.SetSkill(SkillName.Meditation, 1.5, 3.5);
            this.SetSkill(SkillName.MagicResist, 120.2, 125.1);
            this.SetSkill(SkillName.Tactics, 93.8, 98.5);
            this.SetSkill(SkillName.Wrestling, 83.1, 89.9);

            this.PackReg(20);
            this.PackItem(new Bandage(10));
            this.PackItem(new DragonBlood(4));

            this.Tamable = false;
            //ControlSlots = 2;
            //MinTameSkill = 106.0;
        }

        public FairyDragon1(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 4);
            this.AddLoot(LootPack.MedScrolls);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.20)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        c.DropItem(new EssenceDiligence());
                        break;
                    case 1:
                        c.DropItem(new FairyDragonWing());
                        break;
                    case 2:
                        c.DropItem(new FaeryDust());
                        break;
                    case 3:
                        c.DropItem(new FeyWings());
                        break;
                }

                if (Utility.RandomDouble() < 0.30)
                {
                    c.DropItem(new DraconicOrbKeyBlue());
                }
            }
        }

        public override int GetIdleSound()
        {
            return 1561;
        }

        public override int GetAngerSound()
        {
            return 1558;
        }

        public override int GetHurtSound()
        {
            return 1560;
        }

        public override int GetDeathSound()
        {
            return 1559;
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