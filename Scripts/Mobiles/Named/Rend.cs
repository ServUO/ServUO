using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Rend corpse")]
    public class Rend : Reptalon
    {
        [Constructable]
        public Rend()
        {

            Name = "Rend";
            Hue = 0x455;

            SetStr(1261, 1284);
            SetDex(363, 384);
            SetInt(601, 642);

            SetHits(5176, 6100);

            SetDamage(26, 33);

            SetDamageType(ResistanceType.Physical, 100);
            SetDamageType(ResistanceType.Poison, 0);
            SetDamageType(ResistanceType.Energy, 0);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 81, 94);
            SetResistance(ResistanceType.Cold, 46, 55);
            SetResistance(ResistanceType.Poison, 35, 44);
            SetResistance(ResistanceType.Energy, 45, 52);

            SetSkill(SkillName.Wrestling, 136.3, 150.3);
            SetSkill(SkillName.Tactics, 133.4, 141.4);
            SetSkill(SkillName.MagicResist, 90.9, 110.0);
            SetSkill(SkillName.Anatomy, 66.6, 72.0);

            Fame = 21000;
            Karma = -21000;

            Tamable = false;

            SetSpecialAbility(SpecialAbility.GraspingClaw);
            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public Rend(Serial serial)
            : base(serial)
        {
        }
        public override bool CanBeParagon => false;
        public override bool GivesMLMinorArtifact => true;

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Paragon.ChestChance > Utility.RandomDouble())
                c.DropItem(new ParagonChest(Name, 5));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
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
