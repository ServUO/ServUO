using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a centaur corpse")]
    public class Centaur : BaseCreature
    {
        [Constructable]
        public Centaur()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("centaur");
            Body = 101;
            BaseSoundID = 679;

            SetStr(202, 300);
            SetDex(104, 260);
            SetInt(91, 100);

            SetHits(130, 172);

            SetDamage(13, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.Anatomy, 95.1, 115.0);
            SetSkill(SkillName.Archery, 95.1, 100.0);
            SetSkill(SkillName.MagicResist, 50.3, 80.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 95.1, 100.0);

            Fame = 6500;
            Karma = 0;

            AddItem(new Bow());
        }

        public Centaur(Serial serial)
            : base(serial)
        {
        }

        public override TribeType Tribe => TribeType.Fey;

        public override int Meat => 1;
        public override int Hides => 8;
        public override HideType HideType => HideType.Spined;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItem<Arrow>(Utility.RandomMinMax(80, 90)));
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
