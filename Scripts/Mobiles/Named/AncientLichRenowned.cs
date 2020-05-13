using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("Ancient Lich [Renowned] corpse")]
    public class AncientLichRenowned : BaseRenowned
    {
        [Constructable]
        public AncientLichRenowned()
            : base(AIType.AI_NecroMage)
        {
            Name = "Ancient Lich";
            Title = "[Renowned]";
            Body = 78;
            BaseSoundID = 412;

            SetStr(250, 305);
            SetDex(96, 115);
            SetInt(966, 1045);

            SetHits(2000, 2500);

            SetDamage(15, 27);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 25, 30);

            SetSkill(SkillName.EvalInt, 120.1, 130.0);
            SetSkill(SkillName.Magery, 120.1, 130.0);
            SetSkill(SkillName.Meditation, 100.1, 101.0);
            SetSkill(SkillName.MagicResist, 175.2, 200.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 75.1, 100.0);

            Fame = 23000;
            Karma = -23000;
        }

        public AncientLichRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new Type[] { typeof(SpinedBloodwormBracers), typeof(DefenderOfTheMagus) };
        public override Type[] SharedSAList => new Type[] { typeof(SummonersKilt) };

        public override bool Unprovokable => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int GetIdleSound()
        {
            return 0x19D;
        }

        public override int GetAngerSound()
        {
            return 0x175;
        }

        public override int GetDeathSound()
        {
            return 0x108;
        }

        public override int GetAttackSound()
        {
            return 0xE2;
        }

        public override int GetHurtSound()
        {
            return 0x28B;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.NecroRegs, 30, 275);
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
