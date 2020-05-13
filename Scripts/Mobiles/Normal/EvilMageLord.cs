using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an evil mage lord corpse")]
    public class EvilMageLord : BaseCreature
    {
        [Constructable]
        public EvilMageLord()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("evil mage lord");
            Body = Utility.RandomList(125, 126);

            PackItem(new Robe(Utility.RandomMetalHue()));
            PackItem(new WizardsHat(Utility.RandomMetalHue()));

            SetStr(81, 105);
            SetDex(191, 215);
            SetInt(126, 150);

            SetHits(49, 63);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 80.2, 100.0);
            SetSkill(SkillName.Magery, 95.1, 100.0);
            SetSkill(SkillName.Meditation, 27.5, 50.0);
            SetSkill(SkillName.MagicResist, 77.5, 100.0);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 20.3, 80.0);

            Fame = 10500;
            Karma = -10500;
        }

        public override int GetDeathSound()
        {
            return 0x423;
        }

        public override int GetHurtSound()
        {
            return 0x436;
        }

        public EvilMageLord(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;
        public override int Meat => 1;
        public override int TreasureMapLevel => 2;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.MageryRegs, 23);
            AddLoot(LootPack.RandomLootItem(new[] { typeof(BloodOathScroll), typeof(CurseWeaponScroll), typeof(StrangleScroll), typeof(LichFormScroll) }, 25.0, 1, false, true));
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
