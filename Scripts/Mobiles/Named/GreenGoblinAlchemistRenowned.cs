using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("Green Goblin Alchemist [Renowned] corpse")]
    public class GreenGoblinAlchemistRenowned : BaseRenowned
    {
        [Constructable]
        public GreenGoblinAlchemistRenowned()
            : base(AIType.AI_Melee)
        {
            this.Name = "Green Goblin Alchemist";
            this.Title = "[Renowned]";
            this.Body = 723;
            this.BaseSoundID = 437;

            this.SetStr(600, 650);
            this.SetDex(50, 70);
            this.SetInt(100, 250);

            this.SetHits(1000, 1500);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 55);
            this.SetResistance(ResistanceType.Fire, 55, 60);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.MagicResist, 120.0, 125.0);
            this.SetSkill(SkillName.Tactics, 95.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 110.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;
			
            this.PackItem(new EssenceControl());

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

        public GreenGoblinAlchemistRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(ObsidianEarrings), typeof(TheImpalersPick) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Orc;
            }
        }
        public override bool AllureImmune
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
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