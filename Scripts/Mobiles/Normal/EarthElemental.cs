using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an earth elemental corpse")]
    public class EarthElemental : BaseCreature
    {
        [Constructable]
        public EarthElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an earth elemental";
            Body = 14;
            BaseSoundID = 268;

            SetStr(126, 155);
            SetDex(66, 85);
            SetInt(71, 92);

            SetHits(76, 93);

            SetDamage(9, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 3500;
            Karma = -3500;

            ControlSlots = 2;
        }

        public EarthElemental(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty => 117.5;
        public override double DispelFocus => 45.0;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 1;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItemCallback(SpawnOre, 100.0, 5, false, true));
            AddLoot(LootPack.LootItem<FertileDirt>(1, 4, true));
            AddLoot(LootPack.LootItem<MandrakeRoot>(true));
        }

        private Item SpawnOre(IEntity e)
        {
            Item ore = new IronOre
            {
                ItemID = 0x19B7
            };

            return ore;
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
