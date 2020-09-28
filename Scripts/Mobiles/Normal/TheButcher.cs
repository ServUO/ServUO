using Server.Items;

using System;

namespace Server.Mobiles
{
    [CorpseName("an butcher corpse")]
    public class TheButcher : BaseCreature
    {
        [Constructable]
        public TheButcher()
            : base(AIType.AI_Necro, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("daemon");
            Title = "the Butcher";
            Hue = 2075;
            Body = 306;
            BaseSoundID = 0x2A7;

            SetStr(250, 300);
            SetDex(90, 130);
            SetInt(800, 1000);

            SetHits(700, 1000);

            SetDamage(15, 27);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 25, 40);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 35, 40);

            SetSkill(SkillName.DetectHidden, 80.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Poisoning, 160.0);
            SetSkill(SkillName.MagicResist, 180.0, 200.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 75.0, 80.0);
            SetSkill(SkillName.Necromancy, 100.0, 110.0);
            SetSkill(SkillName.SpiritSpeak, 95.0, 105.0);

            Fame = 24000;
            Karma = -24000;
        }

        public TheButcher(Serial serial)
            : base(serial)
        {
        }

        public override bool IgnoreYoungProtection => true;

        public override bool AutoDispel => true;

        public override bool Unprovokable => true;

        public override bool AreaPeaceImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override Poison HitPoison => 0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly;

        public override int TreasureMapLevel => 1;

        public override bool AlwaysMurderer => true;

        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.BleedAttack;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.LootItem<PumpkinCarvingKit>(20.0));
            AddLoot(LootPack.RandomLootItem(new Type[] { typeof(ObsidianSkull), typeof(CrystalSkull), typeof(JadeSkull) }, 1.0, 1)); // 1% chance
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
