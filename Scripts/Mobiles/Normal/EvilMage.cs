using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an evil mage corpse")]
    public class EvilMage : BaseCreature
    {
        [Constructable]
        public EvilMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("evil mage");
            Title = "the evil mage";

            Robe robe = new Robe(Utility.RandomNeutralHue());
            Sandals sandals = new Sandals();

            Body = 124;

            PackItem(robe);
            PackItem(sandals);

            SetStr(81, 105);
            SetDex(91, 115);
            SetInt(96, 120);

            SetHits(49, 63);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.EvalInt, 75.1, 100.0);
            SetSkill(SkillName.Magery, 75.1, 100.0);
            SetSkill(SkillName.MagicResist, 75.0, 97.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 20.2, 60.0);

            Fame = 2500;
            Karma = -2500;
        }

        public override int GetDeathSound()
        {
            return 0x423;
        }

        public override int GetHurtSound()
        {
            return 0x436;
        }

        public EvilMage(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;
        public override int Meat => 1;
        public override int TreasureMapLevel => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.MageryRegs, 6);
            AddLoot(LootPack.RandomLootItem(new[] { typeof(BloodOathScroll), typeof(CurseWeaponScroll), typeof(StrangleScroll) }, 16.6, 1, false, true));
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
