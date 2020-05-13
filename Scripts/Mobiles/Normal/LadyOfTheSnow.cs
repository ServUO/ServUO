using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lady of the snow corpse")]
    public class LadyOfTheSnow : BaseCreature
    {
        [Constructable]
        public LadyOfTheSnow()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lady of the snow";
            Body = 252;
            BaseSoundID = 0x482;

            SetStr(276, 305);
            SetDex(106, 125);
            SetInt(471, 495);

            SetHits(596, 625);

            SetDamage(13, 20);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 40, 55);
            SetResistance(ResistanceType.Cold, 70, 90);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 65, 85);

            SetSkill(SkillName.Magery, 95.1, 110.0);
            SetSkill(SkillName.MagicResist, 90.1, 105.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);
            SetSkill(SkillName.Necromancy, 90, 110.0);
            SetSkill(SkillName.SpiritSpeak, 90.0, 110.0);

            Fame = 15200;
            Karma = -15200;

            SetWeaponAbility(WeaponAbility.ColdWind);
        }

        public LadyOfTheSnow(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 4;
        public override int GetDeathSound()
        {
            return 0x370;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MageryRegs, 3);
            AddLoot(LootPack.BonsaiSeed);
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
