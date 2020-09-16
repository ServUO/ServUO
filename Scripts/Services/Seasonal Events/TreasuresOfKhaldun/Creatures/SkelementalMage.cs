using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal corpse")]
    public class SkelementalMage : BaseCreature
    {
        [Constructable]
        public SkelementalMage()
            : this(Utility.RandomBool() ? SkelementalKnight.SkeletalType.Cold : SkelementalKnight.SkeletalType.Fire)
        {
        }

        [Constructable]
        public SkelementalMage(SkelementalKnight.SkeletalType type)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Skelemental Mage";
            Body = 0x32;
            BaseSoundID = 451;

            int fire = 100, cold = 100, poison = 100, energy = 100;

            switch (type)
            {
                case SkelementalKnight.SkeletalType.Fire:
                    {
                        Hue = 2634;
                        SetDamageType(ResistanceType.Fire, 100);
                        cold = 5;
                        break;
                    }
                case SkelementalKnight.SkeletalType.Cold:
                    {
                        Hue = 2581;
                        SetDamageType(ResistanceType.Cold, 100);
                        fire = 5;
                        break;
                    }
                case SkelementalKnight.SkeletalType.Poison:
                    {
                        Hue = 2688;
                        SetDamageType(ResistanceType.Poison, 100);
                        energy = 5;
                        break;
                    }
                case SkelementalKnight.SkeletalType.Energy:
                    {
                        Hue = 2717;
                        SetDamageType(ResistanceType.Energy, 100);
                        poison = 5;
                        break;
                    }
            }

            SetStr(200, 250);
            SetDex(70, 100);
            SetInt(100, 130);

            SetHits(100, 150);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 0);

            SetResistance(ResistanceType.Physical, 95);
            SetResistance(ResistanceType.Fire, fire);
            SetResistance(ResistanceType.Cold, cold);
            SetResistance(ResistanceType.Poison, poison);
            SetResistance(ResistanceType.Energy, energy);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 100.0);
            SetSkill(SkillName.DetectHidden, 50.0);
            SetSkill(SkillName.Magery, 110.0, 120.0);
            SetSkill(SkillName.Meditation, 150.0, 155.0);
            SetSkill(SkillName.Focus, 0.0, 60.0);

            Fame = 3000;
            Karma = -3000;
        }

        public SkelementalMage(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override TribeType Tribe => TribeType.Undead;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.Potions);
            AddLoot(LootPack.MageryRegs, 3);
            AddLoot(LootPack.NecroRegs, Utility.RandomMinMax(3, 10));
            AddLoot(LootPack.LootItem<Bone>());
        }

        public override void OnBeforeDamage(Mobile from, ref int totalDamage, DamageType type)
        {
            if (Region.IsPartOf("Khaldun") && IsChampionSpawn && !Caddellite.CheckDamage(from, type))
            {
                totalDamage = 0;
            }

            base.OnBeforeDamage(from, ref totalDamage, type);
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
