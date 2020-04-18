using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("Green Goblin Alchemist [Renowned] corpse")]
    public class GreenGoblinAlchemistRenowned : BaseRenowned
    {
        [Constructable]
        public GreenGoblinAlchemistRenowned()
            : base(AIType.AI_Melee)
        {
            Name = "Green Goblin Alchemist";
            Title = "[Renowned]";
            Body = 723;
            BaseSoundID = 0x600;

            SetStr(600, 650);
            SetDex(50, 70);
            SetInt(100, 250);

            SetHits(1000, 1500);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 55);
            SetResistance(ResistanceType.Fire, 55, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 20, 25);

            SetSkill(SkillName.MagicResist, 120.0, 125.0);
            SetSkill(SkillName.Tactics, 95.0, 100.0);
            SetSkill(SkillName.Wrestling, 100.0, 110.0);

            Fame = 1500;
            Karma = -1500;
        }

        public GreenGoblinAlchemistRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new Type[] { typeof(ObsidianEarrings), typeof(TheImpalersPick) };
        public override Type[] SharedSAList => new Type[] { };

        public override bool AllureImmune => true;

        public override int GetAngerSound() { return 0x600; }
        public override int GetIdleSound() { return 0x600; }
        public override int GetAttackSound() { return 0x5FD; }
        public override int GetHurtSound() { return 0x5FF; }
        public override int GetDeathSound() { return 0x5FE; }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
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