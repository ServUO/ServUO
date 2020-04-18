using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("Gray Goblin Mage [Renowned] corpse")]
    public class GrayGoblinMageRenowned : BaseRenowned
    {
        [Constructable]
        public GrayGoblinMageRenowned()
            : base(AIType.AI_Mage)
        {
            Name = "Gray Goblin Mage";
            Title = "[Renowned]";

            Body = 723;
            Hue = 1900;

            BaseSoundID = 0x600;

            SetStr(550, 600);
            SetDex(70, 75);
            SetInt(500, 600);

            SetHits(1100, 1300);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 45, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 20, 25);

            SetSkill(SkillName.MagicResist, 120.0, 125.0);
            SetSkill(SkillName.Tactics, 95.0, 100.0);
            SetSkill(SkillName.Wrestling, 100.0, 110.0);
            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Meditation, 100.0, 105.0);
            SetSkill(SkillName.Magery, 100.0, 110.0);

            Fame = 1500;
            Karma = -1500;
        }

        public GrayGoblinMageRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new Type[] { };
        public override Type[] SharedSAList => new Type[] { typeof(StormCaller), typeof(TorcOfTheGuardians), typeof(GiantSteps), typeof(CavalrysFolly) };
        public override bool AllureImmune => true;

        public override int GetAngerSound() { return 0x600; }
        public override int GetIdleSound() { return 0x600; }
        public override int GetAttackSound() { return 0x5FD; }
        public override int GetHurtSound() { return 0x5FF; }
        public override int GetDeathSound() { return 0x5FE; }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}