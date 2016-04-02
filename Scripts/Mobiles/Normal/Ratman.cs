using System;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a ratman's corpse")]
    public class Ratman : BaseCreature
    {
        [Constructable]
        public Ratman()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("ratman");
            this.Body = 42;
            this.BaseSoundID = 437;

            this.SetStr(96, 120);
            this.SetDex(81, 100);
            this.SetInt(36, 60);

            this.SetHits(58, 72);

            this.SetDamage(4, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 35.1, 60.0);
            this.SetSkill(SkillName.Tactics, 50.1, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 75.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 28;
        }

        public Ratman(Serial serial)
            : base(serial)
        {
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