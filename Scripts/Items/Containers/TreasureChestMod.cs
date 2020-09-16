// Treasure Chest Pack - Version 0.99I
// By Nerun

namespace Server.Items
{

    // ---------- [Level 1] ----------
    // Large, Medium and Small Crate
    [Flipable(0xe3e, 0xe3f)]
    public class TreasureLevel1 : BaseTreasureChestMod
    {
        public override int DefaultGumpID => 0x49;

        [Constructable]
        public TreasureLevel1() : base(Utility.RandomList(0xE3C, 0xE3E, 0x9a9))
        {
            RequiredSkill = 52;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;
            TrapType = TrapType.MagicTrap;
            TrapPower = 1 * Utility.Random(1, 25);

            DropItem(new Gold(30, 100));
            DropItem(new Bolt(10));
            DropItem(Loot.RandomClothing());

            AddLoot(Loot.RandomWeapon());
            AddLoot(Loot.RandomArmorOrShield());
            AddLoot(Loot.RandomJewelry());

            for (int i = Utility.Random(3) + 1; i > 0; i--) // random 1 to 3
                DropItem(Loot.RandomGem());
        }

        public TreasureLevel1(Serial serial) : base(serial)
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

    // ---------- [Level 1 Hybrid] ----------
    // Large, Medium and Small Crate
    [Flipable(0xe3e, 0xe3f)]
    public class TreasureLevel1h : BaseTreasureChestMod
    {
        public override int DefaultGumpID => 0x49;

        [Constructable]
        public TreasureLevel1h() : base(Utility.RandomList(0xE3C, 0xE3E, 0x9a9))
        {
            RequiredSkill = 56;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;
            TrapType = TrapType.MagicTrap;
            TrapPower = 1 * Utility.Random(1, 25);

            DropItem(new Gold(10, 40));
            DropItem(new Bolt(5));
            switch (Utility.Random(2))
            {
                case 0: DropItem(new Shoes(Utility.Random(1, 2))); break;
                case 1: DropItem(new Sandals(Utility.Random(1, 2))); break;
            }

            switch (Utility.Random(3))
            {
                case 0: DropItem(new BeverageBottle(BeverageType.Ale)); break;
                case 1: DropItem(new BeverageBottle(BeverageType.Liquor)); break;
                case 2: DropItem(new Jug(BeverageType.Cider)); break;
            }
        }

        public TreasureLevel1h(Serial serial) : base(serial)
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

    // ---------- [Level 2] ----------
    // Large, Medium and Small Crate
    // Wooden, Metal and Metal Golden Chest
    // Keg and Barrel
    [Flipable(0xe43, 0xe42)]
    public class TreasureLevel2 : BaseTreasureChestMod
    {
        [Constructable]
        public TreasureLevel2() : base(Utility.RandomList(0xe3c, 0xE3E, 0x9a9, 0xe42, 0x9ab, 0xe40, 0xe7f, 0xe77))
        {
            RequiredSkill = 72;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;
            TrapType = TrapType.MagicTrap;
            TrapPower = 2 * Utility.Random(1, 25);

            DropItem(new Gold(70, 100));
            DropItem(new Arrow(10));
            DropItem(Loot.RandomPotion());
            for (int i = Utility.Random(1, 2); i > 1; i--)
            {
                Item ReagentLoot = Loot.RandomReagent();
                ReagentLoot.Amount = Utility.Random(1, 2);
                DropItem(ReagentLoot);
            }
            if (Utility.RandomBool()) //50% chance
                for (int i = Utility.Random(8) + 1; i > 0; i--)
                    DropItem(Loot.RandomScroll(0, 39, SpellbookType.Regular));

            if (Utility.RandomBool()) //50% chance
                for (int i = Utility.Random(6) + 1; i > 0; i--)
                    DropItem(Loot.RandomGem());
        }

        public TreasureLevel2(Serial serial) : base(serial)
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

    // ---------- [Level 3] ----------
    // Wooden, Metal and Metal Golden Chest
    [Flipable(0x9ab, 0xe7c)]
    public class TreasureLevel3 : BaseTreasureChestMod
    {
        public override int DefaultGumpID => 0x4A;

        [Constructable]
        public TreasureLevel3() : base(Utility.RandomList(0x9ab, 0xe40, 0xe42))
        {
            RequiredSkill = 84;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;
            TrapType = TrapType.MagicTrap;
            TrapPower = 3 * Utility.Random(1, 25);

            DropItem(new Gold(180, 240));
            DropItem(new Arrow(10));

            for (int i = Utility.Random(1, 3); i > 1; i--)
            {
                Item ReagentLoot = Loot.RandomReagent();
                ReagentLoot.Amount = Utility.Random(1, 9);
                DropItem(ReagentLoot);
            }

            for (int i = Utility.Random(1, 3); i > 1; i--)
                DropItem(Loot.RandomPotion());

            if (0.67 > Utility.RandomDouble()) //67% chance = 2/3
                for (int i = Utility.Random(12) + 1; i > 0; i--)
                    DropItem(Loot.RandomScroll(0, 47, SpellbookType.Regular));

            if (0.67 > Utility.RandomDouble()) //67% chance = 2/3
                for (int i = Utility.Random(9) + 1; i > 0; i--)
                    DropItem(Loot.RandomGem());

            for (int i = Utility.Random(1, 3); i > 1; i--)
                DropItem(Loot.RandomWand());

            // Magical ArmorOrWeapon
            for (int i = Utility.Random(1, 3); i > 1; i--)
            {
                Item item = Loot.RandomArmorOrShieldOrWeapon();

                AddLoot(item);
            }

            for (int i = Utility.Random(1, 2); i > 1; i--)
                AddLoot(Loot.RandomClothing());

            for (int i = Utility.Random(1, 2); i > 1; i--)
                AddLoot(Loot.RandomJewelry());

            // Magic clothing (not implemented)

            // Magic jewelry (not implemented)
        }

        public TreasureLevel3(Serial serial) : base(serial)
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

    // ---------- [Level 4] ----------
    // Wooden, Metal and Metal Golden Chest
    [Flipable(0xe41, 0xe40)]
    public class TreasureLevel4 : BaseTreasureChestMod
    {
        [Constructable]
        public TreasureLevel4() : base(Utility.RandomList(0xe40, 0xe42, 0x9ab))
        {
            RequiredSkill = 92;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;
            TrapType = TrapType.MagicTrap;
            TrapPower = 4 * Utility.Random(1, 25);

            DropItem(new Gold(200, 400));
            DropItem(new BlankScroll(Utility.Random(1, 4)));

            for (int i = Utility.Random(1, 4); i > 1; i--)
            {
                Item ReagentLoot = Loot.RandomReagent();
                ReagentLoot.Amount = Utility.Random(6, 12);
                DropItem(ReagentLoot);
            }

            for (int i = Utility.Random(1, 4); i > 1; i--)
                DropItem(Loot.RandomPotion());

            if (0.75 > Utility.RandomDouble()) //75% chance = 3/4
                for (int i = Utility.RandomMinMax(8, 16); i > 0; i--)
                    DropItem(Loot.RandomScroll(0, 47, SpellbookType.Regular));

            if (0.75 > Utility.RandomDouble()) //75% chance = 3/4
                for (int i = Utility.RandomMinMax(6, 12) + 1; i > 0; i--)
                    DropItem(Loot.RandomGem());

            for (int i = Utility.Random(1, 4); i > 1; i--)
                DropItem(Loot.RandomWand());

            // Magical ArmorOrWeapon
            for (int i = Utility.Random(1, 4); i > 1; i--)
            {
                Item item = Loot.RandomArmorOrShieldOrWeapon();

                AddLoot(item);
            }

            for (int i = Utility.Random(1, 2); i > 1; i--)
                AddLoot(Loot.RandomClothing());

            for (int i = Utility.Random(1, 2); i > 1; i--)
                AddLoot(Loot.RandomJewelry());
        }

        public TreasureLevel4(Serial serial) : base(serial)
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
