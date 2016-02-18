using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a goblin keeper corpse")]
    public class GrayGoblinKeeper : BaseCreature
    {
        [Constructable]
        public GrayGoblinKeeper()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Gray goblin keeper";
            this.Body = 334;
            this.BaseSoundID = 0x45A;

            this.SetStr(326);
            this.SetDex(79);
            this.SetInt(114);

            this.SetHits(186);
            this.SetStam(79);
            this.SetMana(114);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45);
            this.SetResistance(ResistanceType.Fire, 33);
            this.SetResistance(ResistanceType.Cold, 25);
            this.SetResistance(ResistanceType.Poison, 20);
            this.SetResistance(ResistanceType.Energy, 10);

            this.SetSkill(SkillName.MagicResist, 129.9);
            this.SetSkill(SkillName.Tactics, 86.7);
            this.SetSkill(SkillName.Anatomy, 86.6);
            this.SetSkill(SkillName.Wrestling, 103.6);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;

            QLPoints = 10;

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

        public GrayGoblinKeeper(Serial serial)
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