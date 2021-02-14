namespace Server.Items
{
    [Flipable]
    public class ParagonChest : LockableContainer
    {
        private static readonly int[] m_ItemIDs =
        {
            0x9AB, 0xE40, 0xE41, 0xE7C
        };

        private static readonly int[] m_Hues =
        {
            0x0, 0x455, 0x47E, 0x89F, 0x8A5, 0x8AB,
            0x966, 0x96D, 0x972, 0x973, 0x979
        };

        private string m_Name;

        [Constructable]
        public ParagonChest(string name, int level)
            : base(Utility.RandomList(m_ItemIDs))
        {
            m_Name = name;
            Hue = Utility.RandomList(m_Hues);
            Fill(level + 1);
        }

        public ParagonChest(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1063449, m_Name);
        }

        public void Flip()
        {
            switch (ItemID)
            {
                case 0x9AB:
                    ItemID = 0xE7C;
                    break;
                case 0xE7C:
                    ItemID = 0x9AB;
                    break;
                case 0xE40:
                    ItemID = 0xE41;
                    break;
                case 0xE41:
                    ItemID = 0xE40;
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Name);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Name = Utility.Intern(reader.ReadString());
        }

        private void Fill(int level)
        {
            TrapType = TrapType.ExplosionTrap;
            TrapPower = level * 25;
            TrapLevel = level;
            Locked = true;

            switch (level)
            {
                case 1:
                    RequiredSkill = 36;
                    break;
                case 2:
                    RequiredSkill = 76;
                    break;
                case 3:
                    RequiredSkill = 84;
                    break;
                case 4:
                    RequiredSkill = 92;
                    break;
                case 5:
                    RequiredSkill = 100;
                    break;
                case 6:
                    RequiredSkill = 100;
                    break;
            }

            LockLevel = RequiredSkill - 10;
            MaxLockLevel = RequiredSkill + 40;

            DropItem(new Gold(level * 200));

            for (int i = 0; i < level; ++i)
                DropItem(Loot.RandomScroll(0, 63, SpellbookType.Regular));

            for (int i = 0; i < level * 2; ++i)
            {
                Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();

                if (item != null)
                {
                    TreasureMapChest.GetRandomItemStat(out int min, out int max);

                    RunicReforging.GenerateRandomItem(item, 0, min, max);

                    DropItem(item);
                }
            }

            for (int i = 0; i < level; i++)
            {
                Item item = Loot.RandomPossibleReagent();
                item.Amount = Utility.RandomMinMax(40, 60);
                DropItem(item);
            }

            for (int i = 0; i < level; i++)
            {
                Item item = Loot.RandomGem();
                DropItem(item);
            }

            DropItem(new TreasureMap(TreasureMapInfo.ConvertLevel(level + 1), (Siege.SiegeShard ? Map.Felucca : Utility.RandomBool() ? Map.Felucca : Map.Trammel)));
        }
    }
}
