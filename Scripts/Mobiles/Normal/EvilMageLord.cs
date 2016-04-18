using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("an evil mage lord corpse")] 
    public class EvilMageLord : BaseCreature 
    { 
        [Constructable] 
        public EvilMageLord()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        { 
            this.Name = NameList.RandomName("evil mage lord");
            this.Body = Utility.RandomList(125, 126);

            this.PackItem(new Robe(Utility.RandomMetalHue())); 
            this.PackItem(new WizardsHat(Utility.RandomMetalHue())); 

            this.SetStr(81, 105);
            this.SetDex(191, 215);
            this.SetInt(126, 150);

            this.SetHits(49, 63);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.EvalInt, 80.2, 100.0);
            this.SetSkill(SkillName.Magery, 95.1, 100.0);
            this.SetSkill(SkillName.Meditation, 27.5, 50.0);
            this.SetSkill(SkillName.MagicResist, 77.5, 100.0);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 20.3, 80.0);

            this.Fame = 10500;
            this.Karma = -10500;

            this.VirtualArmor = 16;
			switch (Utility.Random(16))
            {
                case 0: PackItem(new BloodOathScroll()); break;
                case 1: PackItem(new CurseWeaponScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new LichFormScroll()); break;
			}
            this.PackReg(23);
            if (Utility.RandomBool())
                this.PackItem(new Shoes());
            else
                this.PackItem(new Sandals());
        }

        public EvilMageLord(Serial serial)
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
        public override bool AlwaysMurderer
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
                return 1;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return Core.AOS ? 2 : 0;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.MedScrolls, 2);
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