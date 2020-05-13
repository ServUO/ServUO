using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a ratman archer corpse")]
    public class RatmanArcher : BaseCreature
    {
        [Constructable]
        public RatmanArcher()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("ratman");
            Body = 0x8E;
            BaseSoundID = 437;

            SetStr(146, 180);
            SetDex(101, 130);
            SetInt(116, 140);

            SetHits(88, 108);

            SetDamage(4, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 60.2, 100.0);
            SetSkill(SkillName.Archery, 80.1, 90.0);
            SetSkill(SkillName.MagicResist, 65.1, 90.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

            Fame = 6500;
            Karma = -6500;

            AddItem(new Bow());
        }

        public RatmanArcher(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType => InhumanSpeech.Ratman;
        public override bool CanRummageCorpses => true;
        public override int Hides => 8;
        public override HideType HideType => HideType.Spined;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.LootItem<Arrow>(50, 70, true));
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
