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
            Name = "a gray goblin mage";

            Body = 723;
            Hue = 1900;
            BaseSoundID = 0x45A;

            SetStr(227, 285);
            SetDex(70, 88);
            SetInt(451, 499);

            SetHits(129, 151);
            SetStam(70, 88);
            SetMana(451, 499);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 25, 32);
            SetResistance(ResistanceType.Poison, 10, 19);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 141.9, 147.1);
            SetSkill(SkillName.Tactics, 80.7, 86.9);
            SetSkill(SkillName.Anatomy, 81.9, 89.4);
            SetSkill(SkillName.Wrestling, 90.5, 104.2);
            SetSkill(SkillName.Magery, 105.5, 119.1);
            SetSkill(SkillName.EvalInt, 94.9, 107.7);
			
            Fame = 1500;
            Karma = -1500;

            VirtualArmor = 28;

            switch ( Utility.Random(20) )
            {
                case 0:
                    PackItem(new Scimitar());
                    break;
                case 1:
                    PackItem(new Katana());
                    break;
                case 2:
                    PackItem(new WarMace());
                    break;
                case 3:
                    PackItem(new WarHammer());
                    break;
                case 4:
                    PackItem(new Kryss());
                    break;
                case 5:
                    PackItem(new Pitchfork());
                    break;
            }

            PackItem(new ThighBoots());

            switch ( Utility.Random(3) )
            {
                case 0:
                    PackItem(new Ribs());
                    break;
                case 1:
                    PackItem(new Shaft());
                    break;
                case 2:
                    PackItem(new Candle());
                    break;
            }

            if (0.2 > Utility.RandomDouble())
                PackItem(new BolaBall());
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

        public override TribeType Tribe { get { return TribeType.GrayGoblin; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Body = 723;
                Hue = 1900;
            }
        }
    }
}
