using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Stone Monster corpse")]
    public class StoneMonster : BaseCreature
    {
        [Constructable]
        public StoneMonster()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            this.Name = "Stone Monster";

            switch ( Utility.Random(6) )
            {
                default:
                case 0:
                    this.Body = 86;
                    break;
                case 1:
                    this.Body = 722;
                    break;
                case 2:
                    this.Body = 59;
                    break;
                case 3:
                    this.Body = 85;
                    break; 
                case 4:
                    this.Body = 310;
                    break;
                case 5:
                    this.Body = 83;
                    break;						
            }

            this.Hue = 0;
                   
            if (this.Body == 86)
            {
                this.BaseSoundID = 634;  
                this.SetStr(150, 320);
                this.SetDex(94, 190);
                this.SetInt(64, 160);

                this.SetHits(128, 155);
                this.SetMana(0);

                this.SetDamage(5, 11);

                this.SetDamageType(ResistanceType.Physical, 100);

                this.SetResistance(ResistanceType.Physical, 35, 40);
                this.SetResistance(ResistanceType.Fire, 20, 30);
                this.SetResistance(ResistanceType.Cold, 25, 35);
                this.SetResistance(ResistanceType.Poison, 30, 40);
                this.SetResistance(ResistanceType.Energy, 25, 35);

                this.SetSkill(SkillName.MagicResist, 70.1, 85.0);
                this.SetSkill(SkillName.Swords, 60.1, 85.0);
                this.SetSkill(SkillName.Tactics, 75.1, 90.0); 
            }
            else if (this.Body == 722)
            {
                this.BaseSoundID = 372;

                this.SetStr(250, 350);
                this.SetDex(120, 140);
                this.SetInt(250, 350);

                this.SetHits(200, 300);

                this.SetDamage(15, 27);

                this.SetDamageType(ResistanceType.Physical, 10);
                this.SetDamageType(ResistanceType.Cold, 50);
                this.SetDamageType(ResistanceType.Energy, 40);
	 
                this.SetResistance(ResistanceType.Physical, 45, 55);
                this.SetResistance(ResistanceType.Fire, 30, 40);
                this.SetResistance(ResistanceType.Cold, 40, 55);
                this.SetResistance(ResistanceType.Poison, 55, 65);
                this.SetResistance(ResistanceType.Energy, 40, 50);

                this.SetSkill(SkillName.EvalInt, 90.1, 110.0);
                this.SetSkill(SkillName.Magery, 120);
                this.SetSkill(SkillName.MagicResist, 100.1, 120.0);
                this.SetSkill(SkillName.Tactics, 60.1, 70.0);
                this.SetSkill(SkillName.Wrestling, 60.1, 70.0);

                if (0.025 > Utility.RandomDouble())
                    this.PackItem(new GargoylesPickaxe());

                if (0.2 > Utility.RandomDouble())
                    this.PackItem(new UndeadGargHorn()); 
                //if ( 0.2 > Utility.RandomDouble() )
                //PackItem( new UndeadGargoyleMedallions() );
            }
            else if (this.Body == 59)
            {
                this.BaseSoundID = 362;
                this.SetStr(796, 825);
                this.SetDex(86, 105);
                this.SetInt(436, 475);

                this.SetHits(478, 495);

                this.SetDamage(16, 22);

                this.SetDamageType(ResistanceType.Physical, 100);

                this.SetResistance(ResistanceType.Physical, 55, 65);
                this.SetResistance(ResistanceType.Fire, 60, 70);
                this.SetResistance(ResistanceType.Cold, 30, 40);
                this.SetResistance(ResistanceType.Poison, 25, 35);
                this.SetResistance(ResistanceType.Energy, 35, 45);

                this.SetSkill(SkillName.EvalInt, 30.1, 40.0);
                this.SetSkill(SkillName.Magery, 30.1, 40.0);
                this.SetSkill(SkillName.MagicResist, 99.1, 100.0);
                this.SetSkill(SkillName.Tactics, 97.6, 100.0);
                this.SetSkill(SkillName.Wrestling, 90.1, 92.5);
            }
            else if (this.Body == 85)
            {
                this.BaseSoundID = 639;
                this.SetStr(281, 305);
                this.SetDex(191, 215);
                this.SetInt(226, 250);

                this.SetHits(169, 183);
                this.SetStam(36, 45);

                this.SetDamage(5, 10);

                this.SetDamageType(ResistanceType.Physical, 100);

                this.SetResistance(ResistanceType.Physical, 40, 45);
                this.SetResistance(ResistanceType.Fire, 20, 30);
                this.SetResistance(ResistanceType.Cold, 25, 35);
                this.SetResistance(ResistanceType.Poison, 35, 40);
                this.SetResistance(ResistanceType.Energy, 25, 35);

                this.SetSkill(SkillName.EvalInt, 95.1, 100.0);
                this.SetSkill(SkillName.Magery, 95.1, 100.0);
                this.SetSkill(SkillName.MagicResist, 75.0, 97.5);
                this.SetSkill(SkillName.Tactics, 65.0, 87.5);
                this.SetSkill(SkillName.Wrestling, 20.2, 60.0);
            }
            else if (this.Body == 310)
            {
                this.BaseSoundID = 0x482;
                this.SetStr(126, 150);
                this.SetDex(76, 100);
                this.SetInt(86, 110);

                this.SetHits(76, 90);

                this.SetDamage(10, 14);

                this.SetDamageType(ResistanceType.Physical, 20);
                this.SetDamageType(ResistanceType.Cold, 60);
                this.SetDamageType(ResistanceType.Poison, 20);

                this.SetResistance(ResistanceType.Physical, 50, 60);
                this.SetResistance(ResistanceType.Fire, 25, 30);
                this.SetResistance(ResistanceType.Cold, 70, 80);
                this.SetResistance(ResistanceType.Poison, 30, 40);
                this.SetResistance(ResistanceType.Energy, 40, 50);

                this.SetSkill(SkillName.MagicResist, 70.1, 95.0);
                this.SetSkill(SkillName.Tactics, 45.1, 70.0);
                this.SetSkill(SkillName.Wrestling, 50.1, 70.0);
            }
            else if (this.Body == 83)
            {
                this.BaseSoundID = 427; 
                this.SetStr(767, 945);
                this.SetDex(66, 75);
                this.SetInt(46, 70);

                this.SetHits(476, 552);

                this.SetDamage(20, 25);

                this.SetDamageType(ResistanceType.Physical, 100);

                this.SetResistance(ResistanceType.Physical, 45, 55);
                this.SetResistance(ResistanceType.Fire, 30, 40);
                this.SetResistance(ResistanceType.Cold, 30, 40);
                this.SetResistance(ResistanceType.Poison, 40, 50);
                this.SetResistance(ResistanceType.Energy, 40, 50);

                this.SetSkill(SkillName.MagicResist, 125.1, 140.0);
                this.SetSkill(SkillName.Tactics, 90.1, 100.0);
                this.SetSkill(SkillName.Wrestling, 90.1, 100.0);
            }
			
            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 40;
        }

        public StoneMonster(Serial serial)
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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
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