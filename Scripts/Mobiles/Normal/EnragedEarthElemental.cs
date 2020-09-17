using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an earth elemental corpse")]
    public class EnragedEarthElemental : BaseCreature
    {
        [Constructable]
        public EnragedEarthElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Enraged Earth Elemental";
            Body = 14;
            BaseSoundID = 268;
            Hue = 442;

            SetStr(147, 155);
            SetDex(78, 90);
            SetInt(94, 115);

            SetHits(500, 550);

            SetDamage(9, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Parry, 120.0);

            Fame = 3500;
            Karma = -3500;

            ControlSlots = 2;
        }

        public EnragedEarthElemental(Serial serial)
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
            AddLoot(LootPack.LootItem<FertileDirt>(Utility.RandomMinMax(1, 4)));
            AddLoot(LootPack.LootItem<MandrakeRoot>());
            AddLoot(LootPack.LootItemCallback(SpawnOre));
        }

        private Item SpawnOre(IEntity e)
        {
            var ore = new IronOre(5)
            {
                ItemID = 0x19B7
            };

            return ore;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.03)
                c.DropItem(new LuckyCoin());
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
