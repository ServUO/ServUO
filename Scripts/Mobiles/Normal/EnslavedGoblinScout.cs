using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinScout : BaseCreature
    {
        [Constructable]
        public EnslavedGoblinScout()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Enslaved Goblin Scout";
            this.Body = 334;
            this.BaseSoundID = 0x45A;

            this.SetStr(320, 320);
            this.SetDex(74, 74);
            this.SetInt(112, 112);

            this.SetHits(182, 182);
            this.SetStam(74, 74);
            this.SetMana(112, 112);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 42, 42);
            this.SetResistance(ResistanceType.Fire, 33, 33);
            this.SetResistance(ResistanceType.Cold, 30, 30);
            this.SetResistance(ResistanceType.Poison, 14, 14);
            this.SetResistance(ResistanceType.Energy, 18, 18);

            this.SetSkill(SkillName.MagicResist, 95.0, 95.0);
            this.SetSkill(SkillName.Tactics, 80.0, 86.9);
            this.SetSkill(SkillName.Anatomy, 82.0, 89.3);
            this.SetSkill(SkillName.Wrestling, 99.2, 113.7);

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

        public EnslavedGoblinScout(Serial serial)
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