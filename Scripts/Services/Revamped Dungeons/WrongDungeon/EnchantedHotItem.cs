using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Server.Mobiles;

namespace Server.Items
{
    public static class EnchantedHotItem
    {
        public static string FilePath = Path.Combine("Saves/Misc", "EnchantedHotItem.bin");

        public static void Configure()
        {
            EventSink.ItemDeleted += OnDeleted;
            EventSink.OnEnterRegion += OnEnterRegion;

            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            HotItems = new Dictionary<Item, Container>();
        }

        public static Timer Timer { get; set; }
        public static Dictionary<Item, Container> HotItems { get; set; }

        public static void CheckTimer()
        {
            if (HotItems.Count > 0)
            {
                if (Timer == null)
                {
                    Timer = Timer.DelayCall(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), OnTick);
                }
            }
            else if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public static void OnTick()
        {
            foreach(var item in HotItems.Keys)
            {
                if (!item.Deleted && item.RootParent is Mobile)
                {
                    if (((Mobile)item.RootParent).Region.IsPartOf("Wrong"))
                    {
                        AOS.Damage((Mobile)item.RootParent, Utility.RandomMinMax(10, 13), 0, 100, 0, 0, 0);

                        if (0.2 > Utility.RandomDouble())
                        {
                            ((Mobile)item.RootParent).SendLocalizedMessage(1152086); // Ouch! These stolen items are hot!
                        }
                    }
                }
            }
        }

        public static void AddProperties(Item item, ObjectPropertyList list)
        {
            if (HotItems.ContainsKey(item))
            {
                list.Add(1152081); // Enchanted Hot Item
                list.Add(1152082); // (Escape from dungeon to remove spell)
            }
        }

        public static void AddItem(Item item, Container c)
        {
            if (!HotItems.ContainsKey(item))
            {
                HotItems[item] = c;
                CheckTimer();

                item.InvalidateProperties();
            }
        }

        public static void OnDeleted(ItemDeletedEventArgs e)
        {
            Item item = e.Item;

            if (HotItems.ContainsKey(item))
            {
                HotItems.Remove(item);
                CheckTimer();
            }
        }

        public static void OnEnterRegion(OnEnterRegionEventArgs e)
        {
            Mobile m = e.From;
            bool debug = m is PlayerMobile;

            if (!(m is PlayerMobile) || e.OldRegion == null || e.NewRegion == null || m.Backpack == null)
                return;

            if (e.OldRegion.IsPartOf("Wrong") && !e.NewRegion.IsPartOf("Wrong"))
            {
                bool found = false;

                List<Item> check = new List<Item>(m.Items.Where(i => HotItems.ContainsKey(i)));
                check.AddRange(m.Backpack.Items.Where(i => HotItems.ContainsKey(i)));

                foreach (var hotItem in check)
                {
                    HotItems.Remove(hotItem);
                    found = true;

                    if (hotItem is BaseWeapon)
                        hotItem.Hue = ((BaseWeapon)hotItem).GetElementalDamageHue();
                    else
                        hotItem.Hue = 0;

                    hotItem.InvalidateProperties();
                }

                ColUtility.Free(check);

                if (found)
                {
                    m.SendLocalizedMessage(1152085); // The curse is removed from the item you stole!
                }
            }
        }

        public static bool CheckDrop(Mobile from, Container droppedTo, Item dropped)
        {
            if (HotItems.ContainsKey(dropped))
            {
                Container c = HotItems[dropped];
                if (droppedTo != c && droppedTo != from.Backpack)
                {
                    from.SendLocalizedMessage(1152083); // The stolen item magically returns to the trunk where you found it.

                    if (c != null)
                    {
                        c.DropItem(dropped);
                    }
                    else
                    {
                        dropped.Delete();
                    }

                    return false;
                }
            }

            return true;
        }

        public static bool CheckDrop(Mobile from, Item dropped)
        {
            if (HotItems.ContainsKey(dropped))
            {
                Container c = HotItems[dropped];

                from.SendLocalizedMessage(1152083); // The stolen item magically returns to the trunk where you found it.

                if (c != null)
                {
                    c.DropItem(dropped);
                }
                else
                {
                    dropped.Delete();
                }

                return false;
            }

            return true;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(HotItems.Count);
                    foreach(var kvp in HotItems)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        Item item = reader.ReadItem();
                        Container c = reader.ReadItem() as Container;

                        if (item != null && c != null)
                        {
                            AddItem(item, c);
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
                var chest = new HotItemChest(i <= 3 ? 3648 : 3649);

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

            foreach (var item in list)
                item.Delete();

            ColUtility.Free(list);

            int toSpawn = Utility.RandomMinMax(2, 4);

            for (int i = 0; i < toSpawn; i++)
            {
                Item item;

                if (i == 0)
                {
                    item = Loot.RandomScroll(0, Loot.RegularScrollTypes.Length, SpellbookType.Regular);
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

                    RunicReforging.GenerateRandomItem(item, 0, min, max, this.Map);

                    item.Hue = 1258;
                    EnchantedHotItem.AddItem(item, this);
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

            writer.Write((int)0); // version 
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
