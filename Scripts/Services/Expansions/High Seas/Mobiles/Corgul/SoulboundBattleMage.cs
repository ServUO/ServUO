using Server;
using System;

namespace Server.Mobiles
{
    public class SoulboundBattleMage : EvilMageLord
    {
        [Constructable]
        public SoulboundBattleMage()
        {
            Title = "the soulbound battle mage";

            SetStr(156);
            SetDex(101);
            SetInt(181);

            SetHits(419);
            SetMana(619);

            SetDamage(12, 17);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 120.0, 125.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 110.0);
            SetSkill(SkillName.Anatomy, 1.0, 0.0);
            SetSkill(SkillName.Magery, 105.0, 110.0);
            SetSkill(SkillName.EvalInt, 95.0, 100.0);
            SetSkill(SkillName.Meditation, 20.0, 30.0);

            Fame = 5000;
            Karma = -5000;
        }


        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
        }

        public SoulboundBattleMage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}