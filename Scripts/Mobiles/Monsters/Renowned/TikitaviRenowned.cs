using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("Tikitavi [Renowned] corpse")] 
    public class TikitaviRenowned : BaseRenowned
    {
        [Constructable]
        public TikitaviRenowned()
            : base(AIType.AI_Melee)
        {
            this.Name = "Tikitavi";
            this.Title = "[Renowned]";
            this.Body = 42;
            this.BaseSoundID = 437;

            this.SetStr(300, 350);
            this.SetDex(100, 150);
            this.SetInt(200, 250);

            this.SetHits(45000, 50000);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 35);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 35.1, 60.0);
            this.SetSkill(SkillName.Tactics, 50.1, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 75.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;

            this.PackItem(new EssenceBalance());
        }

        public TikitaviRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(BasiliskHideBreastplate), typeof(CrystallineBlackrock) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(LegacyOfDespair), typeof(CrystalShards), typeof(MysticsGarb) };
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
            this.AddLoot(LootPack.Meager);
            // TODO: weapon, misc
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