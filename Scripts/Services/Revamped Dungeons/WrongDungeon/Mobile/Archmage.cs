using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("an archmage corpse")] 
    public class Archmage : BaseCreature 
    { 
        [Constructable] 
        public Archmage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("evil mage");
            this.Title = "The Insane The Archmage";
            this.Body = Utility.RandomList(125, 126);

            PackItem(new Robe(Utility.RandomMetalHue()));

            SetStr(85, 90);
            SetDex(194, 203);
            SetInt(237, 241);

            SetHits(3106, 3122);
            SetMana(578, 616);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 68);
            SetResistance(ResistanceType.Fire, 65, 66);
            SetResistance(ResistanceType.Cold, 62, 69);
            SetResistance(ResistanceType.Poison, 62, 67);
            SetResistance(ResistanceType.Energy, 64, 68);

            SetSkill(SkillName.EvalInt, 88.9, 94.1);
            SetSkill(SkillName.Magery, 96.9, 98.4);
            SetSkill(SkillName.Meditation, 89.9, 90.7);
            SetSkill(SkillName.MagicResist, 87.2, 88.7);
            SetSkill(SkillName.Tactics, 78.2, 79.9);
            SetSkill(SkillName.Wrestling, 84.8, 92.6);

            Fame = 14500;
            Karma = -14500;

            VirtualArmor = 16;
			switch (Utility.Random(16))
            {
                case 0: PackItem(new BloodOathScroll()); break;
                case 1: PackItem(new CurseWeaponScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new LichFormScroll()); break;
			}
            PackReg(23);
            PackItem(new Sandals());

            if (Utility.RandomDouble() < 0.75)
            {
                PackItem(new SeveredHumanEars());
            }
        }

        public Archmage(Serial serial)
            : base(serial)
        { 
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override int Meat { get { return 1; } }
        public override int TreasureMapLevel { get { return Core.AOS ? 2 : 0; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
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