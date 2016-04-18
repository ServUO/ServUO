using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("Rakktavi [Renowned] corpse")]
    public class RakktaviRenowned : BaseRenowned
    {
        [Constructable]
        public RakktaviRenowned()
            : base(AIType.AI_Archer)
        {
            this.Name = "Rakktavi";
            this.Title = "[Renowned]";
            this.Body = 0x8E;
            this.BaseSoundID = 437;

            this.SetStr(119);
            this.SetDex(279);
            this.SetInt(327);

            this.SetHits(50000);
			this.SetMana(327);
			this.SetStam(279);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 30);
            this.SetResistance(ResistanceType.Fire, 10, 25);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.Archery, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 66.0);
            this.SetSkill(SkillName.Tactics, 68.1);
            this.SetSkill(SkillName.Wrestling, 85.5);

            this.Fame = 6500;
            this.Karma = -6500;

            this.VirtualArmor = 56;

            this.PackItem(new EssenceBalance());
			
            this.AddItem(new Bow());
            this.PackItem(new Arrow(Utility.RandomMinMax(10, 30)));
        }

        public RakktaviRenowned(Serial serial)
            : base(serial)
        {
        }
        public override bool AllureImmune
        {
            get
            {
                return true;
            }
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
                return new Type[] { typeof(CavalrysFolly), typeof(TorcOfTheGuardians) };
            }
        }
        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Ratman;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
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

            if (this.Body == 42)
            {
                this.Body = 0x8E;
                this.Hue = 0;
            }
        }
    }
}