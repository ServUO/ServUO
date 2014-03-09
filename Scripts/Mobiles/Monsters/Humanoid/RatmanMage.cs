using System;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a glowing ratman corpse")]
    public class RatmanMage : BaseCreature
    {
        [Constructable]
        public RatmanMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("ratman");
            this.Body = 0x8F;
            this.BaseSoundID = 437;

            this.SetStr(146, 180);
            this.SetDex(101, 130);
            this.SetInt(186, 210);

            this.SetHits(88, 108);

            this.SetDamage(7, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 45);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.EvalInt, 70.1, 80.0);
            this.SetSkill(SkillName.Magery, 70.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 65.1, 90.0);
            this.SetSkill(SkillName.Tactics, 50.1, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 75.0);

            this.Fame = 7500;
            this.Karma = -7500;

            this.VirtualArmor = 44;

            this.PackReg(6);

            if (0.02 > Utility.RandomDouble())
                this.PackStatue();

			switch (Utility.Random(60))
            {
                case 0: PackItem(new AnimateDeadScroll()); break;
                case 1: PackItem(new BloodOathScroll()); break;
                case 2: PackItem(new CorpseSkinScroll()); break;
                case 3: PackItem(new CurseWeaponScroll()); break;
				case 4: PackItem(new EvilOmenScroll()); break;
				case 5: PackItem(new HorrificBeastScroll()); break;
				case 6: PackItem(new MindRotScroll()); break;
				case 7: PackItem(new PainSpikeScroll()); break;
				case 8: PackItem(new WraithFormScroll()); break;
				case 9: PackItem(new PoisonStrikeScroll()); break; 
			}
        }

        public RatmanMage(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Ratman;
            }
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
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.LowScrolls);
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

            if (this.Body == 42)
            {
                this.Body = 0x8F;
                this.Hue = 0;
            }
        }
    }
}