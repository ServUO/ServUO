using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGrayGoblin : BaseCreature
    {
        [Constructable]
        public EnslavedGrayGoblin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Enslaved Gray Goblin";
            this.Body = 334;
            this.BaseSoundID = 0x45A;

            this.SetStr(321, 321);
            this.SetDex(64, 64);
            this.SetInt(147, 147);

            this.SetHits(179, 179);
            this.SetStam(64, 64);
            this.SetMana(147, 147);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 50);
            this.SetResistance(ResistanceType.Fire, 38, 38);
            this.SetResistance(ResistanceType.Cold, 32, 32);
            this.SetResistance(ResistanceType.Poison, 12, 12);
            this.SetResistance(ResistanceType.Energy, 11, 11);

            this.SetSkill(SkillName.MagicResist, 121.6, 121.6);
            this.SetSkill(SkillName.Tactics, 90.0, 90.0);
            this.SetSkill(SkillName.Anatomy, 82.0, 82.0);
            this.SetSkill(SkillName.Wrestling, 99.2, 99.2);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;

            // Loot - 30-40gold, magicitem,gem,goblin blood, essence control
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

        public EnslavedGrayGoblin(Serial serial)
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