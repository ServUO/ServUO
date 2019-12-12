using System;
using Server.Items;

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

            VirtualArmor = 49;

            if (Utility.RandomDouble() < 0.2)
                PackItem(new PumpkinCarvingKit());
        }

        public TheButcher(Serial serial)
            : base(serial)
        {
        }

        public override bool IgnoreYoungProtection { get { return Core.ML; } }
        public override bool AutoDispel { get { return true; } }
        public override bool BardImmune { get { return !Core.SE; } }
        public override bool Unprovokable { get { return Core.SE; } }
        public override bool AreaPeaceImmune { get { return Core.SE; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return 0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.BleedAttack;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
