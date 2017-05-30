using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGreenGoblinAlchemist : BaseCreature
    {
        [Constructable]
        public EnslavedGreenGoblinAlchemist()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Green Goblin Alchemist";
            this.Body = 723;
            this.BaseSoundID = 0x45A;

            this.SetStr(289, 289);
            this.SetDex(72, 72);
            this.SetInt(113, 113);

            this.SetHits(196, 196);
            this.SetStam(72, 72);
            this.SetMana(113, 113);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 49);
            this.SetResistance(ResistanceType.Fire, 50, 53);
            this.SetResistance(ResistanceType.Cold, 25, 30);
            this.SetResistance(ResistanceType.Poison, 40, 42);
            this.SetResistance(ResistanceType.Energy, 15, 18);

            this.SetSkill(SkillName.MagicResist, 124.1, 126.2);
            this.SetSkill(SkillName.Tactics, 75.3, 83.6);
            this.SetSkill(SkillName.Anatomy, 0.0, 0.0);
            this.SetSkill(SkillName.Wrestling, 90.4, 94.7);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;

            // loot 30-40 gold, magic item, gem, essence control,gob blood
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

        public EnslavedGreenGoblinAlchemist(Serial serial)
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