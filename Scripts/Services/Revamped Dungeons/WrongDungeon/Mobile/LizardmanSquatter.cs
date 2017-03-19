using System;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a lizardman squatter corpse")]
    public class LizardmanSquatter : BaseCreature
    {
        [Constructable]
        public LizardmanSquatter()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("lizardman");
            this.Title = "the squatter";
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;

            SetStr(105, 138);
            SetDex(89, 114);
            SetInt(50, 66);

            SetHits(787, 869);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);

            SetSkill(SkillName.MagicResist, 35.1, 90.0);
            SetSkill(SkillName.Tactics, 95.1, 120.0);
            SetSkill(SkillName.Wrestling, 95.1, 120.0);

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 28;
        }

        public LizardmanSquatter(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Lizardman;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 12;
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
            AddLoot(LootPack.Meager);
            // TODO: weapon
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