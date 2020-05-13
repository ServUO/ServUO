using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a liche's corpse")]
    public class Lich : BaseCreature
    {
        [Constructable]
        public Lich()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lich";
            Body = 24;
            BaseSoundID = 0x3E9;

            SetStr(171, 200);
            SetDex(126, 145);
            SetInt(276, 305);

            SetHits(103, 120);

            SetDamage(24, 26);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Necromancy, 89, 99.1);
            SetSkill(SkillName.SpiritSpeak, 90.0, 99.0);

            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Magery, 70.1, 80.0);
            SetSkill(SkillName.Meditation, 85.1, 95.0);
            SetSkill(SkillName.MagicResist, 80.1, 100.0);
            SetSkill(SkillName.Tactics, 70.1, 90.0);

            Fame = 8000;
            Karma = -8000;
        }

        public Lich(Serial serial)
            : base(serial)
        {
        }

        public override TribeType Tribe => TribeType.Undead;

        public override bool CanRummageCorpses => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 3;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.NecroRegs, 17, 24);
            AddLoot(LootPack.RandomLootItem(new Type[] { typeof(LichFormScroll), typeof(PoisonStrikeScroll), typeof(StrangleScroll), typeof(VengefulSpiritScroll), typeof(WitherScroll) }, false, true));
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
