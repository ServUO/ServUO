using Server.Items;

namespace Server.Mobiles
{
    public interface IAcidCreature
    {
    }

    [CorpseName("an acid elementals corpse")]
    public class AcidElemental : BaseCreature, IAcidCreature
    {
        [Constructable]
        public AcidElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an acid elemental";
            Body = 158;
            BaseSoundID = 263;

            SetStr(326, 355);
            SetDex(66, 85);
            SetInt(271, 295);

            SetHits(196, 213);

            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Poison, 90);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 30.3, 60.0);
            SetSkill(SkillName.EvalInt, 80.1, 95.0);
            SetSkill(SkillName.Magery, 70.1, 85.0);
            SetSkill(SkillName.Meditation, 0.0, 0.0);
            SetSkill(SkillName.MagicResist, 60.1, 85.0);
            SetSkill(SkillName.Tactics, 80.1, 90.0);
            SetSkill(SkillName.Wrestling, 70.1, 90.0);

            Fame = 10000;
            Karma = -10000;
        }

        public AcidElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
        public override double HitPoisonChance => 0.75;
        public override int TreasureMapLevel => 2;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.LootItem<Nightshade>(4, true));
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
