using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clan chitter assistant corpse")]
    public class ClanCA : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Ratman; } }
        [Constructable]
        public ClanCA()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clan Chitter Assistant";
            this.Body = 0x8E;
            this.BaseSoundID = 437;

            this.SetStr(146, 175);
            this.SetDex(101, 130);
            this.SetInt(120, 135);

            this.SetHits(120, 145);

            this.SetDamage(4, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 23, 35);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Poison, 15, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.Archery, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 81.1, 90.0);
            this.SetSkill(SkillName.Tactics, 53.8, 75.0);
            this.SetSkill(SkillName.Wrestling, 62.3, 75.0);

            this.Fame = 6500;
            this.Karma = -6500;

            this.VirtualArmor = 56;

            this.AddItem(new Bow());
            this.PackItem(new Arrow(Utility.RandomMinMax(50, 70)));
        }

        public ClanCA(Serial serial)
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
            this.AddLoot(LootPack.Rich);
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