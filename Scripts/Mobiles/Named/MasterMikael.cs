using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Master Mikael corpse")]
    public class MasterMikael : BoneMagi
    {
        [Constructable]
        public MasterMikael()
        {
            Name = "Master Mikael";
            Hue = 0x8FD;

            SetStr(93, 122);
            SetDex(91, 100);
            SetInt(252, 271);

            SetHits(789, 1014);

            SetDamage(11, 19);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 59);
            SetResistance(ResistanceType.Fire, 40, 46);
            SetResistance(ResistanceType.Cold, 72, 80);
            SetResistance(ResistanceType.Poison, 44, 49);
            SetResistance(ResistanceType.Energy, 50, 57);

            SetSkill(SkillName.Wrestling, 80.1, 87.2);
            SetSkill(SkillName.Tactics, 79.0, 90.9);
            SetSkill(SkillName.MagicResist, 90.3, 106.9);
            SetSkill(SkillName.Magery, 103.8, 108.0);
            SetSkill(SkillName.EvalInt, 96.1, 105.3);
            SetSkill(SkillName.Necromancy, 103.8, 108.0);
            SetSkill(SkillName.SpiritSpeak, 96.1, 105.3);

            Fame = 18000;
            Karma = -18000;
        }

        public MasterMikael(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeParagon => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 2);
            AddLoot(LootPack.MageryRegs, 3);
            AddLoot(LootPack.NecroRegs, 1, 10);
            AddLoot(LootPack.LootItem<DisintegratingThesisNotes>(15.0));
            AddLoot(LootPack.Parrot);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
        }

        // TODO: Special move?
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
