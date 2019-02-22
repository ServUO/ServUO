using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal corpse")]
    public class SkelementalKnight : BaseCreature
    {
        public enum SkeletalType
        {
            Fire,
            Cold,
            Poison,
            Energy
        }

        [Constructable]
        public SkelementalKnight()
            : this(Utility.RandomBool() ? SkeletalType.Energy : SkeletalType.Poison)
        {
        }

        [Constructable]
        public SkelementalKnight(SkeletalType type)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Skelemental Knight";
            Body = 0x93;
            BaseSoundID = 451;

            int fire = 100, cold = 100, poison = 100, energy = 100;

            switch (type)
            {
                case SkeletalType.Fire:
                    {
                        Hue = 2634;
                        SetDamageType(ResistanceType.Fire, 100);
                        cold = 5;
                        break;
                    }
                case SkeletalType.Cold:
                    {
                        Hue = 2581;
                        SetDamageType(ResistanceType.Cold, 100);
                        fire = 5;
                        break;
                    }
                case SkeletalType.Poison:
                    {
                        Hue = 2688;
                        SetDamageType(ResistanceType.Poison, 100);
                        energy = 5;
                        break;
                    }
                case SkeletalType.Energy:
                    {
                        Hue = 2717;
                        SetDamageType(ResistanceType.Energy, 100);
                        poison = 5;
                        break;
                    }
            }

            SetStr(200, 250);
            SetDex(70, 95);
            SetInt(35, 60);

            SetHits(110, 150);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 0);

            SetResistance(ResistanceType.Physical, 95);
            SetResistance(ResistanceType.Fire, fire);
            SetResistance(ResistanceType.Cold, cold);
            SetResistance(ResistanceType.Poison, poison);
            SetResistance(ResistanceType.Energy, energy);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 85.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 100.0);
            SetSkill(SkillName.DetectHidden, 40.0);

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 40;

            switch (Utility.Random(6))
            {
                case 0:
                    PackItem(new PlateArms());
                    break;
                case 1:
                    PackItem(new PlateChest());
                    break;
                case 2:
                    PackItem(new PlateGloves());
                    break;
                case 3:
                    PackItem(new PlateGorget());
                    break;
                case 4:
                    PackItem(new PlateLegs());
                    break;
                case 5:
                    PackItem(new PlateHelm());
                    break;
            }

            PackItem(new Scimitar());
            PackItem(new WoodenShield());
        }

        public override void OnBeforeDamage(Mobile from, ref int totalDamage, Server.DamageType type)
        {
            if (Region.IsPartOf("Khaldun") && IsChampionSpawn && !Caddellite.CheckDamage(from, type))
            {
                totalDamage = 0;
            }

            base.OnBeforeDamage(from, ref totalDamage, type);
        }

        public SkelementalKnight(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune { get { return true; } }

        public override TribeType Tribe { get { return TribeType.Undead; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
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
