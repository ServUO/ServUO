/* Based on Gargoyle, still no infos on Undead Gargoyle... Have to get also the correct body ID */
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a putrid undead gargoyle corpse")]
    public class PutridUndeadGargoyle : BaseCreature
    {
        [Constructable]
        public PutridUndeadGargoyle()
            : base(AIType.AI_Mystic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a putrid undead gargoyle";
            Body = 722;
            BaseSoundID = 372;
            Hue = 1778;

            SetStr(525);
            SetDex(120, 125);
            SetInt(1145);

            SetHits(660, 665);

            SetDamage(21, 30);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 55, 60);
            SetResistance(ResistanceType.Fire, 33);
            SetResistance(ResistanceType.Cold, 50);
            SetResistance(ResistanceType.Poison, 50, 55);
            SetResistance(ResistanceType.Energy, 45, 50);

            SetSkill(SkillName.EvalInt, 125.0);
            SetSkill(SkillName.Magery, 127.0);
            SetSkill(SkillName.Mysticism, 100.0);
            SetSkill(SkillName.Meditation, 105.0, 108.0);
            SetSkill(SkillName.MagicResist, 180.0, 187.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0, 102.0);

            Fame = 3500;
            Karma = -3500;
        }

        public PutridUndeadGargoyle(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override bool Unprovokable => true;
        public override bool BardImmune => true;
        public override Poison PoisonImmune => Poison.Deadly;
        public override Poison HitPoison => Poison.Deadly;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 5);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 4));
            AddLoot(LootPack.LootItem<TatteredAncientScroll>(5.0));
            AddLoot(LootPack.LootItem<InfusedGlassStave>(10.0));
            AddLoot(LootPack.LootItem<AncientPotteryFragments>(15.0));
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
