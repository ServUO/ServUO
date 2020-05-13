using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("Acid Elemental [Renowned] corpse")]
    public class AcidElementalRenowned : BaseRenowned
    {
        [Constructable]
        public AcidElementalRenowned()
            : base(AIType.AI_Mage)
        {
            Name = "Acid Elemental";
            Title = "[Renowned]";
            Body = 0x9E;
            BaseSoundID = 278;

            SetStr(450, 600);
            SetDex(120, 185);
            SetInt(361, 435);

            SetHits(2000, 2400);

            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Poison, 50);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 40, 70);
            SetResistance(ResistanceType.Fire, 30, 50);
            SetResistance(ResistanceType.Cold, 20, 40);
            SetResistance(ResistanceType.Poison, 10, 30);
            SetResistance(ResistanceType.Energy, 20, 50);

            SetSkill(SkillName.EvalInt, 80.1, 100.0);
            SetSkill(SkillName.Magery, 80.1, 100.0);
            SetSkill(SkillName.MagicResist, 65.2, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 12500;
            Karma = -12500;
        }

        public AcidElementalRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new Type[] { typeof(BreastplateOfTheBerserker), typeof(TerathanWarriorCostume) };
        public override Type[] SharedSAList => new Type[] { typeof(MysticsGarb) };
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItem<Nightshade>(4));
            AddLoot(LootPack.LootItem<LesserPoisonPotion>());
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
