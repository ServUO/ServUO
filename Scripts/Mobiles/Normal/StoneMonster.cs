using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("stone monster corpse")]
    public class StoneMonster : BaseCreature
    {
        [Constructable]
        public StoneMonster()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            Name = "Stone Monster";

            switch (Utility.Random(6))
            {
                default:
                case 0:
                    Body = 86;
                    break;
                case 1:
                    Body = 722;
                    break;
                case 2:
                    Body = 59;
                    break;
                case 3:
                    Body = 85;
                    break;
                case 4:
                    Body = 310;
                    break;
                case 5:
                    Body = 83;
                    break;
            }

            Hue = 0;

            if (Body == 86)
            {
                BaseSoundID = 634;
                SetStr(150, 320);
                SetDex(94, 190);
                SetInt(64, 160);

                SetHits(128, 155);
                SetMana(0);

                SetDamage(5, 11);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 35, 40);
                SetResistance(ResistanceType.Fire, 20, 30);
                SetResistance(ResistanceType.Cold, 25, 35);
                SetResistance(ResistanceType.Poison, 30, 40);
                SetResistance(ResistanceType.Energy, 25, 35);

                SetSkill(SkillName.MagicResist, 70.1, 85.0);
                SetSkill(SkillName.Swords, 60.1, 85.0);
                SetSkill(SkillName.Tactics, 75.1, 90.0);
            }
            else if (Body == 722)
            {
                BaseSoundID = 372;

                SetStr(250, 350);
                SetDex(120, 140);
                SetInt(250, 350);

                SetHits(200, 300);

                SetDamage(15, 27);

                SetDamageType(ResistanceType.Physical, 10);
                SetDamageType(ResistanceType.Cold, 50);
                SetDamageType(ResistanceType.Energy, 40);

                SetResistance(ResistanceType.Physical, 45, 55);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 40, 55);
                SetResistance(ResistanceType.Poison, 55, 65);
                SetResistance(ResistanceType.Energy, 40, 50);

                SetSkill(SkillName.EvalInt, 90.1, 110.0);
                SetSkill(SkillName.Magery, 120);
                SetSkill(SkillName.MagicResist, 100.1, 120.0);
                SetSkill(SkillName.Tactics, 60.1, 70.0);
                SetSkill(SkillName.Wrestling, 60.1, 70.0);
            }
            else if (Body == 59)
            {
                BaseSoundID = 362;
                SetStr(796, 825);
                SetDex(86, 105);
                SetInt(436, 475);

                SetHits(478, 495);

                SetDamage(16, 22);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 55, 65);
                SetResistance(ResistanceType.Fire, 60, 70);
                SetResistance(ResistanceType.Cold, 30, 40);
                SetResistance(ResistanceType.Poison, 25, 35);
                SetResistance(ResistanceType.Energy, 35, 45);

                SetSkill(SkillName.EvalInt, 30.1, 40.0);
                SetSkill(SkillName.Magery, 30.1, 40.0);
                SetSkill(SkillName.MagicResist, 99.1, 100.0);
                SetSkill(SkillName.Tactics, 97.6, 100.0);
                SetSkill(SkillName.Wrestling, 90.1, 92.5);
            }
            else if (Body == 85)
            {
                BaseSoundID = 639;
                SetStr(281, 305);
                SetDex(191, 215);
                SetInt(226, 250);

                SetHits(169, 183);
                SetStam(36, 45);

                SetDamage(5, 10);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 40, 45);
                SetResistance(ResistanceType.Fire, 20, 30);
                SetResistance(ResistanceType.Cold, 25, 35);
                SetResistance(ResistanceType.Poison, 35, 40);
                SetResistance(ResistanceType.Energy, 25, 35);

                SetSkill(SkillName.EvalInt, 95.1, 100.0);
                SetSkill(SkillName.Magery, 95.1, 100.0);
                SetSkill(SkillName.MagicResist, 75.0, 97.5);
                SetSkill(SkillName.Tactics, 65.0, 87.5);
                SetSkill(SkillName.Wrestling, 20.2, 60.0);
            }
            else if (Body == 310)
            {
                BaseSoundID = 0x482;
                SetStr(126, 150);
                SetDex(76, 100);
                SetInt(86, 110);

                SetHits(76, 90);

                SetDamage(10, 14);

                SetDamageType(ResistanceType.Physical, 20);
                SetDamageType(ResistanceType.Cold, 60);
                SetDamageType(ResistanceType.Poison, 20);

                SetResistance(ResistanceType.Physical, 50, 60);
                SetResistance(ResistanceType.Fire, 25, 30);
                SetResistance(ResistanceType.Cold, 70, 80);
                SetResistance(ResistanceType.Poison, 30, 40);
                SetResistance(ResistanceType.Energy, 40, 50);

                SetSkill(SkillName.MagicResist, 70.1, 95.0);
                SetSkill(SkillName.Tactics, 45.1, 70.0);
                SetSkill(SkillName.Wrestling, 50.1, 70.0);
            }
            else if (Body == 83)
            {
                BaseSoundID = 427;
                SetStr(767, 945);
                SetDex(66, 75);
                SetInt(46, 70);

                SetHits(476, 552);

                SetDamage(20, 25);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 45, 55);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 30, 40);
                SetResistance(ResistanceType.Poison, 40, 50);
                SetResistance(ResistanceType.Energy, 40, 50);

                SetSkill(SkillName.MagicResist, 125.1, 140.0);
                SetSkill(SkillName.Tactics, 90.1, 100.0);
                SetSkill(SkillName.Wrestling, 90.1, 100.0);
            }

            Fame = 8000;
            Karma = -8000;
        }

        public StoneMonster(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);

            if (LootStage == LootStage.Death && Body == 772)
            {
                AddLoot(LootPack.LootItem<GargoylesPickaxe>(2.5));
                AddLoot(LootPack.LootItem<UndeadGargHorn>(20.0));
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
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
