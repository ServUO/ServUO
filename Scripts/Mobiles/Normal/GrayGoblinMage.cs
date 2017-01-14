using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a goblin mage corpse")]
    public class GrayGoblinMage : BaseCreature
    {
        [Constructable]
        public GrayGoblinMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Gray Goblin Mage";
            this.Body = 334;
            this.BaseSoundID = 0x45A;

            this.SetStr(227, 285);
            this.SetDex(70, 88);
            this.SetInt(451, 499);

            this.SetHits(129, 151);
            this.SetStam(70, 88);
            this.SetMana(451, 499);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 25, 32);
            this.SetResistance(ResistanceType.Poison, 10, 19);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 141.9, 147.1);
            this.SetSkill(SkillName.Tactics, 80.7, 86.9);
            this.SetSkill(SkillName.Anatomy, 81.9, 89.4);
            this.SetSkill(SkillName.Wrestling, 90.5, 104.2);
            this.SetSkill(SkillName.Magery, 105.5, 119.1);
            this.SetSkill(SkillName.EvalInt, 94.9, 107.7);
			
            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;
            this.QLPoints = 10;

            switch ( Utility.Random(20) )
            {
                case 0:
                    this.PackItem(new Scimitar());
                    break;
                case 1:
                    this.PackItem(new Katana());
                    break;
                case 2:
                    this.PackItem(new WarMace());
                    break;
                case 3:
                    this.PackItem(new WarHammer());
                    break;
                case 4:
                    this.PackItem(new Kryss());
                    break;
                case 5:
                    this.PackItem(new Pitchfork());
                    break;
            }

            this.PackItem(new ThighBoots());

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.PackItem(new Ribs());
                    break;
                case 1:
                    this.PackItem(new Shaft());
                    break;
                case 2:
                    this.PackItem(new Candle());
                    break;
            }

            if (0.2 > Utility.RandomDouble())
                this.PackItem(new BolaBall());
        }

        public GrayGoblinMage(Serial serial)
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
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.SavagesAndOrcs;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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