using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shimmering effusion corpse")]
    public class ShimmeringEffusion : BasePeerless
    {
        [Constructable]
        public ShimmeringEffusion()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shimmering effusion";
            Body = 0x105;

            SetStr(500, 550);
            SetDex(350, 400);
            SetInt(1500, 1600);

            SetHits(20000);

            SetDamage(27, 31);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 60, 80);
            SetResistance(ResistanceType.Cold, 60, 80);
            SetResistance(ResistanceType.Poison, 60, 80);
            SetResistance(ResistanceType.Energy, 60, 80);

            SetSkill(SkillName.Wrestling, 100.0, 105.0);
            SetSkill(SkillName.Tactics, 100.0, 105.0);
            SetSkill(SkillName.MagicResist, 150);
            SetSkill(SkillName.Magery, 150.0);
            SetSkill(SkillName.EvalInt, 150.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Spellweaving, 120.0);

            Fame = 30000;
            Karma = -30000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
            AddLoot(LootPack.Parrot, 2);
            AddLoot(LootPack.HighScrolls, 3);
            AddLoot(LootPack.MedScrolls, 3);
            AddLoot(LootPack.ArcanistScrolls, Utility.RandomMinMax(1, 6));
            AddLoot(LootPack.PeerlessResource, 8);
            AddLoot(LootPack.Talisman, 5);
            AddLoot(LootPack.LootItem<CapturedEssence>());
            AddLoot(LootPack.LootItem<ShimmeringCrystals>());

            AddLoot(LootPack.RandomLootItem(new[] { typeof(ShimmeringEffusionStatuette), typeof(CorporealBrumeStatuette), typeof(MantraEffervescenceStatuette), typeof(FetidEssenceStatuette) }, 5.0, 1));

            AddLoot(LootPack.LootItem<FerretImprisonedInCrystal>(5.0));
            AddLoot(LootPack.LootItem<CrystallineRing>(2.25));
        }

        public override bool AutoDispel => true;
        public override int TreasureMapLevel => 5;
        public override bool HasFireRing => true;
        public override double FireRingChance => 0.1;

        public override int GetIdleSound()
        {
            return 0x1BF;
        }

        public override int GetAttackSound()
        {
            return 0x1C0;
        }

        public override int GetHurtSound()
        {
            return 0x1C1;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }

        #region Helpers
        public override bool CanSpawnHelpers => true;
        public override int MaxHelpersWaves => 4;
        public override double SpawnHelpersChance => 0.1;

        public override void SpawnHelpers()
        {
            int amount = 1;

            if (Altar != null)
                amount = Altar.Fighters.Count;

            if (amount > 5)
                amount = 5;

            for (int i = 0; i < amount; i++)
            {
                switch (Utility.Random(3))
                {
                    case 0:
                        SpawnHelper(new MantraEffervescence(), 2);
                        break;
                    case 1:
                        SpawnHelper(new CorporealBrume(), 2);
                        break;
                    case 2:
                        SpawnHelper(new FetidEssence(), 2);
                        break;
                }
            }
        }

        #endregion

        public ShimmeringEffusion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
