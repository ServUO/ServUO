using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Master Theophilus corpse")]
    public class MasterTheophilus : EvilMageLord
    {
        [Constructable]
        public MasterTheophilus()
        {
            Name = "Master Theophilus";
            Title = "the necromancer";
            Hue = 0;

            SetStr(137, 187);
            SetDex(253, 301);
            SetInt(393, 444);

            SetHits(663, 876);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 60);
            SetResistance(ResistanceType.Fire, 50, 58);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 69.9, 105.3);
            SetSkill(SkillName.Tactics, 113.0, 117.9);
            SetSkill(SkillName.MagicResist, 127.0, 132.8);
            SetSkill(SkillName.Magery, 138.1, 143.7);
            SetSkill(SkillName.EvalInt, 125.6, 133.8);
            SetSkill(SkillName.Necromancy, 125.6, 133.8);
            SetSkill(SkillName.SpiritSpeak, 125.6, 133.8);
            SetSkill(SkillName.Meditation, 128.8, 132.9);

            Fame = 18000;
            Karma = -18000;

            AddItem(new Shoes(0x537));
            AddItem(new Robe(0x452));

            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
        }

        public MasterTheophilus(Serial serial)
            : base(serial)
        {
        }
        public override bool CanBeParagon => false;
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Paragon.ChestChance > Utility.RandomDouble())
                c.DropItem(new ParagonChest(Name, 5));
        }

        public override bool AllureImmune => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.MedScrolls, 4);
            AddLoot(LootPack.HighScrolls, 4);
            AddLoot(LootPack.MageryRegs, 22);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
            AddLoot(LootPack.LootItem<DisintegratingThesisNotes>(15.0));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
