using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a lizardman corpse")]
    public class LizardmanDefender : BaseCreature
    {
        [Constructable]
        public LizardmanDefender()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("lizardman");
            Title = "the defender";
            Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;
            Hue = 1949;

            SetStr(157, 180);
            SetDex(105, 108);
            SetInt(50, 57);

            SetHits(1100, 1157);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);

            SetSkill(SkillName.MagicResist, 65.1, 90.0);
            SetSkill(SkillName.Tactics, 95.1, 120.0);
            SetSkill(SkillName.Wrestling, 95.1, 120.0);

            Fame = 11000;
            Karma = -11000;
        }

        public LizardmanDefender(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType => InhumanSpeech.Lizardman;

        public override bool CanRummageCorpses => true;

        public override int TreasureMapLevel => 3;

        public override int Meat => 1;

        public override int Hides => 12;

        public override HideType HideType => HideType.Spined;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}