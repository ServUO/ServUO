using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class Executioner : BaseCreature 
    { 
        [Constructable] 
        public Executioner()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        { 
            this.SpeechHue = Utility.RandomDyedHue(); 
            this.Title = "the executioner"; 
            this.Hue = Utility.RandomSkinHue(); 

            if (this.Female = Utility.RandomBool()) 
            { 
                this.Body = 0x191; 
                this.Name = NameList.RandomName("female"); 
                this.AddItem(new Skirt(Utility.RandomRedHue())); 
            }
            else 
            { 
                this.Body = 0x190; 
                this.Name = NameList.RandomName("male"); 
                this.AddItem(new ShortPants(Utility.RandomRedHue())); 
            }

            this.SetStr(386, 400);
            this.SetDex(151, 165);
            this.SetInt(161, 175);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 25, 30);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 125.0);
            this.SetSkill(SkillName.Fencing, 46.0, 77.5);
            this.SetSkill(SkillName.Macing, 35.0, 57.5);
            this.SetSkill(SkillName.Poisoning, 60.0, 82.5);
            this.SetSkill(SkillName.MagicResist, 83.5, 92.5);
            this.SetSkill(SkillName.Swords, 125.0);
            this.SetSkill(SkillName.Tactics, 125.0);
            this.SetSkill(SkillName.Lumberjacking, 125.0);

            this.Fame = 5000;
            this.Karma = -5000;

            this.VirtualArmor = 40;

            this.AddItem(new ThighBoots(Utility.RandomRedHue())); 
            this.AddItem(new Surcoat(Utility.RandomRedHue()));    
            this.AddItem(new ExecutionersAxe());

            Utility.AssignRandomHair(this);
        }

        public Executioner(Serial serial)
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Meager);
        }

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }
}