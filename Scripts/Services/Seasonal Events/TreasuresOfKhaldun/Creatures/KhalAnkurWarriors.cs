using Server.Items;
using System;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class KhalAnkurWarriors : BaseCreature
    {
        public static readonly WarriorType[] Types =
            Enum.GetValues(typeof(WarriorType))
                .Cast<WarriorType>()
                .ToArray();

        public static WarriorType RandomType => Types[Utility.Random(Types.Length)];

        public enum WarriorType
        {
            Scout,
            Corporal,
            Lieutenant,
            Captain,
            General
        }

        public WarriorType _Type { get; set; }

        [Constructable]
        public KhalAnkurWarriors()
            : this(RandomType)
        {
        }

        [Constructable]
        public KhalAnkurWarriors(WarriorType type)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            _Type = type;

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            BaseSoundID = 0x45A;

            SetStr(150, 250);
            SetDex(150);
            SetInt(100, 150);

            SetDamage(20, 30);

            SetHits(600, 800);

            string _title = null;

            switch (_Type)
            {
                case WarriorType.Scout: _title = "the Scout"; break;
                case WarriorType.Corporal: _title = "the Corporal"; break;
                case WarriorType.Lieutenant: _title = "the Lieutenant"; break;
                case WarriorType.Captain: _title = "the Captain"; SetHits(1000, 1500); break;
                case WarriorType.General: _title = "the General"; SetHits(1000, 1500); break;
            }

            Title = _title;

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Fencing, 105.0, 130.0);
            SetSkill(SkillName.Macing, 1105.0, 130.0);
            SetSkill(SkillName.MagicResist, 105.0, 130.0);
            SetSkill(SkillName.Swords, 105.0, 130.0);
            SetSkill(SkillName.Tactics, 105.0, 130.0);
            SetSkill(SkillName.Archery, 105.0, 130.0);
            SetSkill(SkillName.Magery, 105.0, 130.0);
            SetSkill(SkillName.Meditation, 105.0, 130.0);

            Fame = 5000;
            Karma = -5000;

            switch (Utility.Random(4))
            {
                case 0:
                    {
                        Hue = 2697;
                        SetWearable(new ChainChest(), Hue);
                        SetWearable(new ChainCoif(), Hue);
                        SetWearable(new Cloak(), Hue);

                        break;
                    }
                case 1:
                    {
                        Hue = 2697;
                        SetWearable(new PlateChest(), Hue);
                        SetWearable(new PlateLegs(), Hue);
                        SetWearable(new Cloak(), Hue);
                        break;
                    }
                case 2:
                    {
                        Hue = 2684;
                        SetWearable(new PlateChest(), Hue);
                        SetWearable(new PlateLegs(), Hue);
                        SetWearable(new PlateArms(), Hue);
                        break;
                    }
                case 3:
                    {
                        Hue = 2697;
                        SetWearable(new ChainChest(), Hue);
                        SetWearable(new ChainLegs(), Hue);
                        SetWearable(new Boots(Hue));
                        break;
                    }
            }

            switch (Utility.Random(2))
            {
                case 0:
                    {
                        SetWearable(Loot.Construct(new Type[] { typeof(Spear), typeof(QuarterStaff), typeof(BlackStaff), typeof(Tessen), typeof(Cleaver), typeof(Lajatang) }));

                        break;
                    }
                case 1:
                    {
                        SetWearable(Loot.Construct(new Type[] { typeof(Yumi), typeof(Crossbow), typeof(RepeatingCrossbow), typeof(HeavyCrossbow) }));

                        RangeFight = 7;
                        AI = AIType.AI_Archer;

                        break;
                    }
            }

            int hairHue = Utility.RandomNondyedHue();

            Utility.AssignRandomHair(this, hairHue);

            if (Utility.Random(7) != 0)
                Utility.AssignRandomFacialHair(this, hairHue);
        }

        public override void OnBeforeDamage(Mobile from, ref int totalDamage, DamageType type)
        {
            if (Region.IsPartOf("Khaldun") && IsChampionSpawn && !Caddellite.CheckDamage(from, type))
            {
                totalDamage = 0;
            }

            base.OnBeforeDamage(from, ref totalDamage, type);
        }

        public override bool AlwaysMurderer => true;
        public override bool ShowFameTitle => false;

        public KhalAnkurWarriors(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep != null && !(wep is Fists))
            {
                if (Utility.RandomDouble() > 0.5)
                    return wep.PrimaryAbility;

                return wep.SecondaryAbility;
            }

            return null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Type = (WarriorType)reader.ReadInt();
        }
    }
}
