using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("Gray Goblin Mage [Renowned] corpse")] 
    public class GrayGoblinMageRenowned : BaseRenowned
    {
        [Constructable]
        public GrayGoblinMageRenowned()
            : base(AIType.AI_Mage)
        {
            this.Name = "Gray Goblin Mage";
            this.Title = "[Renowned]";
            this.Body = 723;
            this.BaseSoundID = 437;

            this.SetStr(550, 600);
            this.SetDex(70, 75);
            this.SetInt(500, 600);

            this.SetHits(1100, 1300);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 45, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.MagicResist, 120.0, 125.0);
            this.SetSkill(SkillName.Tactics, 95.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 110.0);
            this.SetSkill(SkillName.EvalInt, 100.0, 120.0);
            this.SetSkill(SkillName.Meditation, 100.0, 105.0);
            this.SetSkill(SkillName.Magery, 100.0, 110.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;
            this.QLPoints = 50;

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

        public GrayGoblinMageRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(StormCaller), typeof(TorcOfTheGuardians), typeof(GiantSteps), typeof(CavalrysFolly) };
            }
        }
        public override bool AllureImmune
        {
            get
            {
                return true;
            }
        }
        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Orc;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
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