using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an usagralem ballem corpse")]
    public class UsagralemBallem : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Killing;
        public override int Stage => 3;

        [Constructable]
        public UsagralemBallem()
            : base(AIType.AI_Melee, 10, 1, 0.2, 0.4)
        {
            Name = "usagrallem ballem";
            Hue = 2071;
            Body = 318;
            BaseSoundID = 0x165;

            SetStr(900, 1000);
            SetDex(1028);
            SetInt(1000, 1100);

            SetHits(2000, 2200);
            SetMana(5000);

            SetDamage(17, 21);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 40, 60);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.MagicResist, 80.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 90.0);
            SetSkill(SkillName.Wrestling, 80.0, 90.0);

            Fame = 18000;
            Karma = -18000;

            SetWeaponAbility(WeaponAbility.DoubleStrike);
            SetWeaponAbility(WeaponAbility.WhirlwindAttack);
            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        public UsagralemBallem(Serial serial)
            : base(serial)
        {
        }

        public override bool IgnoreYoungProtection => true;

        public override bool Unprovokable => true;

        public override bool AreaPeaceImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 1);
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.LootItem<DaemonBone>(30, true));
            AddLoot(LootPack.LootItem<AncientPotteryFragments>(30.0));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
