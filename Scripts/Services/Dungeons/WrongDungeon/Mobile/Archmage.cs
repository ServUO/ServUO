using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an archmage corpse")]
    public class Archmage : BaseCreature
    {
        [Constructable]
        public Archmage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("evil mage");
            Title = "The Insane The Archmage";
            Body = Utility.RandomList(125, 126);

            SetStr(85, 90);
            SetDex(194, 203);
            SetInt(237, 241);

            SetHits(3106, 3122);
            SetMana(578, 616);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 68);
            SetResistance(ResistanceType.Fire, 65, 66);
            SetResistance(ResistanceType.Cold, 62, 69);
            SetResistance(ResistanceType.Poison, 62, 67);
            SetResistance(ResistanceType.Energy, 64, 68);

            SetSkill(SkillName.EvalInt, 88.9, 94.1);
            SetSkill(SkillName.Magery, 96.9, 98.4);
            SetSkill(SkillName.Meditation, 89.9, 90.7);
            SetSkill(SkillName.MagicResist, 87.2, 88.7);
            SetSkill(SkillName.Tactics, 78.2, 79.9);
            SetSkill(SkillName.Wrestling, 84.8, 92.6);

            Fame = 14500;
            Karma = -14500;
        }

        public Archmage(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;

        public override bool AlwaysMurderer => true;

        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.MageryRegs, 23);
            AddLoot(LootPack.RandomLootItem(new Type[] { typeof(BloodOathScroll), typeof(CurseWeaponScroll), typeof(StrangleScroll), typeof(LichFormScroll) }, false, true));
            AddLoot(LootPack.LootItem<SeveredHumanEars>(75.0, 1));
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
