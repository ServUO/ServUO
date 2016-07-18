using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("Vitavi [Renowned] corpse")]
    public class VitaviRenowned : BaseRenowned
    {
        [Constructable]
        public VitaviRenowned()
            : base(AIType.AI_Mage)
        {
            this.Name = "Vitavi";
            this.Title = "[Renowned]";
            this.Body = 0x8F;
            this.BaseSoundID = 437;

            this.SetStr(300, 350);
            this.SetDex(250, 300);
            this.SetInt(300, 350);

            this.SetHits(45000, 50000);

            this.SetDamage(7, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 30, 50);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.EvalInt, 70.1, 80.0);
            this.SetSkill(SkillName.Magery, 70.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 75.1, 100.0);
            this.SetSkill(SkillName.Tactics, 70.1, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 75.0);

            this.Fame = 7500;
            this.Karma = -7500;

            this.VirtualArmor = 44;
            this.QLPoints = 50;

            this.PackItem(new EssenceBalance());
            this.PackReg(6);

            if (0.02 > Utility.RandomDouble())
                this.PackStatue();
        }

        public VitaviRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get { return new Type[] {}; }
        }

        public override Type[] SharedSAList
        {
            get { return new[] {typeof (AxeOfAbandon), typeof (DemonBridleRing), typeof (VoidInfusedKilt)}; }
        }

        public override InhumanSpeech SpeechType
        {
            get { return InhumanSpeech.Ratman; }
        }
        public override bool AllureImmune
        {
            get
            {
                return true;
            }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public override int Hides
        {
            get { return 8; }
        }

        public override HideType HideType
        {
            get { return HideType.Spined; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            if (Body == 42)
            {
                Body = 0x8F;
                Hue = 0;
            }
        }
    }
}