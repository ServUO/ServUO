using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tiger corpse")]
    public class WildTiger : BaseMount
    {
        public override double HealChance => .167;
        public virtual Item GetPelt => new TigerPelt(4);

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool CanRide { get; set; }

        [Constructable]
        public WildTiger()
            : this("a wild tiger")
        {
            CanRide = false;
        }

        [Constructable]
        public WildTiger(string name)
            : base(name, Utility.RandomBool() ? 1254 : 1255, 16071, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            if (Body == 1255)
                ItemID = 16072;

            SetStr(496, 554);
            SetDex(88, 124);
            SetInt(94, 163);

            SetHits(352, 450);

            SetDamage(18, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 56, 75);
            SetResistance(ResistanceType.Fire, 21, 40);
            SetResistance(ResistanceType.Cold, 55, 64);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 90.8, 97.5);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.Tactics, 100.2, 102.5);
            SetSkill(SkillName.Wrestling, 90.1, 94.4);

            Fame = 11000;
            Karma = -11000;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 95.1;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.GraspingClaw);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CanRide)
                base.OnDoubleClick(from);

            else if (from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman)
            {
                Container pack = Backpack;

                if (pack != null)
                {
                    pack.DisplayTo(from);
                }
            }
        }

        public override int GetIdleSound() { return 0x673; }
        public override int GetAngerSound() { return 0x670; }
        public override int GetHurtSound() { return 0x672; }
        public override int GetDeathSound() { return 0x671; }

        public override double WeaponAbilityChance => 0.5;

        public override int Meat => 2;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override int TreasureMapLevel => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);

            if (LootStage == LootStage.Death)
            {
                switch (Utility.Random(11))
                {
                    default: AddLoot(LootPack.PeculiarSeed1); break;
                    case 5:
                    case 6:
                    case 7: AddLoot(LootPack.PeculiarSeed2); break;
                    case 8:
                    case 9: AddLoot(LootPack.PeculiarSeed3); break;
                    case 10: AddLoot(LootPack.PeculiarSeed4); break;
                }
            }
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (!Controlled && corpse != null && !corpse.Carved)
            {
                from.SendLocalizedMessage(1156197); // You cut away some pelts, but they remain on the corpse.
                corpse.DropItem(GetPelt);
            }

            base.OnCarve(from, corpse, with);
        }

        public WildTiger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(CanRide);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    CanRide = reader.ReadBool();
                    break;
                case 0:
                    break;
            }
        }
    }

    [CorpseName("a tiger corpse")]
    public class WildWhiteTiger : WildTiger
    {
        public override Item GetPelt => new WhiteTigerPelt(4);

        [Constructable]
        public WildWhiteTiger()
            : base("a wild white tiger")
        {
            Hue = 2500;
        }

        public WildWhiteTiger(Serial serial)
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

    [CorpseName("a tiger corpse")]
    public class WildBlackTiger : WildTiger
    {
        public override Item GetPelt => new BlackTigerPelt(4);

        [Constructable]
        public WildBlackTiger()
            : base("a wild black tiger")
        {
            Hue = 1175;
        }

        public WildBlackTiger(Serial serial)
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
