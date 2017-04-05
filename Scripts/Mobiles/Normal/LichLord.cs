using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a liche's corpse")]
    public class LichLord : BaseCreature
    {
        [Constructable]
        public LichLord()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a lich lord";
            this.Body = 79;
            this.BaseSoundID = 412;

            this.SetStr(416, 505);
            this.SetDex(146, 165);
            this.SetInt(566, 655);

            this.SetHits(250, 303);

            this.SetDamage(11, 13);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Cold, 60);
            this.SetDamageType(ResistanceType.Energy, 40);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Necromancy, 90, 110.0);
            this.SetSkill(SkillName.SpiritSpeak, 90.0, 110.0);

            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 150.5, 200.0);
            this.SetSkill(SkillName.Tactics, 50.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 18000;
            this.Karma = -18000;

            this.VirtualArmor = 50;
            this.PackItem(new GnarledStaff());
            this.PackNecroReg(12, 40);

			switch (Utility.Random(15))
            {
                case 0: PackItem(new LichFormScroll()); break;
                case 1: PackItem(new PoisonStrikeScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new VengefulSpiritScroll()); break;
				case 4: PackItem(new WitherScroll()); break;
			}
        }

        public LichLord(Serial serial)
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
                return 4;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
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