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

            this.SetStr(315, 354);
            this.SetDex(139, 177);
            this.SetInt(243, 288);

            this.SetHits(50000);
			this.SetMana(243, 288);
			this.SetStam(139, 177);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 26, 28);
            this.SetResistance(ResistanceType.Fire, 22, 25);
            this.SetResistance(ResistanceType.Cold, 30, 38);
            this.SetResistance(ResistanceType.Poison, 14, 17);
            this.SetResistance(ResistanceType.Energy, 15, 18);

            this.SetSkill(SkillName.MagicResist, 40.4);
            this.SetSkill(SkillName.Tactics, 73.6);
            this.SetSkill(SkillName.Wrestling, 66.5);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;
        }

        public TikitaviRenowned(Serial serial)
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
                return new Type[] { typeof(BasiliskHideBreastplate) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(LegacyOfDespair), typeof(MysticsGarb) };
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
            this.AddLoot(LootPack.FilthyRich, 3);
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