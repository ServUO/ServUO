using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a liche's corpse")]
    public class Lich : BaseCreature
    {
        [Constructable]
        public Lich()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a lich";
            this.Body = 24;
            this.BaseSoundID = 0x3E9;

            this.SetStr(171, 200);
            this.SetDex(126, 145);
            this.SetInt(276, 305);

            this.SetHits(103, 120);

            this.SetDamage(24, 26);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Necromancy, 89, 99.1);
            this.SetSkill(SkillName.SpiritSpeak, 90.0, 99.0);

            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Magery, 70.1, 80.0);
            this.SetSkill(SkillName.Meditation, 85.1, 95.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 100.0);
            this.SetSkill(SkillName.Tactics, 70.1, 90.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 50;

			switch (Utility.Random(25))
            {
                case 0: PackItem(new LichFormScroll()); break;
                case 1: PackItem(new PoisonStrikeScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new VengefulSpiritScroll()); break;
				case 4: PackItem(new WitherScroll()); break;
			}


            this.PackItem(new GnarledStaff());
            this.PackNecroReg(17, 24);
        }

        public Lich(Serial serial)
            : base(serial)
        {
        }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 3;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
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