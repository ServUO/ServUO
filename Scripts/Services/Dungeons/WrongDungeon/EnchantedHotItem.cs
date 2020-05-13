using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Items
{
    public static class EnchantedHotItem
    {
        public static string FilePath = Path.Combine("Saves/Misc", "EnchantedHotItem.bin");

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(1);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    if (version == 0)
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            Item item = reader.ReadItem();
                            Container c = reader.ReadItem() as Container;

                            if (item != null && c != null)
                            {
                                item.AttachSocket(new EnchantedHotItemSocket(c));
                            }
                        }
                    }
                });
        }

        public static Point3D[] ChestLocs =
        {
            new Point3D(5703, 664, 0), new Point3D(5703, 663, 0), new Point3D(5703, 662, 0), new Point3D(5703, 661, 0),
            new Point3D(5707, 659, 0), new Point3D(5708, 659, 0), new Point3D(5709, 659, 0)
        };

        public static void SpawnChests(Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(new Point3D(5707, 662, 0), 15);

            foreach (Item item in eable)
            {
                if (item is MetalGoldenChest)
                {
                    item.Delete();
                }
            }

            eable.Free();

            for (int i = 0; i < ChestLocs.Length; i++)
            {
                HotItemChest chest = new HotItemChest(i <= 3 ? 3648 : 3649);

                chest.MoveToWorld(ChestLocs[i], map);
                chest.SpawnLoot();
            }
        }
    }

    public class HotItemChest : LockableContainer
    {
        public HotItemChest(int itemID)
            : base(itemID)
        {
            Movable = false;
            RequiredSkill = 92;
            LockLevel = RequiredSkill - Utility.Random(1, 10);
            MaxLockLevel = RequiredSkill;

            LiftOverride = true;
            Locked = true;
        }

        public void SpawnLoot()
        {
            List<Item> list = new List<Item>(Items);

            foreach (Item item in list)
                item.Delete();

            ColUtility.Free(list);

            int toSpawn = Utility.RandomMinMax(2, 4);

            for (int i = 0; i < toSpawn; i++)
            {
                Item item;

                if (i == 0)
                {
                    item = Loot.RandomScroll(0, Loot.MageryScrollTypes.Length, SpellbookType.Regular);
                }
                else
                {
                    item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(false, false, true);

                    if (item is BaseWeapon && 0.01 > Utility.RandomDouble())
                    {
                        ((BaseWeapon)item).ExtendedWeaponAttributes.AssassinHoned = 1;
                    }

                    int min = 400;
                    int max = 1400;

                    RunicReforging.GenerateRandomItem(item, 0, min, max, Map);

                    item.Hue = 1258;
                    item.AttachSocket(new EnchantedHotItemSocket(this));
                    //EnchantedHotItem.AddItem(item, this);
                }

                DropItem(item);
            }
        }

        public override void LockPick(Mobile from)
        {
            base.LockPick(from);

            Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
                {
                    Visible = false;
                    Visible = true;

                    Locked = true;

                    SpawnLoot();
                });
        }

        public HotItemChest(Serial serial)
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

            if (!Locked)
            {
                Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
                {
                    Locked = true;
                });
            }
        }
    }
}
