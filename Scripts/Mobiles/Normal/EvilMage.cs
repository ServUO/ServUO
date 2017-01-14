using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("an evil mage corpse")] 
    public class EvilMage : BaseCreature 
    { 
        [Constructable] 
        public EvilMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        { 
            this.Name = NameList.RandomName("evil mage");
            this.Title = "the evil mage";
            this.Body = 124;

            this.SetStr(81, 105);
            this.SetDex(91, 115);
            this.SetInt(96, 120);

            this.SetHits(49, 63);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.EvalInt, 75.1, 100.0);
            this.SetSkill(SkillName.Magery, 75.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 75.0, 97.5);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 20.2, 60.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 16;
            this.PackReg(6);
            this.PackItem(new Robe(Utility.RandomNeutralHue())); // TODO: Proper hue
            this.PackItem(new Sandals());
			switch (Utility.Random(18))
            {
                case 0: PackItem(new BloodOathScroll()); break;
                case 1: PackItem(new CurseWeaponScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
			}
        }

        public EvilMage(Serial serial)
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
                return Core.AOS ? 1 : 0;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.MedScrolls);
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